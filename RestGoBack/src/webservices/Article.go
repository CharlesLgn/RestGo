package webservices

import (
  "encoding/xml"
  "github.com/antchfx/xmlquery"
  "sync"
)

type Articles struct {
  XMLName     xml.Name   `xml:"articles" json:"-" yaml:"-"`
  ArticleList []*Article `xml:"article,omitempty"  json:"articles" yaml:"articles"`
}

type Article struct {
  XMLName     xml.Name `xml:"article"  json:"-" yaml:"-"`
  ID          int      `xml:"id,attr"  json:"id,omitempty" yaml:"id,omitempty"`
  Libelle     string   `xml:"lib"      json:"lib,omitempty" yaml:"lib"`
  Prix        float64  `xml:"price"    json:"price,omitempty" yaml:"price"`
  IdCategorie int      `xml:"idCateg"  json:"idCateg,omitempty" yaml:"idCateg"`
}

type ArticlesWithCateg struct {
  XMLName     xml.Name            `xml:"articles" json:"-" yaml:"-"`
  ArticleList []*ArticleWithCateg `xml:"article,omitempty"  json:"articles" yaml:"articles"`
}

type ArticleWithCateg struct {
  XMLName     xml.Name `xml:"article"  json:"-" yaml:"-"`
  ID          int      `xml:"id,attr"  json:"id,omitempty" yaml:"id,omitempty"`
  Libelle     string   `xml:"lib"      json:"lib,omitempty" yaml:"lib"`
  Prix        float64  `xml:"price"    json:"price,omitempty" yaml:"price"`
  Categorie   *Categorie`xml:"category"  json:"category,omitempty" yaml:"category"`
}

var lockArticle = sync.RWMutex{}

func getData(path string) string {
  root := getArticleXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addArticle(article Article) {
  CreateArticleInXml(&article)
}
