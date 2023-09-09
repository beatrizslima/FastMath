package object

import (
	"log"
	"sync"
	"time"

	"github.com/gorilla/websocket"
)

type Player struct {
	conn          *websocket.Conn
	name          string
	msgFromClient chan []byte
	exitChan      chan int
	sendMutex     sync.Mutex
	points        int
}

func NewPlayer(playerName string, conn *websocket.Conn) *Player {
	return &Player{
		conn:          conn,
		msgFromClient: make(chan []byte, 5),
		exitChan:      make(chan int),
		name:          playerName,
	}
}

func (p *Player) RecieveMessages() {
	for {
		_, message, err := p.conn.ReadMessage()
		p.msgFromClient <- message
		if err != nil {
			log.Println("Could not recieve message from player ")
		}
	}
}

func (p *Player) SendMessage(msg []byte) error {
	p.sendMutex.Lock()
	defer p.sendMutex.Unlock()

	log.Println("Sending message to", p.name, ":", string(msg))
	return p.conn.WriteMessage(1, msg)
}

func (p *Player) Close() {
	time.Sleep(1 * time.Second)
	p.conn.Close()
}
