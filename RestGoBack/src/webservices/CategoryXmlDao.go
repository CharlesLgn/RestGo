package webservices

import (
	"github.com/antchfx/xmlquery"
	"io/ioutil"
	"log"
	"os"
	"strconv"
	"strings"
)

func getCategoryXml() *xmlquery.Node {
	pwd, _ := os.Getwd()
	xmlCategory, _ := ioutil.ReadFile(pwd + "/public/data/categories.xml")

	root, err := xmlquery.Parse(strings.NewReader(string(xmlCategory)))
	if err != nil {
		panic(err)
	}
	return root
}

func getNewIdForCategoryXml() int {
	root := getCategoryXml()
	idCategory := -1
	xmlquery.FindEach(root, "categories/category", func(i int, node *xmlquery.Node) {
		id, _ := strconv.Atoi(node.Attr[0].Value)
		log.Println(id)
		if idCategory < id {
			idCategory = id
		}
	})
	return idCategory+1
}

func getAllCategoryWithXml() map[int]*Categorie {
	root := getCategoryXml()
	categories := make(map[int]*Categorie)
	xmlquery.FindEach(root, "categories/category", func(i int, node *xmlquery.Node) {
		id, _ := strconv.Atoi(node.Attr[0].Value)
		categoryPath := "categories/category[@id='" + strconv.Itoa(id) + "']"
		lib := getCategData(categoryPath + "/lib")
		categorie := Categorie{
			ID:          id,
			Libelle:     lib,
		}
		categories[id] = &categorie
	})
	return categories
}

func CreateCategoryInXml(category *Categorie) {
	category.ID = getNewIdForCategoryXml()
	categoryToAdd := getACategoryAsXmlString(category)
	xmlData := strings.Split(getCategoryXmlAsString(), "</categories>")[0] + categoryToAdd + "</categories>"
	rewriteCategoryXml(xmlData)
}

func UpdateCategoryInXml(category *Categorie) {
	root := getArticleXml()
	id := strconv.Itoa(category.ID)
	node := xmlquery.FindOne(root, "categories/category[@id='"+id+"']")
	if category.Libelle == "" {
		category.Libelle = node.SelectElement("lib").InnerText()
	}
	articleToAdd := getACategoryAsXmlString(category)
	xmlData := getXmlBeforeACategory(getCategoryXmlAsString(), category.ID) + articleToAdd + getXmlAfterACategory(getCategoryXmlAsString(), category.ID)
	rewriteCategoryXml(xmlData)
}

func OverwriteCategoryInXml(category *Categorie) {
	UpdateCategoryInXml(category)
}

func DeleteCategoryInXml(id int) {
	//append in a string
	xmlData := getCategoryXmlAsString()
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
	lockCategorie.Lock()
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
	lockCategorie.Unlock()
}

func getACategoryAsXmlString(category *Categorie) string {
	//getData
	id := strconv.Itoa(category.ID)
	lib := category.Libelle
	//append in a string
	categorieToAdd := "<category id=\"" + id + "\"><lib>" + lib + "</lib></category>"
	return categorieToAdd
}

func getXmlBeforeACategory(xmlData string, id int) string {
	Id := strconv.Itoa(id)
	return strings.Split(xmlData, "<category id=\""+Id+"\">")[0]
}

func getXmlAfterACategory(xmlData string, id int) string {
	Id := strconv.Itoa(id)
	res := ""
	dataDelete := strings.Split(xmlData, "<category id=\""+Id+"\">")
	dataDelete = strings.Split(dataDelete[1], "</category>")
	for i := 1; i < len(dataDelete)-1; i++ {
		res += dataDelete[i] + "</category>"
	}
	res += dataDelete[len(dataDelete)-1]
	return res
}
