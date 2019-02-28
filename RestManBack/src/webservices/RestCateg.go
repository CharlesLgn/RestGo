package main

import (
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
	"strconv"
)

func GetCategogies(w rest.ResponseWriter, r *rest.Request) {
	lockCategorie.RLock()
	log.Println("get  all category")
	stock := make([]Categorie, len(categories))
	i := 0
	for _, categ := range categories {
		stock[i] = *categ
		i++
	}
	lockCategorie.RUnlock()
	w.WriteJson(&stock)
}

func GetCategogie(w rest.ResponseWriter, r *rest.Request) {
	id, _ := strconv.Atoi(r.PathParam("id"))
	log.Println("get  category : ", id)
	lockCategorie.RLock()
	var categ *Categorie
	if categories[id] != nil {
		categ = &Categorie{}
		*categ = *categories[id]
	}
	lockCategorie.RUnlock()

	if categ == nil {
		rest.NotFound(w, r)
		return
	}
	log.Println(categ)
	w.WriteJson(categ)
}

func CreateCategogie(w rest.ResponseWriter, r *rest.Request) {
	categ := Categorie{}
	err := r.DecodeJsonPayload(&categ)
	if err != nil {
		rest.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	if categ.Libelle == "" {
		rest.Error(w, "We need a Name", 400)
		return
	}
	addCateg(categ)
	log.Println("category created")
	w.WriteJson(&categ)
}

func DeleteCategogie(w rest.ResponseWriter, r *rest.Request) {
	code, err := strconv.Atoi(r.PathParam("id"))
	if err != nil {
		rest.Error(w, "Id use to be an int", 400)
		return
	}
	lockCategorie.Lock()
	if categories[code] == nil {
		rest.Error(w, "no data to delete found", 400)
		return
	}
	deleteArticles(code)
	delete(categories, code)
	lockCategorie.Unlock()
	log.Println("category deleted : ", code)
	w.WriteHeader(http.StatusOK)
}

func UpdateCategogie(w rest.ResponseWriter, r *rest.Request) {
	code, err1 := strconv.Atoi(r.PathParam("id"))
	if err1 != nil {
		rest.Error(w, "Id use to be an int", 400)
		return
	}
	categ := Categorie{}
	err := r.DecodeJsonPayload(&categ)
	if err != nil {
		rest.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	if categ.Libelle == "" {
		rest.Error(w, "We need a Name", 400)
		return
	}
	if categories[code] == nil {
		rest.Error(w, "not fount to update", 400)
		return
	}
	lockCategorie.Lock()
	categ.ID=code
	categories[code] = &categ
	lockCategorie.Unlock()
	log.Println("category updated")
	w.WriteJson(&categ)
}
