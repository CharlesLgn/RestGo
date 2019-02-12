package main

import (
	"fmt"
	"github.com/gorilla/mux"
	"log"
	"net/http"
)

func main() {
	fmt.Print("Server started\n...")
	router := mux.NewRouter()
	InitArticle()
	router.HandleFunc("/people", GetArticles).Methods("GET")
	router.HandleFunc("/people/{id}", GetArticle).Methods("GET")
	router.HandleFunc("/people/{id}", CreateArticle).Methods("POST")
	router.HandleFunc("/people/{id}", DeleteArticle).Methods("DELETE")
	fmt.Print("\n...\nServer start\n")
	log.Fatal(http.ListenAndServe(":8000", router))
}
