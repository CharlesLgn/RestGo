package webservices

import (
  "github.com/antchfx/xmlquery"
  "io/ioutil"
  "os"
  "strconv"
  "strings"
  "sync"
)

type Article struct {
  ID          int      `xml:"article>id,attr" json:"id"`
  Libelle     string   `xml:"article>lib"     json:"lib"`
  Prix        float64  `xml:"article>price"   json:"price"`
  IdCategorie int      `xml:"article>idCateg" json:"idCateg"`
}

var articles map[int]*Article
var lockArticle = sync.RWMutex{}
var idArticle = 0

func getXml() *xmlquery.Node {
  pwd, _ := os.Getwd()
  xmlArticle, _ := ioutil.ReadFile(pwd + "/public/data/articles.xml")

  root, err := xmlquery.Parse(strings.NewReader(string(xmlArticle)))
  if err != nil {
    panic(err)
  }
  return root
}

func InitArticleWithXml() {
  root := getXml()
  articles = make(map[int]*Article)
  idArticle = -1
  xmlquery.FindEach(root, "articles/article", func(i int, node *xmlquery.Node) {
    id, _ := strconv.Atoi(node.Attr[0].Value)
    articlePath := "articles/article[@id='"+strconv.Itoa(id)+"']"
    lib := getData(articlePath+"/lib")
    price, _ := strconv.ParseFloat(getData(articlePath+"/price"), 64)
    idCateg, _ := strconv.Atoi(getData(articlePath+"/idCateg"))
    article := Article{
      ID: id,
      Libelle: lib,
      Prix : price,
      IdCategorie: idCateg,
    }
    articles[id] = &article
    if idArticle < id {
      idArticle = id
    }
  })
}

func getData(path string) string {
  root := getXml()
  return xmlquery.FindOne(root, path).InnerText()
}

func addArticle(article Article) {
  lockArticle.Lock()
  article.ID = idArticle
  idArticle += 1
  articles[article.ID] = &article
  lockArticle.Unlock()
}
