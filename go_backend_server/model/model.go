package model

import (
	"encoding/json"
	"log"
	"math/rand"

	"github.com/google/uuid"
)

const (
	Beggining = "beggining"
	Starting  = "starting"
	Waiting   = "waiting"
	Guessed   = "guessed"
	End       = "end"
)

type GameState struct {
	GameEnded bool
	Winner    bool
	RoundData *RoundData
}

type GameStart struct {
	YourPoints   int
	OpponentName string
}

type Guess struct {
	RoundId string
	Value1  int
	Value2  int
	Guess   int
}

type GuessResponse struct {
	RoundId string
	Points  int
	Answer  string //right, wrong, late
}

type SocketMessage struct {
	Type    string
	Message json.RawMessage
}

type RoundData struct {
	RoundId      string
	Value1       int
	Value2       int
	Alternatives [3]int
}

func NewRoundData() *RoundData {

	position := rand.Intn(2)
	round := RoundData{
		RoundId:      uuid.New().String(),
		Value1:       rand.Intn(10),
		Value2:       rand.Intn(10),
		Alternatives: [3]int{rand.Intn(100), rand.Intn(100), rand.Intn(100)},
	}
	round.Alternatives[position] = round.Value1 * round.Value2

	return &round
}

func ParseSocketMessage(msgBytes []byte) *SocketMessage {
	msg := SocketMessage{}
	err := json.Unmarshal(msgBytes, &msg)
	if err != nil {
		log.Println("ERROR", "Error in recieving message", err)
	}
	return &msg
}

func ParseGuess(msg json.RawMessage) *Guess {
	guess := Guess{}
	err := json.Unmarshal(msg, &guess)
	if err != nil {
		log.Println("ERROR", "Error when parsing guess json", err)
	}
	return &guess
}

// ToBytes returns the socket message in bytes
func (msg *SocketMessage) ToBytes() (returnMsg []byte) {
	returnMsg, err := json.Marshal(msg)
	if err != nil {
		returnMsg = []byte(err.Error())
	}
	return
}

// ToSocketBytes returns the game start message to socket message in bytes
func (msg *GameState) ToSocketBytes() []byte {
	gameState, err := json.Marshal(msg)
	if err != nil {
		gameState = []byte(err.Error())
	}
	sm := SocketMessage{
		Type:    Starting,
		Message: gameState,
	}
	return sm.ToBytes()
}
func (msg *GameStart) ToSocketBytes() []byte {
	gameStart, err := json.Marshal(msg)
	if err != nil {
		gameStart = []byte(err.Error())
	}
	sm := SocketMessage{
		Type:    Beggining,
		Message: gameStart,
	}
	return sm.ToBytes()
}
