package data

type In struct {
	Url       string     `json:"url"`
	Content   string     `json:"content-type"`
	Method 	  string     `json:"method"`
	Lang	  string     `json:"lang"`
	Param 	  string     `json:"param"`
}

type Out struct {
	Url     string `json:"url,omitempty"`
	Content string `json:"content-type,omitempty"`
	Status  string `json:"status,omitempty"`
	Data    string `json:"data,omitempty"`
}