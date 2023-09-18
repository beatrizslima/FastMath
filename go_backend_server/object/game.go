package object

import (
	"encoding/json"
	"fast_math_server/model"
	"log"

	"github.com/google/uuid"
)

type Game struct {
	MatchID   string
	Player1   *Player
	Player2   *Player
	Status    string
	GameState model.GameState
}

func NewGame(p1 *Player, p2 *Player) *Game {
	p1.points = 0
	p2.points = 0
	roundMsg := model.NewRoundData()
	game := Game{
		MatchID: uuid.New().String(),
		Player1: p1,
		Player2: p2,
		Status:  model.Starting,
		GameState: model.GameState{
			GameEnded: false,
			RoundData: roundMsg,
		},
	}

	go game.RouteMessage()

	initMsgP1 := model.GameStart{
		YourPoints:   0,
		OpponentName: p2.name,
	}

	initMsgP2 := model.GameStart{
		YourPoints:   0,
		OpponentName: p2.name,
	}

	startState := model.GameState{
		GameEnded: false,
		Winner:    false,
		RoundData: roundMsg,
	}

	p1.SendMessage(initMsgP1.ToSocketBytes())

	p2.SendMessage(initMsgP2.ToSocketBytes())

	p1.SendMessage(startState.ToSocketBytes())

	p2.SendMessage(startState.ToSocketBytes())

	return &game
}

// RouteMessage ..
func (g *Game) RouteMessage() {
	log.Println("Starting RouteMessage ...")
BREAK:
	for {
		select {
		case msgBytes := <-g.Player1.msgFromClient:
			msg := model.ParseSocketMessage(msgBytes)
			log.Println(g.Player1.name, " Sent the message: ", msg)
			g.handleMessage(g.Player1, msg)

		case msgBytes := <-g.Player2.msgFromClient:
			msg := model.ParseSocketMessage(msgBytes)
			log.Println(g.Player2.name, " Sent the message: ", msg)
			g.Player1.SendMessage(msg.ToBytes())
			g.handleMessage(g.Player2, msg)

		case <-g.Player1.exitChan:
			log.Println(g.Player1.name, " has exited the game")
			g.Player2.SendMessage(getGameWinnerMessage(g.Player1.name))
			go g.Player2.Close()
			break BREAK

		case <-g.Player2.exitChan:
			log.Println(g.Player2.name, " has exited the game")
			g.Player1.SendMessage(getGameWinnerMessage(g.Player2.name))
			go g.Player1.Close()
			break BREAK
		}
	}
	log.Println("Closing RouteMessage game for", g.Player1.name, " and ", g.Player2.name)
}

func getGameWinnerMessage(OpponentName string) []byte {
	gW := model.GameState{
		GameEnded: true,
		Winner:    true,
	}
	winMessage, _ := json.Marshal(gW)
	msg := model.SocketMessage{
		Type:    model.End,
		Message: winMessage,
	}
	return msg.ToBytes()
}
func getGameLoserMessage(OpponentName string) []byte {
	gW := model.GameState{
		GameEnded: true,
		Winner:    false,
	}
	winMessage, _ := json.Marshal(gW)
	msg := model.SocketMessage{
		Type:    model.End,
		Message: winMessage,
	}
	return msg.ToBytes()
}

func (g *Game) handleMessage(player *Player, msg *model.SocketMessage) {
	switch msg.Type {
	case model.Guessed:
		guess := model.ParseGuess(msg.Message)
		switch g.Status {
		case model.Starting, model.Waiting:
			if g.checkIfGuessIsRight(guess) {
				if player == g.Player1 {
					handleRightAnswer(guess.RoundId, g.Player1, g.Player2)
				} else {
					handleRightAnswer(guess.RoundId, g.Player2, g.Player1)
				}
				g.UpdateGameState()
			} else {
				if player == g.Player1 {
					handleWrongAnswer(guess.RoundId, g.Player1)
				} else {
					handleWrongAnswer(guess.RoundId, g.Player2)
				}
			}
		}
	case model.End:
		break
	}
}

func (g *Game) checkIfGuessIsRight(guess *model.Guess) bool {
	if g.GameState.RoundData.RoundId == guess.RoundId {
		rightAnswer := g.GameState.RoundData.Value1 * g.GameState.RoundData.Value2
		if guess.Guess == rightAnswer {
			return true
		}
	}
	return false
}

func handleRightAnswer(roundId string, right *Player, late *Player) {
	right.points += 1
	late.points -= 1

	responseLate, err := json.Marshal(model.GuessResponse{
		RoundId: roundId,
		Points:  late.points,
		Answer:  "late",
	})
	if err != nil {
		log.Println("Error marshaling guess response")
	}
	msgLate := model.SocketMessage{
		Type:    model.Guessed,
		Message: responseLate,
	}
	late.SendMessage(msgLate.ToBytes())

	responseRight, err := json.Marshal(model.GuessResponse{
		RoundId: roundId,
		Points:  right.points,
		Answer:  "right",
	})
	if err != nil {
		log.Println("Error marshaling guess response")
	}
	msgRight := model.SocketMessage{
		Type:    model.Guessed,
		Message: responseRight,
	}
	right.SendMessage(msgRight.ToBytes())
}

func handleWrongAnswer(roundId string, wrong *Player) {
	responseRight, err := json.Marshal(model.GuessResponse{
		RoundId: roundId,
		Answer:  "wrong",
	})
	if err != nil {
		log.Println("Error marshaling guess response")
	}
	msgRight := model.SocketMessage{
		Type:    model.Guessed,
		Message: responseRight,
	}
	wrong.SendMessage(msgRight.ToBytes())
}

func (g *Game) UpdateGameState() {
	if g.Player1.points >= 5 {
		msgWinner := getGameWinnerMessage(g.Player2.name)
		msgLoser := getGameLoserMessage(g.Player1.name)
		g.Player1.SendMessage(msgWinner)
		g.Player2.SendMessage(msgLoser)
		g.Status = model.End

		go g.Player1.Close()
		go g.Player2.Close()
	}
	if g.Player2.points >= 5 {
		msgWinner := getGameWinnerMessage(g.Player1.name)
		msgLoser := getGameLoserMessage(g.Player2.name)
		g.Player2.SendMessage(msgWinner)
		g.Player1.SendMessage(msgLoser)
		g.Status = model.End

		go g.Player1.Close()
		go g.Player2.Close()
	}

	g.GameState.RoundData = model.NewRoundData()
	g.Status = model.Waiting

	g.Player1.SendMessage(g.GameState.ToSocketBytes())
	g.Player2.SendMessage(g.GameState.ToSocketBytes())
}
