package webservices

import (
  "github.com/antchfx/xmlquery"
  "sync"
)

type Article struct {
  ID          int      `json:"id,omitempty"`
  Libelle     string   `json:"lib,omitempty"`
  Prix        float64  `json:"price,omitempty"`
  IdCategorie int      `json:"idCateg,omitempty"`
}

var lockArticle = sync.RWMutex{}

func getData(path string) string {
  root := getArticleXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addArticle(article Article) {
  CreateArticleInXml(&article)
}
