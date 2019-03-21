package webservices

import (
	"github.com/antchfx/xmlquery"
	"sync"
)

type Categorie struct {
	ID      int    `json:"id"`
	Libelle string `json:"lib"`
}

var categories map[int]*Categorie
var lockCategorie = sync.RWMutex{}
var idCategorie = 0

func getCategData(path string) string {
	root := getCategoryXml()
	return xmlquery.FindOne(root, path).InnerText()
}

func addCateg(categorie Categorie)  {
	lockCategorie.Lock()
	categorie.ID = idCategorie
	idCategorie++
	categories[categorie.ID] = &categorie
	lockCategorie.Unlock()
}