package wikipedia

type Page struct {
	Url     string `json:"url"`
	Title   string `json:"title"`
	Image   string `json:"picture"`
	Lang    string `json:"lang"`
	Summary string `json:"resume"`
}
