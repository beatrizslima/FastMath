package model

import (
	"encoding/json"
	"log"
)

const (
	Starting = "starting"
	Waiting  = "waiting"
	Guessed  = "guessed"
	End      = "end"
)

type GameState struct {
	YourPoints   int
	OpponentName string
	GameEnded    bool
	Winner       bool
}

type SocketMessage struct {
	Type    string
	Message json.RawMessage
}

func ParseSocketMessage(msgBytes []byte) *SocketMessage {
	msg := SocketMessage{}
	err := json.Unmarshal(msgBytes, &msg)
	if err != nil {
		log.Println("ERROR", "Error in recieving message", err)
	}
	return &msg
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
		Type:    starting,
		Message: gameState,
	}
	return sm.ToBytes()
}
