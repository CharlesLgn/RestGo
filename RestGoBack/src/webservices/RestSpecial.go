package webservices

import (
  "github.com/gorilla/mux"

  "encoding/json"
  "log"
  "net/http"
  "strconv"
)

func GetArticleByCateg(w http.ResponseWriter, r *http.Request) {
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
  params := mux.Vars(r)
  idCateg, _ := strconv.Atoi(params["id"])
  log.Println("get  article by categ: ", idCateg)
  lockArticle.RLock()

  stock := make([]Article, 0)
  articles := getAllArticleInXml()

  for _, article := range articles {
    if article.IdCategorie == idCateg {
      stock = insert(stock, *article)
    }
  }
  lockArticle.RUnlock()
  _ = json.NewEncoder(w).Encode(stock)
}

func DeleteArticlesByCateg(w http.ResponseWriter, r *http.Request) {
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
  params := mux.Vars(r)
  idCateg, _ := strconv.Atoi(params["id"])
  log.Println("delete article by categ: ", idCateg)
  deleteArticles(idCateg)
  w.WriteHeader(http.StatusOK)
}

func deleteArticles(idCateg int) {
  idArticlesToDelete := getArticleIdByCategInXml(idCateg)
  for _, id := range idArticlesToDelete {
    DeleteArticleInXml(id)
  }
}

func insert(original []Article, value Article) []Article {
  target := make([]Article, len(original)+1)
  copy(target, original[:])
  target[len(original)] = value

  return target
}
