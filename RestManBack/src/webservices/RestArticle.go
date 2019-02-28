package webservices

import (
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
	"strconv"
)

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
	addArticle(article)
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

func UpdateArticle(w rest.ResponseWriter, r *rest.Request) {
	code, err1 := strconv.Atoi(r.PathParam("id"))
	if err1 != nil {
		rest.Error(w, "Id use to be an int", 400)
		return
	}
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
	if articles[code] == nil {
		rest.Error(w, "not fount to update", 400)
		return
	}
	lockCategorie.Lock()
	article.ID=code
	articles[code] = &article
	lockCategorie.Unlock()

	addArticle(article)
	w.WriteJson(&article)
}

func PatchArticle(w rest.ResponseWriter, r *rest.Request) {
	code, err1 := strconv.Atoi(r.PathParam("id"))
	if err1 != nil {
		rest.Error(w, "Id use to be an int", 400)
		return
	}
	article := Article{}
	err := r.DecodeJsonPayload(&article)
	if articles[code] == nil {
		rest.Error(w, "not fount to update", 400)
		return
	}
	lockCategorie.Lock()
	if err != nil {
		rest.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	if categories[article.IdCategorie] != nil {
		articles[code].IdCategorie = article.IdCategorie
	}
	if article.Libelle != "" {
		articles[code].Libelle = article.Libelle
	}
	if article.Prix >= 0 {
		articles[code].Prix = article.Prix
	}
	lockCategorie.Unlock()
	log.Println("article patch")
	w.WriteJson(&article)
}