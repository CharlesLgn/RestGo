package main

import (
  "RestMan/RestGoBack/src/color"
  "RestMan/RestGoBack/src/crypto"
  "RestMan/RestGoBack/src/data"
  "RestMan/RestGoBack/src/github"
  "RestMan/RestGoBack/src/lissajous"
  "RestMan/RestGoBack/src/webservices"
  "RestMan/RestGoBack/src/wikipedia"
  "RestMan/RestGoBack/src/yesnomaybe"

  "github.com/gorilla/mux"

  "log"
  "net/http"
)

// main-gui function
func main() {
  get8000()
}

func get8000() {
  log.Println("RestMan is starting")
  router := mux.NewRouter()

  //****************************
  // Article
  //****************************
  router.HandleFunc("/articles", webservices.GetArticles).Methods("GET")
  router.HandleFunc("/article/{id}", webservices.GetArticleById).Methods("GET")
  router.HandleFunc("/article/categorie/{id}", webservices.GetArticleByCateg).Methods("GET")

  //Create
  router.HandleFunc("/article", webservices.CreateArticle).Methods("POST")
  //Delete
  router.HandleFunc("/article/{id}", webservices.DeleteArticle).Methods("DELETE")
  router.HandleFunc("/article/categorie/{id}", webservices.DeleteArticlesByCateg).Methods("DELETE")

  //Override
  router.HandleFunc("/article/{id}", webservices.OverrideArticle).Methods("PUT")
  //Update
  router.HandleFunc("/article/{id}", webservices.UpdateArticle).Methods("PATCH")
  router.HandleFunc("/article/{id}", webservices.UpdateArticle).Methods("POST")

  //****************************
  // Category
  //****************************
  router.HandleFunc("/categories", webservices.GetCategories).Methods("GET")
  router.HandleFunc("/categorie/{id}", webservices.GetCategoryById).Methods("GET")

  //Create
  router.HandleFunc("/categorie", webservices.CreateCategory).Methods("POST")

  //Delete
  router.HandleFunc("/categorie/{id}", webservices.DeleteCategory).Methods("DELETE")
  //Override
  router.HandleFunc("/categorie/{id}", webservices.OverrideCategory).Methods("PUT")
  //Update
  router.HandleFunc("/categorie/{id}", webservices.UpdateCategory).Methods("PATCH")
  router.HandleFunc("/categorie/{id}", webservices.UpdateCategory).Methods("POST")

  //****************************
  // fun
  //****************************
  router.HandleFunc("/fun/data", data.GetData).Methods("POST")
  router.HandleFunc("/fun/color", color.RandomColor).Methods("GET")
  router.HandleFunc("/fun/yes", yesnomaybe.YesNoMaybe).Methods("GET")
  router.HandleFunc("/fun/wiki/{title}", wikipedia.GetPage).Methods("GET")
  router.HandleFunc("/fun/github/{user}", github.GetGithubLanguagePercent).Methods("GET")
  router.HandleFunc("/fun/trad/l33t", crypto.TranslteL33t).Methods("POST")
  router.HandleFunc("/fun/trad/morse", crypto.TranslteMorse).Methods("POST")
  //****************************
  // Graphique
  //****************************
  router.HandleFunc("/fun/lissa", lissajous.LissajousWeb).Methods("GET")

  log.Println("Server start !")
  log.Fatal(http.ListenAndServe(":8000", router))
}
