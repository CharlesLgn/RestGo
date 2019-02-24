package main

type Article struct {
	ID        int      `json:"id"`
	Libelle   string     `json:"lib"`
	Prix  	  float64    `json:"price"`
	IdCategorie int `json:"idCateg"`
}