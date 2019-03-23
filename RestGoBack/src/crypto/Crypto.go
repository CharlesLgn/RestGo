package crypto

type Awnser struct {
  OriginalMessage string `xml:"awnser>original" json:"original"`
  NewMessage      string `xml:"awnser>code"     json:"code"`
  Time            string `xml:"awnser>time"     json:"time" `
}
