package webservices

import (
  "encoding/json"
  "encoding/xml"
  "log"
  "net/http"
  "strconv"
  "strings"

  "github.com/gorilla/mux"

  "gopkg.in/yaml.v2"
)

func setHeader(w http.ResponseWriter, r *http.Request) {
  w.Header().Set("X-Powered-by", "Okya Corp")
  lang := r.Header.Get("Accept-Language")
  if strings.Contains(lang, "fr") || strings.Contains(lang, "FR") {
    w.Header().Set("X-Custom", "Salut mon pote")
  } else {
    w.Header().Set("X-Custom", "Good Morning Sir")
  }
}

func GetArticleByCateg(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
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

  w.Header().Set("Content-Type", getContentType(r))
  if isResInXML(r) {
    articlesXml := Articles{}
    articlesXml.ArticleList = stock
    _ = xml.NewEncoder(w).Encode(articlesXml)
  } else if isResInYaml(r) {
    _ = yaml.NewEncoder(w).Encode(stock)
  } else {
    _ = json.NewEncoder(w).Encode(stock)
  }
}

func DeleteArticlesByCateg(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
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

func isResInThis(r *http.Request, this string) bool {
  contentType := r.Header.Get("Content-Type")
  accept := r.Header.Get("Accept")
  if strings.Contains(contentType, this) || strings.Contains(accept, this) {
    return true
  } else {
    return false
  }
}

func isResInXML(r *http.Request) bool {
  return isResInThis(r, "xml")
}

func isResInYaml(r *http.Request) bool {
  return isResInThis(r, "yaml")
}

func getContentType(r *http.Request) string {
  if isResInXML(r) {
    return "application/xml; charset=utf-8"
  } else if isResInYaml(r) {
    return "application/x-yaml; charset=utf-8"
  } else {
    return "application/json; charset=utf-8"
  }
}
