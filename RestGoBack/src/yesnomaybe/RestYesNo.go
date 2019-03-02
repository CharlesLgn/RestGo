package yesnomaybe

import (
	"github.com/ant0ine/go-json-rest/rest"
	"math/rand"
)

func YesNoMaybe(w rest.ResponseWriter, r *rest.Request) {
	x := rand.Intn(100)
	if x%3 == 0 {
		w.WriteJson(map[string]string{"awnser": "Yes !"})
	} else if x%3 == 1 {
		w.WriteJson(map[string]string{"awnser": "No !"})
	} else {
		w.WriteJson(map[string]string{"awnser": "Maybe !"})
	}
}
