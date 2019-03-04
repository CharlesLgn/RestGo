package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
)

func main() {
	jsonData := map[string]string{"message": "coucou ca va ?"}
	jsonValue, _ := json.Marshal(jsonData)
	request, _ := http.NewRequest("POST", "http://localhost:8000/fun/trad/l33t", bytes.NewBuffer(jsonValue))
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