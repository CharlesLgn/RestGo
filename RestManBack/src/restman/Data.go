package main

type DataIn struct {
	Url       string     `json:"url"`
	Content   string     `json:"content-type,omitempty"`
	Method 	  string     `json:"method"`
	Lang	  string     `json:"lang,omitempty"`
	Param 	  string     `json:"param,omitempty"`
}

type DataOut struct {
	Url       string     `json:"url,omitempty"`
	Content   string     `json:"content-type,omitempty"`
	Http 	  string     `json:"http,omitempty"`
	Data 	  string     `json:"data,omitempty"`
}