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

func setHeader(w http.ResponseWriter)  {
  w.Header().Set("X-Powered-by", "Okya Corp")
}

func GetArticleByCateg(w http.ResponseWriter, r *http.Request) {
  setHeader(w)
  params := mux.Vars(r)
  idCateg, _ := strconv.Atoi(params["id"])
  log.Println("get  article by categ: ", idCateg)
  lockArticle.RLock()

  stock := make([]*Article, 0)
  articles := getAllArticleInXml()

  for _, article := range articles {
    if article.IdCategorie == idCateg {
      stock = insert(stock, article)
    }
  }

  contentType := r.Header.Get("Content-Type")
  if strings.Contains(contentType, "xml") {
    w.Header().Set("Content-Type", "application/xml; charset=utf-8")
    articlesXml := Articles{}
    articlesXml.ArticleList = stock
    _ = xml.NewEncoder(w).Encode(articlesXml)
  } else {
    w.Header().Set("Content-Type", "application/json; charset=utf-8")
    _ = json.NewEncoder(w).Encode(stock)
  }
}

func DeleteArticlesByCateg(w http.ResponseWriter, r *http.Request) {
  setHeader(w)
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

func insert(original []*Article, value *Article) []*Article {
  target := make([]*Article, len(original)+1)
  copy(target, original[:])
  target[len(original)] = value

  return target
}
