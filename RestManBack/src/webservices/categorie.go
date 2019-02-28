package webservices

import "sync"

type Categorie struct {
	ID      int    `json:"id"`
	Libelle string `json:"lib"`
}

var categories map[int]*Categorie
var lockCategorie = sync.RWMutex{}
var idCategorie = 0

func InitCateg() {
	categories = make(map[int]*Categorie)
	categ1 := Categorie{ID: 1, Libelle: "Food",}
	categ2 := Categorie{ID: 2, Libelle: "Clothe",}
	categ3 := Categorie{ID: 3, Libelle: "Drink",}

	categories[1] = &categ1
	categories[2] = &categ2
	categories[3] = &categ3
	idCategorie = 4
}

func addCateg(categorie Categorie)  {
	lockCategorie.Lock()
	categorie.ID = idCategorie
	idCategorie++
	categories[categorie.ID] = &categorie
	lockCategorie.Unlock()
}