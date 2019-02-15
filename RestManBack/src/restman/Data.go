package main

type DataIn struct {
	url       string     `json:"url"`
	content   string     `json:"content-type"`
	method 	  string     `json:"method"`
	lang	  string     `json:"lang"`
	param 	  string     `json:"param"`
}

type DataOut struct {
	url       string     `json:"url,omitempty"`
	content   string     `json:"content-type,omitempty"`
	http 	  string     `json:"http,omitempty"`
	data 	  string     `json:"data,omitempty"`
}