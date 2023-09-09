package main

import (
	"fast_math_server/object"
	"flag"
	"fmt"
	"log"
	"net/http"

	"github.com/gorilla/websocket"
)

var addr = flag.String("addr", "0.0.0.0:4000", "Server address")

func main() {
	flag.Parse()

	http.HandleFunc("/connect", newConnectionParse)
	http.HandleFunc("/health", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, "JumpAndShoot engine is running on this port")
	})

	err := http.ListenAndServe(*addr, nil)
	if err != nil {
		log.Fatal("ListenAndServe: ", err)
	}
}

var upgrader = websocket.Upgrader{
	ReadBufferSize:  2048,
	WriteBufferSize: 2048,
	CheckOrigin: func(r *http.Request) bool {
		return true
	},
	EnableCompression: true,
}

func newConnectionParse(w http.ResponseWriter, r *http.Request) {
	playerName := r.URL.Query()["id"][0]
	log.Println("Player Joined: ", playerName)

	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println("Error on upgrade, ", err)
	}

	p := object.NewPlayer(playerName, conn)

	go p.RecieveMessages()

	object.GS.Match(p)
}
