package main

import (
	"encoding/json"
	"fmt"
	"github.com/gorilla/mux"
	"net/http"
	"strconv"
)

var categories []Categorie

func InitCateg() {
	categories = append(categories, Categorie{ID: 1, lbelle:"Food"})
	categories = append(categories, Categorie{ID: 1, lbelle:"Clothes"})
	categories = append(categories, Categorie{ID: 1, lbelle:"Video Games"})
}

func GetCategogies(w http.ResponseWriter, r *http.Request) {
	_ = json.NewEncoder(w).Encode(categories)
	fmt.Println("get all")
}

func GetCategogie(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	i, err := strconv.ParseInt(params["id"], 10, 64)
	var isJsonSend = false
	for _, item := range categories {
		if item.ID == i {
			_ = json.NewEncoder(w).Encode(item)
			isJsonSend = true
			fmt.Println("get ", mux.Vars(r), " : ", err)
		}
	}
	if !isJsonSend {
		GetArticles(w, r)
	}
}

func CreateCategogie(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	var categorie Categorie
	_ = json.NewDecoder(r.Body).Decode(&categorie)
	i, err := strconv.ParseInt(params["id"], 10, 64)
	categorie.ID = i
	categories = append(categories, categorie)
	_ = json.NewEncoder(w).Encode(articles)
	fmt.Println("create : ", err)
}

func DeleteCategogie(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	for index, item := range categories {
		i, err := strconv.ParseInt(params["id"], 10, 64)
		if item.ID == i {
			categories = append(categories[:index], categories[index+1])
			fmt.Println("delete : ", err)
			break
		}
	}
	json.NewEncoder(w).Encode(articles)
}
