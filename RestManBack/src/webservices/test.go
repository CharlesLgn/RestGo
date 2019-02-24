package main

import (
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
)

func main() {
	log.Println("RestMan is starting")
	InitArticle()
	InitCateg()
	api := rest.NewApi()
	api.Use(rest.DefaultDevStack...)
	router, err := rest.MakeRouter(
		rest.Get("/articles", GetArticles),
		rest.Get("/article/:id", GetArticle),
		rest.Post("/article", CreateArticle),
		rest.Delete("/article/:id", DeleteArticle),
		rest.Get("/categories", GetCategogies),
		rest.Get("/categorie/:id", GetCategogie),
		rest.Post("/categorie", CreateCategogie),
		rest.Delete("/categorie/:id", DeleteCategogie),
	)
	if err != nil {
		log.Fatal(err)
	}
	api.SetApp(router)
	log.Println("Server start !")
	log.Fatal(http.ListenAndServe(":8000", api.MakeHandler()))
}
