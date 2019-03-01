package yesnomaybe

import (
	"github.com/ant0ine/go-json-rest/rest"
	"math/rand"
)

func YesNoMaybe(w rest.ResponseWriter, r *rest.Request) {
	x := rand.Intn(100)
	awnser := Awnser{}
	if x%3 == 0 {
		awnser.Awnser = "Yes !"
	} else if x%3 == 1 {
		awnser.Awnser = "No !"
	} else {
		awnser.Awnser = "Maybe !"
	}
	w.WriteJson(awnser)
}
