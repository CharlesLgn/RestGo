package main

type Article struct {
	ID        int64      `json:"id"`
	Libelle   string     `json:"lib"`
	Prix  	  float64    `json:"price"`
	Categorie *Categorie `json:"categ"`
}