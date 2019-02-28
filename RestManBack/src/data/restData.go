package data

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/ant0ine/go-json-rest/rest"
	"io/ioutil"
	"net/http"
)

func GetData(w rest.ResponseWriter, r *rest.Request) {
	dataIn := DataIn{}
	r.DecodeJsonPayload(&dataIn)

	if dataIn.Url == "" {
		rest.Error(w, "url required", 400)
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
	request.Header.Set("Content-Type", dataIn.Method)
	client := &http.Client{}
	response, err := client.Do(request)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
		rest.NotFound(w, r)
		return
	} else {
		dataOut := DataOut{}
		data, _ := ioutil.ReadAll(response.Body)
		dataOut.Data = string(data)
		dataOut.Url = dataIn.Url
		dataOut.Status = response.Status
		dataOut.Content = response.Proto
		fmt.Println("data send")
		w.WriteJson(dataOut)
	}
}
