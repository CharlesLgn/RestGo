package main

import (
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
	"strconv"
	"sync"
)


var articles map[int]*Article
var lockArticle = sync.RWMutex{}
var idArticle = 0

func InitArticle() {
	articles = make(map[int]*Article)
	article1 := Article{ID: 1, Libelle: "Cookies", Prix: 2.99, IdCategorie: 1,}
	article2 := Article{ID: 2, Libelle: "Brownies", Prix: 1.99, IdCategorie: 1,}
	article3 := Article{ID: 3, Libelle: "Cake", Prix: 4.99, IdCategorie: 1,}

	articles[1] = &article1
	articles[2] = &article2
	articles[3] = &article3
	idArticle = 4
}

func GetArticle(w rest.ResponseWriter, r *rest.Request) {
	id, _ := strconv.Atoi(r.PathParam("id"))
	log.Println("get  article : ", id)
	lockArticle.RLock()
	var article *Article
	if articles[id] != nil {
		article = &Article{}
		*article = *articles[id]
	}
	lockArticle.RUnlock()

	if article == nil {
		rest.NotFound(w, r)
		return
	}
	log.Println(article)
	w.WriteJson(article)
}

func GetArticles(w rest.ResponseWriter, r *rest.Request) {
	lockArticle.RLock()
	log.Println("get  all articles")
	stock := make([]Article, len(articles))
	i := 0
	for _, article := range articles {
		stock[i] = *article
		i++
	}
	lockArticle.RUnlock()
	w.WriteJson(&stock)
}

func CreateArticle(w rest.ResponseWriter, r *rest.Request) {
	article := Article{}
	err := r.DecodeJsonPayload(&article)
	if err != nil {
		rest.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	if article.Libelle == "" {
		rest.Error(w, "We need a Name", 400)
		return
	}
	if article.Prix < 0 {
		rest.Error(w, "Price can't be negative", 400)
		return
	}
	lockArticle.Lock()
	article.ID = idArticle
	idArticle++
	articles[article.ID] = &article
	lockArticle.Unlock()
	w.WriteJson(&article)
}

func DeleteArticle(w rest.ResponseWriter, r *rest.Request) {
	code, err := strconv.Atoi(r.PathParam("id"))
	if err != nil {
		rest.Error(w, "Id use to be an int", 400)
		return
	}
	lockArticle.Lock()
	if articles[code] == nil {
		rest.Error(w, "no data to delete found", 400)
		return
	}
	delete(articles, code)
	lockArticle.Unlock()
	w.WriteHeader(http.StatusOK)
}
