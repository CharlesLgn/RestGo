package main

type Article struct {
	ID      int64   	 `json:"id,omitempty"`
	Libelle string   `json:"lib,omitempty"`
	prix  	float64  `json:"price,omitempty"`
}