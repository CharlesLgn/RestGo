package data

import (
	"fmt"
	"github.com/ant0ine/go-json-rest/rest"
	"log"
	"net/http"
)

func main() {
	fmt.Println("RestManFront started...")
	api := rest.NewApi()
	api.Use(rest.DefaultDevStack...)
	router, err := rest.MakeRouter(rest.Post("/Data", GetData))
	if err != nil {
		log.Fatal(err)
	}
	api.SetApp(router)
	fmt.Println("...Server start")
	log.Fatal(http.ListenAndServe(":8001", api.MakeHandler()))

}
