package main

import (
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
	"strconv"
	"sync"
)

var categories map[int]*Categorie
var lockCategorie = sync.RWMutex{}
var idCategorie = 0

func InitCateg() {
	categories = make(map[int]*Categorie)
	categ1 := Categorie{ID: 1, Libelle: "Food",}
	categ2 := Categorie{ID: 2, Libelle: "Clothe",}
	categ3 := Categorie{ID: 3, Libelle: "Drink",}

	categories[1] = &categ1
	categories[2] = &categ2
	categories[3] = &categ3
	idCategorie = 4
}

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
	if articles[id] != nil {
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
	lockCategorie.Lock()
	categ.ID = idCategorie
	idArticle++
	categories[categ.ID] = &categ
	lockCategorie.Unlock()
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
	delete(categories, code)
	lockCategorie.Unlock()
	log.Println("category deleted : ", code)
	w.WriteHeader(http.StatusOK)
}
