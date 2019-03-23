package webservices

import (
  "encoding/json"
  "github.com/gorilla/mux"
  "log"
  "net/http"
  "strconv"
)

func GetArticles(w http.ResponseWriter, r *http.Request) {
  log.Println("get  all articles")
  articles := getAllArticleInXml()
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
  _ = json.NewEncoder(w).Encode(articles)
}

func GetArticleById(w http.ResponseWriter, r *http.Request) {
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
  params := mux.Vars(r)
  id, _ := strconv.Atoi(params["id"])
  articles := getAllArticleInXml()
  for _, article := range articles {
    if article.ID == id {
      _ = json.NewEncoder(w).Encode(article)
      break
    }
  }
}

func CreateArticle(w http.ResponseWriter, r *http.Request) {
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
  var article Article
  err := json.NewDecoder(r.Body).Decode(&article)
  if err != nil {
    http.Error(w, err.Error(), http.StatusInternalServerError)
    return
  }
  if article.Libelle == "" {
    http.Error(w, "We need a Name", 400)
    return
  }
  if article.Prix < 0 {
    http.Error(w, "Price can't be negative", 400)
    return
  }
  addArticle(article)
  _ = json.NewEncoder(w).Encode(article)
}

func DeleteArticle(w http.ResponseWriter, r *http.Request) {
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
  DeleteArticleInXml(id)
  w.WriteHeader(http.StatusOK)
}

func UpdateArticle(w http.ResponseWriter, r *http.Request) {
  params := mux.Vars(r)
  id, err := strconv.Atoi(params["id"])
  if err != nil {
    http.Error(w, "Id use to be an int", 400)
    return
  }
  article := Article{}
  err = json.NewDecoder(r.Body).Decode(&article)
  if err != nil {
    http.Error(w, err.Error(), http.StatusInternalServerError)
    return
  }
  article.ID = id
  UpdateArticleInXml(&article)
  _ = json.NewEncoder(w).Encode(article)
}

func OverrideArticle(w http.ResponseWriter, r *http.Request) {
  params := mux.Vars(r)
  id, err := strconv.Atoi(params["id"])
  if err != nil {
    http.Error(w, "Id use to be an int", 400)
    return
  }
  articles := getAllArticleInXml()
  article := Article{}
  err = json.NewDecoder(r.Body).Decode(&article)
  if articles[id] == nil {
    http.Error(w, "not fount to update", 400)
    return
  } else if err != nil {
    http.Error(w, err.Error(), http.StatusInternalServerError)
    return
  } else if article.Libelle == "" {
    http.Error(w, "We need a Name", 400)
    return
  } else if article.Prix < 0 {
    http.Error(w, "Price can't be negative", 400)
    return
  }
  article.ID = id
  OverwriteArticleInXml(&article)
  log.Println("article patch")
  _ = json.NewEncoder(w).Encode(article)
}
