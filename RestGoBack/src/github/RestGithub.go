package github

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

func GetGithubLanguagePercent(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	params := mux.Vars(r)
	user := params["user"]
	url := "https://api.github.com/users/" + user + "/repos"
	data, err := sendRequestToGitHub(url)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
		return
	}
	var gitUserJsonAsString = string(data)
	total := 0
	nbLinesPerLanguages := map[string]int{}

	fullNames := getFullNames(gitUserJsonAsString)
	for i := 0; i < len(fullNames); i++ {
		languagesOfRepo := getNbLineOfARepo(fullNames[i])
		for language, nbLines := range languagesOfRepo {
			total += nbLines
			nbLinesPerLanguages[language] += nbLines
		}
	}

	var resToSend = map[string]string{}
	for key, value := range nbLinesPerLanguages {
		log.Println("%.2f", Percent(value, total))
		resToSend[key] = fmt.Sprintf("%.2f", Percent(value, total))
	}

	byt, _ := json.Marshal(resToSend)
	var dat map[string]interface{}
	if err := json.Unmarshal(byt, &dat); err != nil {
		panic(err)
	}

	log.Println(total)
	_ = json.NewEncoder(w).Encode(dat)
}

func getNbLineOfARepo(fullName string) map[string]int {
	url := "https://api.github.com/repos/" + fullName + "/languages"
	language, err := sendRequestToGitHub(url)
	if err != nil {
		fmt.Printf("The HTTP request failed with error %s\n", err)
		return nil
	}
	result := map[string]int{}
	_ = json.Unmarshal(language, &result)
	return result
}

func sendRequestToGitHub(url string) ([]byte, error) {
	in := []byte("")
	request, _ := http.NewRequest("GET", url, bytes.NewBuffer(in))
	request.Header.Set("Content-Type", "application/json")
	client := &http.Client{}
	response, err := client.Do(request)
	if err != nil {
		return nil, err
	}
	data, err2 := ioutil.ReadAll(response.Body)
	return data, err2
}

func getFullNames(data string) map[int]string {
	dataToSplit := strings.Split(data, ",\"full_name\":\"")
	var fullName = map[int]string{}

	for i := 1; i < len(dataToSplit) && i-1 < 500; i++ {
		fullName[i-1] = strings.Split(dataToSplit[i], "\"")[0]
	}
	return fullName

}

func Percent(part int, all int) float32 {
	percent := (float32(part) / float32(all)) * 100.
	return percent
}
