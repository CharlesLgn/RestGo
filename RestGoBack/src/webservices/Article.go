package webservices

import (
  "encoding/xml"
  "github.com/antchfx/xmlquery"
  "sync"
)

type Articles struct {
  XMLName     xml.Name   `xml:"articles" json:"-"`
  ArticleList []*Article `xml:"article,omitempty"  json:"articles"`
}

type Article struct {
  XMLName     xml.Name `xml:"article"  json:"-"`
  ID          int      `xml:"id,attr" json:"id,omitempty"`
  Libelle     string   `xml:"lib"      json:"lib,omitempty"`
  Prix        float64  `xml:"price"    json:"price,omitempty"`
  IdCategorie int      `xml:"idCateg"  json:"idCateg,omitempty"`
}

var lockArticle = sync.RWMutex{}

func getData(path string) string {
  root := getArticleXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addArticle(article Article) {
  CreateArticleInXml(&article)
}
