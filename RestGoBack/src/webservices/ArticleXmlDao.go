package webservices

import (
	"fmt"
	"github.com/antchfx/xmlquery"
	"io/ioutil"
	"os"
	"strconv"
	"strings"
)

func getArticleXml() *xmlquery.Node {
	pwd, _ := os.Getwd()
	xmlArticle, _ := ioutil.ReadFile(pwd + "/public/data/articles.xml")

	root, err := xmlquery.Parse(strings.NewReader(string(xmlArticle)))
	if err != nil {
		panic(err)
	}
	return root
}

func getNewIdForArticleXml() int {
	root := getArticleXml()
	idArticle := -1
	xmlquery.FindEach(root, "articles/article", func(i int, node *xmlquery.Node) {
		id, _ := strconv.Atoi(node.Attr[0].Value)
		if idArticle < id {
			idArticle = id
		}
	})
	return idArticle+1
}

func getArticleIdByCategInXml(idCateg int) map[int]int  {
	root := getArticleXml()
	articles := make(map[int]int)
	xmlquery.FindEach(root, "articles/article[idCateg="+ strconv.Itoa(idCateg) +"]", func(i int, node *xmlquery.Node) {
		id, _ := strconv.Atoi(node.Attr[0].Value)
		articles[id] = id
	})
	return articles
}

func getMapArticleAsArray(data map[int]*Article) []*Article {
	const size = 500
	articles := make([]*Article, size)
	for k,v := range data {
		articles[k] = v
	}
	return articles
}

func getAllArticleInXml() map[int]*Article {
	root := getArticleXml()
	articles := make(map[int]*Article)
	xmlquery.FindEach(root, "articles/article", func(i int, node *xmlquery.Node) {
		id, _ := strconv.Atoi(node.Attr[0].Value)
		articlePath := "articles/article[@id='" + strconv.Itoa(id) + "']"
		lib := getData(articlePath + "/lib")
		price, _ := strconv.ParseFloat(getData(articlePath+"/price"), 64)
		idCateg, _ := strconv.Atoi(getData(articlePath + "/idCateg"))
		article := Article{
			ID:          id,
			Libelle:     lib,
			Prix:        price,
			IdCategorie: idCateg,
		}
		articles[article.ID] = &article
	})
	return articles
}

func CreateArticleInXml(article *Article) {
	article.ID = getNewIdForArticleXml()
	articleToAdd := getAnArticleAsXmlString(article)
	xmlData := strings.Split(getArticleXmlAsString(), "</articles>")[0] + articleToAdd + "</articles>"
	rewriteXml(xmlData)
}

func UpdateArticleInXml(article *Article) {
	root := getArticleXml()
	id := strconv.Itoa(article.ID)
	node := xmlquery.FindOne(root, "articles/article[@id='"+id+"']")
	if article.IdCategorie < 1 {
		article.IdCategorie, _ = strconv.Atoi(node.SelectElement("idCateg").InnerText())
	}
	if article.Libelle == "" {
		article.Libelle = node.SelectElement("lib").InnerText()
	}
	if article.Prix < 0 {
		article.Prix, _ = strconv.ParseFloat(node.SelectElement("price").InnerText(), 64)
	}
	articleToAdd := getAnArticleAsXmlString(article)
	xmlData := strings.Split(getArticleXmlAsString(), "</articles>")[0] + articleToAdd + "</articles>"
	rewriteXml(xmlData)
}

func OverwriteArticleInXml(article *Article) {
	articleToOverwrite := getAnArticleAsXmlString(article)
	xmlData := getXmlBeforeAnArticle(getArticleXmlAsString(), article.ID) + articleToOverwrite + getXmlAfterAnArticle(getArticleXmlAsString(), article.ID)
	rewriteXml(xmlData)
}

func DeleteArticleInXml(id int) {
	//append in a string
	xmlData := getArticleXmlAsString()
	newXml := getXmlBeforeAnArticle(xmlData, id) + getXmlAfterAnArticle(xmlData, id)
	rewriteXml(newXml)
}

func getArticleXmlAsString() string {
	xmlData := "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
	root := getArticleXml()
	xmlquery.FindEach(root, "articles", func(i int, node *xmlquery.Node) {
		xmlData += node.OutputXML(true)
	})
	return xmlData
}

func rewriteXml(xmlData string) {
	pwd, _ := os.Getwd()
	lockArticle.Lock()
	if err := ioutil.WriteFile(pwd+"/public/data/articles.xml", []byte(xmlData), 7777); err != nil {
		panic(err)
	}
	f, err := os.Create(pwd + "/public/data/articles.xml")
	if err != nil {
		panic(err)
	}
	defer f.Close()
	_, _ = f.WriteString(xmlData)
	_ = f.Sync()
	lockArticle.Unlock()
}

func getAnArticleAsXmlString(article *Article) string {
	//getData
	id := strconv.Itoa(article.ID)
	lib := article.Libelle
	price := fmt.Sprintf("%.2f", article.Prix)
	idCateg := strconv.Itoa(article.IdCategorie)
	//append in a string
	articleToAdd := "<article id=\"" + id + "\"><lib>" + lib + "</lib><price>" + price + "</price><idCateg>" + idCateg + "</idCateg></article>"
	return articleToAdd
}

func getXmlBeforeAnArticle(xmlData string, id int) string {
	Id := strconv.Itoa(id)
	return strings.Split(xmlData, "<article id=\""+Id+"\">")[0]
}

func getXmlAfterAnArticle(xmlData string, id int) string {
	Id := strconv.Itoa(id)
	res := ""
	dataDelete := strings.Split(xmlData, "<article id=\""+Id+"\">")
	dataDelete = strings.Split(dataDelete[1], "</article>")
	for i := 1; i < len(dataDelete)-1; i++ {
		res += dataDelete[i] + "</article>"
	}
	res += dataDelete[len(dataDelete)-1]
	return res
}
