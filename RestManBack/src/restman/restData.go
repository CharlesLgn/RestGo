package main

import (
	"fmt"
	"github.com/ant0ine/go-json-rest/rest"
	"io/ioutil"
	"log"
	"net/http"
)

func GetData(w rest.ResponseWriter, r *rest.Request) {
	var dataIn DataIn
	//var dataOut DataOut

	dataIn.url    = r.FormValue("url")
	dataIn.method = r.FormValue("method")
	w.WriteJson(&dataIn)

	if dataIn.url == "" {
		rest.Error(w, "url required", 400)
		return
	}
	if dataIn.method == "" {
		rest.Error(w, "method required", 400)
		return
	}
	w.WriteJson(&dataIn)


	log.Println(dataIn.method)
	log.Println(dataIn.url)

	if dataIn.method == "GET" {
		fmt.Println("Starting the application...")
		response, err := http.Get(dataIn.method)
		if err != nil {
			fmt.Printf("The HTTP request failed with error %s\n", err)
		} else {
			data, _ := ioutil.ReadAll(response.Body)
			fmt.Println(string(data))
		}
	} else if dataIn.method == "POST" {
		/*jsonValue, _ := json.Marshal(dataIn)
		response, err := http.Post("http://localhost:8000/article/4", "application/json", bytes.NewBuffer(jsonValue))
		if err != nil {
			fmt.Printf("The HTTP request failed with error %s\n", err)
		} else {
			data, _ := ioutil.ReadAll(response.Body)
			fmt.Println(string(data))
		}*/
	} else if dataIn.method == "Delete" {}
}
