package crypto

import (
	"github.com/ant0ine/go-json-rest/rest"
	"net/http"
	"time"
)

func TranslteL33t(w rest.ResponseWriter, r *rest.Request) {
	translate(w, r, true)
}

func TranslteMorse(w rest.ResponseWriter, r *rest.Request) {
	translate(w, r, false)
}

func translate(w rest.ResponseWriter, r *rest.Request, isL33t bool)  {
	message := map[string]string{}
	err := r.DecodeJsonPayload(&message)
	if err != nil {
		rest.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	if message["message"] == "" {
		rest.Error(w, "no Message", 400)
		return
	}
	awnser := Awnser{}
	awnser.OriginalMessage = message["message"]
	start := time.Now()
	if isL33t {
		awnser.NewMessage = translateL33t(message["message"])
	} else {
		awnser.NewMessage = translateMorse(message["message"])
	}
	t := time.Now()
	elapsed := t.Sub(start)
	awnser.Time = elapsed.String()
	w.WriteJson(&awnser)
}
