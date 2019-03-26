package webservices

import (
  "encoding/xml"
  "github.com/antchfx/xmlquery"
  "sync"
)

type Categories struct {
  XMLName      xml.Name   `xml:"categories" json:"-"`
  CategoryList []*Categorie `xml:"category,omitempty"  json:"categories"`
}

type Categorie struct {
  XMLName xml.Name `xml:"category" json:"-"`
  ID      int      `xml:"id,attr" json:"id"`
  Libelle string   `xml:"lib" json:"lib"`
}

var lockCategorie = sync.RWMutex{}

func getCategData(path string) string {
  root := getCategoryXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addCateg(categorie Categorie) {
  CreateCategoryInXml(&categorie)
}
