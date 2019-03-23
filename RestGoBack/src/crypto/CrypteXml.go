package crypto

import (
	"fmt"
	"github.com/antchfx/xmlquery"
	"io/ioutil"
	"log"
	"os"
	"strings"
)


func getData() string {
	pwd, _ := os.Getwd()
	log.Println(pwd+"/public/data/crypto.xml")
	xml, _ := ioutil.ReadFile(pwd+"/public/data/crypto.xml")
	return string(xml)
}

func getByXml(path string) string {
	root, err := xmlquery.Parse(strings.NewReader(getData()))
	if err != nil {
		panic(err)
	}
	title := xmlquery.FindOne(root, path)
	log.Println(title.InnerText())
	return title.InnerText()
}

func getL33t(id string) string {
	path := "codes/code[@id='" + id + "']/l33t"
	return getByXml(path)
}

func getMorse(id string) string {
	path := "codes/code[@id='" + id + "']/morse"
	return getByXml(path)
}

func Test(id string) {
	fmt.Println(translateMorse(id))
	fmt.Println(translateL33t(id))
}

func translateMorse(str string) string {
	res := ""
	for _, char := range str {
		res += getMorse(string(char))+ " "
	}
	return strings.TrimSpace(res)
}

func translateL33t(str string) string {
	res := ""
	for _, char := range str {
		res += getL33t(string(char))
	}
	return strings.TrimSpace(res)
}
