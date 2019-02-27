package main

import "sync"

type Article struct {
	ID        int      `json:"id	"`
	Libelle   string     `json:"lib"`
	Prix  	  float64    `json:"price"`
	IdCategorie int `json:"idCateg"`
}

var articles map[int]*Article
var lockArticle = sync.RWMutex{}
var idArticle = 0

func InitArticle() {
	articles = make(map[int]*Article)
	article1 := Article{ID: 1, Libelle: "Cookies", Prix: 2.99, IdCategorie: 1,}
	article2 := Article{ID: 2, Libelle: "Brownies", Prix: 1.99, IdCategorie: 1,}
	article3 := Article{ID: 3, Libelle: "Cake", Prix: 4.99, IdCategorie: 1,}
	article4 := Article{ID: 4, Libelle: "Pull", Prix: 4.99, IdCategorie: 2,}

	articles[1] = &article1
	articles[2] = &article2
	articles[3] = &article3
	articles[4] = &article4
	idArticle = 5
}

func addArticle(article Article)  {
	lockArticle.Lock()
	article.ID = idArticle
	idArticle += 1
	articles[article.ID] = &article
	lockArticle.Unlock()
}