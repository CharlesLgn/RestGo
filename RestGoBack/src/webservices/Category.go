package webservices

import (
	"github.com/antchfx/xmlquery"
	"sync"
)

type Categorie struct {
	ID      int    `json:"id"`
	Libelle string `json:"lib"`
}

var lockCategorie = sync.RWMutex{}

func getCategData(path string) string {
	root := getCategoryXml()
	return xmlquery.FindOne(root, path).InnerText()
}

func addCateg(categorie Categorie)  {
	CreateCategoryInXml(&categorie)
}