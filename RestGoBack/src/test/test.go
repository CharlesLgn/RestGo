package main

import (
	"RestMan/RestGoBack/src/github"
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
)
func main() {
	api := rest.NewApi()
	api.Use(rest.DefaultDevStack...)

	router, err := rest.MakeRouter(
		//Select
		rest.Get	("/github/:name", github.GetGithubLanguagePercent),
		)

	if err != nil {
		log.Fatal(err)
	}
	api.SetApp(router)
	println("Server start !")
	log.Println("Server start !")
	log.Fatal(http.ListenAndServe(":8000", api.MakeHandler()))
}

