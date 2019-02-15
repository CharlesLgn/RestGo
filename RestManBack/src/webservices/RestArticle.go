package main

import (
	"encoding/json"
	"fmt"
	"github.com/gorilla/mux"
	"net/http"
	"strconv"
)

var articles []Article

func InitArticle() {
	articles = append(articles, Article{ID: 1, Libelle: "Cookies", Prix: 2.99, Categorie: &Categorie{ID: 1, lbelle:"Food"}})
	articles = append(articles, Article{ID: 2, Libelle: "Brownies", Prix: 1.99, Categorie: &Categorie{ID: 1, lbelle:"Food"}})
	articles = append(articles, Article{ID: 3, Libelle: "Cake", Prix: 4.99, Categorie: &Categorie{ID: 1, lbelle:"Food"}})
}

func GetArticles(w http.ResponseWriter, r *http.Request) {
	_ = json.NewEncoder(w).Encode(articles)
	fmt.Println("get all")
}

func GetArticle(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	i, _ := strconv.ParseInt(params["id"], 10, 64)
	var isJsonSend = false
	for _, item := range articles {
		if item.ID == i {
			_ = json.NewEncoder(w).Encode(item)
			isJsonSend = true
			fmt.Println("get ", mux.Vars(r),)
		}
	}
	if !isJsonSend {
		GetArticles(w, r)
	}
}

func CreateArticle(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	var article Article
	_ = json.NewDecoder(r.Body).Decode(&article)
	i, _ := strconv.ParseInt(params["id"], 10, 64)
	article.ID = i
	articles = append(articles, article)
	_ = json.NewEncoder(w).Encode(articles)
	fmt.Println("created !")
}

func DeleteArticle(w http.ResponseWriter, r *http.Request) {
	params := mux.Vars(r)
	for index, item := range articles {
		i, _ := strconv.ParseInt(params["id"], 10, 64)
		if item.ID == i {
			articles = append(articles[:index], articles[index+1])
			fmt.Println("deleted ! ")
			break
		}
	}
	_ = json.NewEncoder(w).Encode(articles)
}
