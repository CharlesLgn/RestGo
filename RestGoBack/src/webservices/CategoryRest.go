package webservices

import (
	"encoding/json"
	"log"
	"net/http"
	"strconv"

	"github.com/gorilla/mux"
)

func GetCategories(w http.ResponseWriter, r *http.Request) {
	setHeader(w, r)
	id, err := strconv.Atoi(r.Header.Get("X-Category-id"))
	if err == nil {
		getCategoryById(w, r, id)
	} else {
		categoriesList := getAllCategoryWithXml()
		var categories Categories
		categories.CategoryList = getMapCategAsArray(categoriesList)
		encode(w, r, categories, categoriesList, "get all Categories")
	}
}

func getCategoryById(w http.ResponseWriter, r *http.Request, id int) {
	categories := getAllCategoryWithXml()
	var dataToSend *Categorie
	for _, categ := range categories {
		if categ.ID == id {
			dataToSend = categ
		}
	}
	if dataToSend == nil {
		http.Error(w, "no article with this id", http.StatusNotFound)
	} else {
		encode(w, r, dataToSend, dataToSend, "get category " + strconv.Itoa(id))
	}
}

func GetCategoryById(w http.ResponseWriter, r *http.Request) {
	setHeader(w, r)
	params := mux.Vars(r)
	id, _ := strconv.Atoi(params["id"])
	getCategoryById(w, r, id)
}

func CreateCategory(w http.ResponseWriter, r *http.Request) {
	setHeader(w, r)
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	var category Categorie
	err := json.NewDecoder(r.Body).Decode(&category)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	if category.Libelle == "" {
		http.Error(w, "We need a Name", 400)
		return
	}
	addCateg(category)
	_ = json.NewEncoder(w).Encode(category)
}

func DeleteCategory(w http.ResponseWriter, r *http.Request) {
	setHeader(w, r)
	params := mux.Vars(r)
	id, err := strconv.Atoi(params["id"])
	if err != nil {
		http.Error(w, "Id use to be an int", 400)
		return
	}
	articles := getAllArticleInXml()
	if articles[id] == nil {
		http.Error(w, "no data to delete found", 400)
		return
	}
	DeleteCategoryInXml(id)
	w.WriteHeader(http.StatusOK)
}

func UpdateCategory(w http.ResponseWriter, r *http.Request) {
	setHeader(w, r)
	params := mux.Vars(r)
	id, err := strconv.Atoi(params["id"])
	if err != nil {
		http.Error(w, "Id use to be an int", 400)
		return
	}
	category := Categorie{}
	err = json.NewDecoder(r.Body).Decode(&category)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	category.ID = id
	UpdateCategoryInXml(&category)
	_ = json.NewEncoder(w).Encode(category)
}

func OverrideCategory(w http.ResponseWriter, r *http.Request) {
	setHeader(w, r)
	params := mux.Vars(r)
	id, err := strconv.Atoi(params["id"])
	if err != nil {
		http.Error(w, "Id use to be an int", 400)
		return
	}
	categories := getAllCategoryWithXml()
	category := Categorie{}
	err = json.NewDecoder(r.Body).Decode(&category)
	if categories[id] == nil {
		http.Error(w, "not fount to update", 400)
		return
	} else if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	} else if category.Libelle == "" {
		http.Error(w, "We need a Name", 400)
		return
	}
	category.ID = id
	OverwriteCategoryInXml(&category)
	log.Println("article patch")
	_ = json.NewEncoder(w).Encode(category)
}
