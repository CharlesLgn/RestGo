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
	message := Message{}
	err := r.DecodeJsonPayload(&message)
	if err != nil {
		rest.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}
	if message.Message == "" {
		rest.Error(w, "no Message", 400)
		return
	}
	awnser := Awnser{}
	awnser.OriginalMessage = message.Message
	start := time.Now()
	if isL33t {
		awnser.NewMessage = translateL33t(message.Message)
	} else {
		awnser.NewMessage = translateMorse(message.Message)
	}
	t := time.Now()
	elapsed := t.Sub(start)
	awnser.Time = elapsed.String()
	w.WriteJson(&awnser)
}
