package main

import (
	"fmt"
	"github.com/gorilla/mux"
	"log"
	"net/http"
)

func main() {
	fmt.Println("Server started...")
	router := mux.NewRouter()
	fmt.Println("  Article:")
	InitArticle()
	router.HandleFunc("/article", GetArticles).Methods("GET")
	router.HandleFunc("/article/{id}", GetArticle).Methods("GET")
	fmt.Println("    GET")
	router.HandleFunc("/article/{id}", CreateArticle).Methods("POST")
	fmt.Println("    POST")
	router.HandleFunc("/article/{id}", DeleteArticle).Methods("DELETE")
	fmt.Println("    DELETE")
	fmt.Println("  Categorie:")
	InitCateg()
	router.HandleFunc("/categ", GetCategogies).Methods("GET")
	router.HandleFunc("/categ/{id}", GetCategogie).Methods("GET")
	fmt.Println("    GET")
	router.HandleFunc("/categ/{id}", CreateCategogie).Methods("POST")
	fmt.Println("    POST")
	router.HandleFunc("/categ/{id}", DeleteCategogie).Methods("DELETE")
	fmt.Println("    DELETE")
	fmt.Println("...Server start")
	log.Fatal(http.ListenAndServe(":8000", router))
}