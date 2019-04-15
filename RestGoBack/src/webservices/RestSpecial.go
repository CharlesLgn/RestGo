package webservices

import (
  "encoding/json"
  "encoding/xml"
  "log"
  "net/http"
  "strconv"
  "strings"

  "github.com/CharlesLgn/yamlGoParser/yaml"
  "github.com/gorilla/mux"
)

func setHeader(w http.ResponseWriter, r *http.Request) {
  w.Header().Set("Content-Type", getContentType(r))
  w.Header().Set("X-Powered-by", "Okya Corp")
  lang := r.Header.Get("Accept-Language")
  w.Header().Set("Accept-Language", lang)
  if strings.Contains(lang, "fr") || strings.Contains(lang, "FR") {
    w.Header().Set("X-Custom", "Salut mon pote")
  } else {
    w.Header().Set("X-Custom", "Good Morning Sir")
  }
}

func GetArticleByCateg(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
  display := isDisplayCateg(w, r)
  if display {
    GetArticleByCategWithDisplay(w, r)
  } else {
    params := mux.Vars(r)
    idCateg, _ := strconv.Atoi(params["id"])
    lockArticle.RLock()

    stock := make([]*Article, 0)
    articles := getAllArticleInXml()

    for _, article := range articles {
      if article.IdCategorie == idCateg {
        stock = insert(stock, article)
      }
    }

    articlesXml := Articles{}
    articlesXml.ArticleList = stock
    encode(w, r, articlesXml, stock, "get article by categ:"+strconv.Itoa(idCateg))
  }
}

func GetArticleByCategWithDisplay(w http.ResponseWriter, r *http.Request) {
  params := mux.Vars(r)
  idCateg, _ := strconv.Atoi(params["id"])
  lockArticle.RLock()

  stock := make([]*ArticleWithCateg, 0)
  articles := getAllArticleInXml()

  for _, article := range articles {
    if article.IdCategorie == idCateg {
      articleWC := toArticleWithCateg(article)
      stock = insertAWC(stock, &articleWC)
    }
  }
  articlesXml := ArticlesWithCateg{}
  articlesXml.ArticleList = stock

  encode(w, r, articlesXml, stock, "get article by categ:"+strconv.Itoa(idCateg))
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

func insertAWC(original []*ArticleWithCateg, value *ArticleWithCateg) []*ArticleWithCateg {
  target := make([]*ArticleWithCateg, len(original)+1)
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

func toArticleWithCateg(article *Article) ArticleWithCateg {
  categories := getAllCategoryWithXml()
  var categ *Categorie
  for _, category := range categories {
    if article.IdCategorie == category.ID {
      categ = category
    }
  }
  return ArticleWithCateg{
    ID:        article.ID,
    Libelle:   article.Libelle,
    Prix:      article.Prix,
    Categorie: categ,
  }
}

func encode(w http.ResponseWriter, r *http.Request, caseXML interface{}, otherCase interface{}, message string) {
  if isResInXML(r) {
    log.Println("[XML]", message)
    _ = xml.NewEncoder(w).Encode(caseXML)
  } else if isResInYaml(r) {
    log.Println("[YAML]", message)
    _ = yaml.NewEncoder(w).Encode(otherCase)
  } else {
    log.Println("[JSON]", message)
    _ = json.NewEncoder(w).Encode(otherCase)
  }
}
