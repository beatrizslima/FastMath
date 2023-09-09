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
	GameState string
}

func NewGame(p1 *Player, p2 *Player) *Game {
	p1.points = 0
	p2.points = 0
	game := Game{
		MatchID:   uuid.New().String(),
		Player1:   p1,
		Player2:   p2,
		GameState: model.Starting,
	}

	go game.RouteMessage()

	initMsgP1 := model.GameState{
		YourPoints:   p1.points,
		OpponentName: p2.name,
		GameEnded:    false,
		Winner:       false,
	}

	initMsgP2 := model.GameState{
		YourPoints:   p2.points,
		OpponentName: p1.name,
		GameEnded:    false,
		Winner:       false,
	}

	p1.SendMessage(initMsgP1.ToSocketBytes())

	p2.SendMessage(initMsgP2.ToSocketBytes())

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
			g.Player2.SendMessage(msg.ToBytes())

		case msgBytes := <-g.Player2.msgFromClient:
			msg := model.ParseSocketMessage(msgBytes)
			log.Println(g.Player2.name, " Sent the message: ", msg)
			g.Player1.SendMessage(msg.ToBytes())

		case <-g.Player1.exitChan:
			log.Println(g.Player1.name, " has exited the game")
			g.Player2.SendMessage(getGameEndMessage(g.Player2.name))
			go g.Player2.Close()
			break BREAK

		case <-g.Player2.exitChan:
			log.Println(g.Player2.name, " has exited the game")
			g.Player1.SendMessage(getGameEndMessage(g.Player1.name))
			go g.Player1.Close()
			break BREAK
		}
	}
	log.Println("Closing RouteMessage game for", g.Player1.name, " and ", g.Player2.name)
}

func getGameEndMessage(winner string) []byte {
	gW := model.GameState{
		YourPoints:   5,
		OpponentName: winner,
		GameEnded:    true,
		Winner:       true,
	}
	winMessage, _ := json.Marshal(gW)
	msg := model.SocketMessage{
		Type:    model.End,
		Message: winMessage,
	}
	return msg.ToBytes()
}
