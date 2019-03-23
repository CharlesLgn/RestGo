package wikipedia

import (
	"github.com/gorilla/mux"

	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"strings"
)

func GetPage(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	params := mux.Vars(r)
	title := params["title"]
	log.Println("get article :", title)

	in := []byte("")
	var raw map[string]interface{}
	_ = json.Unmarshal(in, &raw)
	var url = "https://en.wikipedia.org/api/rest_v1/page/summary/" + title
	request, _ := http.NewRequest("GET", url, bytes.NewBuffer(in))
	request.Header.Set("Content-Type", "application/json")
	client := &http.Client{}
	response, err := client.Do(request)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
		http.NotFound(w, r)
		return
	}
	page := Page{}
	data, _ := ioutil.ReadAll(response.Body)
	var str = string(data)
	page.Title = strings.Split(strings.Split(str, ",\"title\":\"")[1], "\",\"")[0]
	page.Image = strings.Split(strings.Split(str, "\"originalimage\":{\"source\":\"")[1], "\",\"")[0]
	page.Lang = strings.Split(strings.Split(str, ",\"lang\":\"")[1], "\",\"")[0]
	page.Url = strings.Split(strings.Split(str, "{\"page\":\"")[1], "\",\"")[0]
	page.Summary = strings.Split(strings.Split(str, "},\"extract\":\"")[1], "\",\"")[0]

	_ = json.NewEncoder(w).Encode(page)
}