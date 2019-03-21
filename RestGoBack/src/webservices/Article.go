package webservices

import (
	"github.com/antchfx/xmlquery"
	"sync"
)

type Article struct {
	ID          int     `json:"id"`
	Libelle     string  `json:"lib"`
	Prix        float64 `json:"price"`
	IdCategorie int     `json:"idCateg"`
}

var articles map[int]*Article
var lockArticle = sync.RWMutex{}
var idArticle = 0



func getData(path string) string {
	root := getXml()
	return xmlquery.FindOne(root, path).InnerText()
}

func addArticle(article Article) {
	lockArticle.Lock()
	article.ID = idArticle
	idArticle++
	articles[article.ID] = &article
	CreateArticleInXml(&article)
	lockArticle.Unlock()
}
