package yesnomaybe

import (
	"encoding/json"
	"math/rand"
	"net/http"
)

func YesNoMaybe(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	x := rand.Intn(100)
	if x%3 == 0 {
		_ = json.NewEncoder(w).Encode(map[string]string{"awnser": "Yes !"})
	} else if x%3 == 1 {
		_ = json.NewEncoder(w).Encode(map[string]string{"awnser": "No !"})
	} else {
		_ = json.NewEncoder(w).Encode(map[string]string{"awnser": "Maybe !"})
	}
}
