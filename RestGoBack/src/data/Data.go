package data

type DataIn struct {
	Url       string     `json:"url"`
	Content   string     `json:"content-type"`
	Method 	  string     `json:"method"`
	Lang	  string     `json:"lang"`
	Param 	  string     `json:"param"`
}

type DataOut struct {
	Url     string `json:"url,omitempty"`
	Content string `json:"content-type,omitempty"`
	Status  string `json:"status,omitempty"`
	Data    string `json:"data,omitempty"`
}