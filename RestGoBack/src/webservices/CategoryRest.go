package webservices

import (
	"encoding/json"
	"encoding/xml"
	"github.com/gorilla/mux"
	"log"
	"net/http"
	"strconv"
	"strings"
)

func GetCategories(w http.ResponseWriter, r *http.Request) {
	contentType := r.Header.Get("Content-Type")
	log.Println("get  all category")
	categoriesList := getAllCategoryWithXml()
	if strings.Contains(contentType, "xml") {
		w.Header().Set("Content-Type", "application/xml; charset=utf-8")
		var categories Categories
		categories.CategoryList = getMapCategAsArray(categoriesList)
		_ = xml.NewEncoder(w).Encode(categories)
	} else {
		w.Header().Set("Content-Type", "application/json; charset=utf-8")
		_ = json.NewEncoder(w).Encode(categoriesList)
	}
}

func GetCategoryById(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	params := mux.Vars(r)
	id, _ := strconv.Atoi(params["id"])
	categories := getAllCategoryWithXml()
	var dataToSend *Categorie
	for _, category := range categories {
		if category.ID == id {
			dataToSend = category
			break
		}
	}
	contentType := r.Header.Get("Content-Type")
	if strings.Contains(contentType, "xml") {
		w.Header().Set("Content-Type", "application/xml; charset=utf-8")
		_ = xml.NewEncoder(w).Encode(dataToSend)
	} else {
		w.Header().Set("Content-Type", "application/json; charset=utf-8")
		_ = json.NewEncoder(w).Encode(dataToSend)
	}
}

func CreateCategory(w http.ResponseWriter, r *http.Request) {
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
