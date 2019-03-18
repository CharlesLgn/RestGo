package github

import (
	"bytes"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
)

func GetGithubLanguagePercent(w rest.ResponseWriter, r *rest.Request) {
	name := r.PathParam("nname")
	url := "https://api.github.com/users/"+name+"/repos"
	in := []byte("")
	request, _ := http.NewRequest("GET", url, bytes.NewBuffer(in))
	request.Header.Set("Content-Type", "application/json")
	client := &http.Client{}
	response, err := client.Do(request)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
		http.NotFound(w, r)
		return
	}
	data, _ := ioutil.ReadAll(response.Body)
	var str = string(data)
	log.Println("github")
	res := map[string]string{"res":str}
	w.WriteJson(&res)
}
