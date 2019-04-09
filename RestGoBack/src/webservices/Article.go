package webservices

import (
  "encoding/xml"
  "github.com/antchfx/xmlquery"
  "sync"
)

type Articles struct {
  XMLName     xml.Name   `xml:"articles" json:"-" yaml:"-"`
  ArticleList []*Article `xml:"article,omitempty"  json:"articles" yaml:"article"`
}

type ArticleList []struct{
  XMLName     xml.Name   `xml:"articles" json:"-" yaml:"-"`
  Article     Article    `xml:"article,omitempty" json:"articles" yaml:"article"`
}

type Article struct {
  XMLName     xml.Name `xml:"article"  json:"-" yaml:"-"`
  ID          int      `xml:"id,attr"  json:"id,omitempty" yaml:"id,omitempty"`
  Libelle     string   `xml:"lib"      json:"lib,omitempty" yaml:"id"`
  Prix        float64  `xml:"price"    json:"price,omitempty" yaml:"id"`
  IdCategorie int      `xml:"idCateg"  json:"idCateg,omitempty" yaml:"id"`
}

var lockArticle = sync.RWMutex{}

func getData(path string) string {
  root := getArticleXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addArticle(article Article) {
  CreateArticleInXml(&article)
}
