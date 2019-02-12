package main

import (
	"fmt"
	"github.com/gorilla/mux"
	"log"
	"net/http"
)

var people []Person

func main() {
	fmt.Print("Server started\n...")
	router := mux.NewRouter()
	router.HandleFunc("/people", GetPeople).Methods("GET")
	router.HandleFunc("/people/{id}", GetPerson).Methods("GET")
	router.HandleFunc("/people/{id}", CreatePerson).Methods("POST")
	router.HandleFunc("/people/{id}", DeletePerson).Methods("DELETE")
	fmt.Print("\n...\nServer start")
	log.Fatal(http.ListenAndServe(":8000", router))
}

func GetPeople(w http.ResponseWriter, r *http.Request) {
	fmt.Print("get all")
}

func GetPerson(w http.ResponseWriter, r *http.Request) {
	fmt.Print("get ", r.Body)
}

func CreatePerson(w http.ResponseWriter, r *http.Request) {
	fmt.Print("create")
}

func DeletePerson(w http.ResponseWriter, r *http.Request) {
	fmt.Print("delete")
}
