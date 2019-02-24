package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/ant0ine/go-json-rest/rest"
	"io/ioutil"
	"log"
	"net/http"
)

func GetData(w rest.ResponseWriter, r *rest.Request) {
	dataIn := DataIn{}
	//var dataOut DataOut
	r.DecodeJsonPayload(&dataIn)

	/*err := json.Unmarshal([]byte(s), data)
	log.Println(err)
	fmt.Println(data.Votes)
	s2, _ := json.Marshal(data)
	fmt.Println(string(s2))*/


	w.WriteJson(&dataIn)

	if dataIn.Url == "" {
		rest.Error(w, "url required", 400)
		return
	}
	if dataIn.Method == "" {
		dataIn.Method = "GET"
	}

	w.WriteJson(&dataIn)


	log.Println(dataIn.Method)
	log.Println(dataIn.Url)

	jsonData := map[string]string{"url": "http://localhost:8000/categories",  "method": "GET"}
	jsonValue, _ := json.Marshal(jsonData)
	request, _ := http.NewRequest("POST", "http://localhost:8001/Data", bytes.NewBuffer(jsonValue))
	//request, _ := http.NewRequest("DELETE", "http://localhost:8000/categorie/4", bytes.NewBuffer(jsonValue))
	request.Header.Set("Content-Type", "application/json")
	client := &http.Client{}
	response, err := client.Do(request)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
	} else {
		data, _ := ioutil.ReadAll(response.Body)
		fmt.Println(string(data))
	}
}
