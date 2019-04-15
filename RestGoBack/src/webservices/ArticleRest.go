package webservices

import (

  "encoding/json"
  "log"
  "net/http"
  "strconv"

  "github.com/gorilla/mux"

)

func isDisplayCateg(w http.ResponseWriter, r *http.Request) bool {
  b := r.Header.Get("X-display-categ") == "true"
  if b {
    w.Header().Set("X-display-categ", "true")
  } else {
    w.Header().Set("X-display-categ", "false")
  }
  return b
}

func GetArticles(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
  id, err := strconv.Atoi(r.Header.Get("X-Article-id"))
  if err == nil {
    getArticleById(w, r, id)
  } else {
    var articleList interface{}
    var articles interface{}
    displayCateg := isDisplayCateg(w,r)
    if displayCateg {
      var articles ArticlesWithCateg
      articleList = getAllArticleWhithCategInXml()
      articles.ArticleList = getMapArticleWithCategAsArray(articleList.(map[int]*ArticleWithCateg))
    } else {
      var articles Articles
      articleList = getAllArticleInXml()
      articles.ArticleList = getMapArticleAsArray(articleList.(map[int]*Article))
    }
    encode(w, r, articles,articleList, "get  all articles")
  }
}

func getArticleById(w http.ResponseWriter, r *http.Request, id int) {
  display := isDisplayCateg(w, r)
  articles := getAllArticleInXml()
  var dataToSend *Article
  for _, article := range articles {
    if article.ID == id {
      dataToSend = article
    }
  }
  if dataToSend == nil {
    http.Error(w, "no article with this id", http.StatusNotFound)
  } else {
    var data interface{}
    if display {
      data = toArticleWithCateg(dataToSend)
    } else {
      data = dataToSend
    }
    encode(w, r, data, data, "get article "+strconv.Itoa(id))
  }
}


func GetArticleById(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
  params := mux.Vars(r)
  log.Println("get one article")
  id, _ := strconv.Atoi(params["id"])
  getArticleById(w, r, id)
}

func CreateArticle(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
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
  DeleteArticleInXml(id)
  w.WriteHeader(http.StatusOK)
}

func UpdateArticle(w http.ResponseWriter, r *http.Request) {
  setHeader(w, r)
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
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
  setHeader(w, r)
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
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
