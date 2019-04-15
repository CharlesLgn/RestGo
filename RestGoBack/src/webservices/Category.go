package webservices

import (
  "encoding/xml"
  "github.com/antchfx/xmlquery"
  "sync"
)

type Categories struct {
  XMLName      xml.Name   `xml:"categories" json:"-"  yaml:"-"`
  CategoryList []*Categorie `xml:"category,omitempty"  json:"categories"  yaml:"categories"`
}

type Categorie struct {
  XMLName xml.Name `xml:"category" json:"-" yaml:"-"`
  ID      int      `xml:"id,attr" json:"id"  yaml:"id"`
  Libelle string   `xml:"lib" json:"lib"  yaml:"lib"`
}

var lockCategorie = sync.RWMutex{}

func getCategData(path string) string {
  root := getCategoryXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addCateg(categorie Categorie) {
  CreateCategoryInXml(&categorie)
}
