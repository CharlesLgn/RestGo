package webservices

import (
	"github.com/antchfx/xmlquery"
	"io/ioutil"
	"os"
	"strconv"
	"strings"
)

func getCategoryXml() *xmlquery.Node {
	pwd, _ := os.Getwd()
	xmlArticle, _ := ioutil.ReadFile(pwd + "/public/data/categories.xml")

	root, err := xmlquery.Parse(strings.NewReader(string(xmlArticle)))
	if err != nil {
		panic(err)
	}
	return root
}

func InitCategoryWithXml() {
	root := getXml()
	articles = make(map[int]*Article)
	idArticle = -1
	xmlquery.FindEach(root, "categories/category", func(i int, node *xmlquery.Node) {
		id, _ := strconv.Atoi(node.Attr[0].Value)
		categoryPath := "categories/category[@id='" + strconv.Itoa(id) + "']"
		lib := getCategData(categoryPath + "/lib")
		categorie := Categorie{
			ID:          id,
			Libelle:     lib,
		}
		categories[id] = &categorie
		if idCategorie < id {
			idCategorie = id
		}
	})
	idCategorie++
}

func CreateCategoryInXml(category *Categorie) {
	articleToAdd := getACategoryAsXmlString(category)
	xmlData := strings.Split(getCategoryXmlAsString(), "</categorie>")[0] + articleToAdd + "</categorie>"
	rewriteCategoryXml(xmlData)
}

func UpdateCategoryInXml(category *Categorie) {
	root := getXml()
	id := strconv.Itoa(category.ID)
	node := xmlquery.FindOne(root, "categories/category[@id='"+id+"']")
	if category.Libelle == "" {
		category.Libelle = node.SelectElement("lib").InnerText()
	}
	articleToAdd := getACategoryAsXmlString(category)
	xmlData := strings.Split(getCategoryXmlAsString(), "</categorie>")[0] + articleToAdd + "</categorie>"
	rewriteCategoryXml(xmlData)
}

func OverwriteCategoryInXml(category *Categorie) {
	articleToOverwrite := getACategoryAsXmlString(category)
	xmlData := getXmlBeforeACategory(getCategoryXmlAsString(), category.ID) + articleToOverwrite + getXmlAfterACategory(getCategoryXmlAsString(), category.ID)
	rewriteCategoryXml(xmlData)
}

func DeleteCategoryInXml(id int) {
	//append in a string
	xmlData := getXmlAsString()
	newXml := getXmlBeforeACategory(xmlData, id) + getXmlAfterACategory(xmlData, id)
	rewriteCategoryXml(newXml)
}

func getCategoryXmlAsString() string {
	xmlData := "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
	root := getCategoryXml()
	xmlquery.FindEach(root, "categories", func(i int, node *xmlquery.Node) {
		xmlData += node.OutputXML(true)
	})
	return xmlData
}

func rewriteCategoryXml(xmlData string) {
	pwd, _ := os.Getwd()
	if err := ioutil.WriteFile(pwd+"/public/data/categories.xml", []byte(xmlData), 7777); err != nil {
		panic(err)
	}
	f, err := os.Create(pwd + "/public/data/categories.xml")
	if err != nil {
		panic(err)
	}
	defer f.Close()
	_, _ = f.WriteString(xmlData)
	_ = f.Sync()
}

func getACategoryAsXmlString(category *Categorie) string {
	//getData
	id := strconv.Itoa(category.ID)
	lib := category.Libelle
	//append in a string
	categorieToAdd := "<categorie id=\"" + id + "\"><lib>" + lib + "</lib></categorie>"
	return categorieToAdd
}

func getXmlBeforeACategory(xmlData string, id int) string {
	Id := strconv.Itoa(id)
	return strings.Split(xmlData, "<categorie id=\""+Id+"\">")[0]
}

func getXmlAfterACategory(xmlData string, id int) string {
	Id := strconv.Itoa(id)
	res := ""
	dataDelete := strings.Split(xmlData, "<categorie id=\""+Id+"\">")
	dataDelete = strings.Split(dataDelete[1], "</categorie>")
	for i := 1; i < len(dataDelete)-1; i++ {
		res += dataDelete[i] + "</categorie>"
	}
	res += dataDelete[len(dataDelete)-1]
	return res
}
