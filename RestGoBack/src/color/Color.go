package color

type Color struct {
	RGB   string `json:"rgb"`
	Lum   string `json:"light"`
	Sat   string `json:"saturation"`
	Red   int    `json:"r"`
	Green int    `json:"v"`
	Blue  int    `json:"b"`
	Link  string `json:"link"`
}
