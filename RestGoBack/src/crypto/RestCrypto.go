package crypto

import (
  "encoding/json"
  "net/http"
  "time"
)

func TranslteL33t(w http.ResponseWriter, r *http.Request) {
  translate(w, r, true)
}

func TranslteMorse(w http.ResponseWriter, r *http.Request) {
  translate(w, r, false)
}

func translate(w http.ResponseWriter, r *http.Request, isL33t bool) {
  w.Header().Set("Content-Type", "application/json; charset=utf-8")
  message := map[string]string{}
  err := json.NewDecoder(r.Body).Decode(&message)
  if err != nil {
    http.Error(w, err.Error(), http.StatusInternalServerError)
    return
  }
  if message["message"] == "" {
    http.Error(w, "no Message", 400)
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
  _ = json.NewEncoder(w).Encode(awnser)
}
