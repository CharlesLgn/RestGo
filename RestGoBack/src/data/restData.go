package data

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
)

func GetData(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	var dataIn In
	err := json.NewDecoder(r.Body).Decode(&dataIn)
	if err != nil {
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	if dataIn.Url == "" {
		http.Error(w, "url required", 400)
		return
	}
	if dataIn.Method == "" {
		dataIn.Method = "GET"
	}
	if dataIn.Content == "" {
		dataIn.Content = "application/json"
	}

	in := []byte(dataIn.Param)
	var raw map[string]interface{}
	_ = json.Unmarshal(in, &raw)

	request, _ := http.NewRequest(dataIn.Method, dataIn.Url, bytes.NewBuffer(in))
	request.Header.Set("Content-Type", dataIn.Content)
	client := &http.Client{}
	response, err := client.Do(request)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
		http.NotFound(w, r)
		return
	}
	dataOut := Out{}
	data, _ := ioutil.ReadAll(response.Body)

	var head = ""
	for k, v := range response.Header {
		head += k + " = "
		head += v[0]
		for i := 1 ; i > len(v) ; i++ {
			head += "; " + v[i]
		}
		head += "\n"
		head += "\n"
	}

	var str = string(data)
	log.Println(str)
	dataOut.Data = str
	dataOut.Url = dataIn.Url
	dataOut.Status = response.Status
	dataOut.Content = response.Proto
	fmt.Println("data send")
	_ = json.NewEncoder(w).Encode(dataOut)
}
