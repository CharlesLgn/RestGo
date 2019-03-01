package crypto

type Message struct {
	Message     string `json:"message"`
}

type Awnser struct {
	OriginalMessage     string `json:"original"`
	NewMessage			string `json:"code"`
	Time			    string `json:"time"`
}

