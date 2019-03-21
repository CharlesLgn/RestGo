package webservices

import (
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
	"strconv"
)

func GetArticleByCateg(w rest.ResponseWriter, r *rest.Request) {
	idCateg, _ := strconv.Atoi(r.PathParam("id"))
	log.Println("get  article by categ: ", idCateg)
	lockArticle.RLock()

	stock := make([]Article, 0)
	for _, article := range articles {
		log.Println(article)
		if article.IdCategorie == idCateg {
			stock = insert(stock, *article)
		}
	}
	lockArticle.RUnlock()
	w.WriteJson(stock)
}

func DeleteArticlesByCateg(w rest.ResponseWriter, r *rest.Request) {
	idCateg, _ := strconv.Atoi(r.PathParam("id"))
	log.Println("delete article by categ: ", idCateg)
	deleteArticles(idCateg)
	w.WriteHeader(http.StatusOK)
}

func deleteArticles(idCateg int) {
	lockArticle.RLock()
	for _, article := range articles {
		if article.IdCategorie == idCateg {
			delete(articles, article.ID)
			deleteArticles(article.ID)
		}
	}
	lockArticle.RUnlock()
}

func insert(original []Article, value Article) []Article {
	target := make([]Article, len(original)+1)
	copy(target, original[:])
	target[len(original)] = value

	return target
}
