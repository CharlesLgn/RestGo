package main

import (
	"github.com/CharlesLgn/RestMan/RestManBack/src/color"
	"github.com/CharlesLgn/RestMan/RestManBack/src/crypto"
	"github.com/CharlesLgn/RestMan/RestManBack/src/data"
	"github.com/CharlesLgn/RestMan/RestManBack/src/webservices"
	"github.com/CharlesLgn/RestMan/RestManBack/src/wikipedia"
	"github.com/CharlesLgn/RestMan/RestManBack/src/yesnomaybe"
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
)

func main() {
	log.Println("RestMan is starting")
	webservices.InitArticle()
	webservices.InitCateg()
	api := rest.NewApi()
	api.Use(rest.DefaultDevStack...)
	router, err := rest.MakeRouter(
	//Select
		rest.Get	("/articles", 				webservices.GetArticles),
		rest.Get	("/article/:id", 			webservices.GetArticle),
		rest.Get	("/article/categorie/:id", 	webservices.GetArticleByCateg),
		rest.Get	("/categories", 			webservices.GetCategogies),
		rest.Get	("/categorie/:id", 			webservices.GetCategogie),
	//Create
		rest.Post	("/article", 				webservices.CreateArticle),
		rest.Post	("/categorie", 				webservices.CreateCategogie),
	//Update
		rest.Put	("/article/:id", 			webservices.UpdateArticle),
		rest.Patch	("/article/:id", 			webservices.PatchArticle),
		rest.Post	("/article/:id", 			webservices.UpdateArticle),
		rest.Put	("/categorie/:id", 			webservices.UpdateCategogie),
		rest.Patch	("/categorie/:id", 			webservices.PatchCategogie),
		rest.Post	("/categorie/:id", 			webservices.UpdateCategogie),
	//Delete
		rest.Delete	("/article/:id", 			webservices.DeleteArticle),
		rest.Delete	("/categorie/:id", 			webservices.DeleteCategogie),
		rest.Delete	("/article/categorie/:id", 	webservices.DeleteArticlesByCateg),

	//Fun
		rest.Post	("/fun/Data", 				data.GetData),
		rest.Get	("/fun/wiki/:title", 		wikipedia.GetPage),
		rest.Get	("/fun/yes", 				yesnomaybe.YesNoMaybe),
		rest.Get	("/fun/color", 				color.RandomColor),
		rest.Post	("/fun/trad/l33t", 			crypto.TranslteL33t),
		rest.Post	("/fun/trad/morse", 		crypto.TranslteMorse),
	)
	if err != nil {
		log.Fatal(err)
	}
	api.SetApp(router)
	log.Println("Server start !")
	log.Fatal(http.ListenAndServe(":8000", api.MakeHandler()))
}
