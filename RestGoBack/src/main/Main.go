package main

import (
	"RestMan/RestGoBack/src/color"
	"RestMan/RestGoBack/src/crypto"
	"RestMan/RestGoBack/src/data"
	"RestMan/RestGoBack/src/lissajous"
	"RestMan/RestGoBack/src/webservices"
	"RestMan/RestGoBack/src/wikipedia"
	"RestMan/RestGoBack/src/yesnomaybe"
	"github.com/ant0ine/go-json-rest/rest"
	"github.com/zserge/webview"
	"log"
	"net"
	"net/http"
	"os"
	"path/filepath"
)

var dir string                           // current directory
var windowWidth, windowHeight = 720, 480 // width and height of the window

var logg  = ""

func init() {
	// getting the current directory to access resources
	var err error
	dir, err = filepath.Abs(filepath.Dir(os.Args[0]))
	if err != nil {
		log.Fatal(err)
	}
}

// main function
func main() {
	f, err := os.OpenFile("./tmp/orders.log", os.O_RDWR|os.O_CREATE|os.O_APPEND, 0666)
	if err != nil {
		log.Fatalf("error opening file: %v", err)
	}
	defer f.Close()

	// channel to get the web prefix
	prefixChannel := make(chan string)
	// run the web server in a separate goroutine
	go app(prefixChannel)
	go get8000()
	go get8001()
	prefix := <- prefixChannel
	// create a web view
	err = webview.Open("RestGoBack", prefix + "/public/html/index.html",
		windowWidth, windowHeight, false)
	if err != nil {
		log.Fatal(err)
	}
}

// web app
func app(prefixChannel chan string) {
	mux := http.NewServeMux()
	mux.Handle("/public/", http.StripPrefix("/public/", http.FileServer(http.Dir(dir+"/public"))))
	mux.HandleFunc("/terminal", getLog)

	// get an ephemeral port, so we're guaranteed not to conflict with anything else
	listener, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		panic(err)
	}
	portAddress := listener.Addr().String()
	prefixChannel <- "http://" + portAddress
	_ =listener.Close()
	server := &http.Server{
		Addr:    portAddress,
		Handler: mux,
	}
	_ = server.ListenAndServe()
}

// get the game frames
func getLog(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Cache-Control", "no-cache")
	_, _ = w.Write([]byte(logg))
}

func get8000(){
	println("RestMan is starting")
	log.Println("RestMan is starting")
	webservices.InitArticle()
	webservices.InitCateg()
	api := rest.NewApi()
	api.Use(rest.DefaultDevStack...)

	router, err := rest.MakeRouter(
		//Select
		rest.Get	("/articles", func(writer rest.ResponseWriter, request *rest.Request) {
			println("get all article")
			webservices.GetArticles(writer, request)
		}),
		rest.Get	("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			id := request.PathParam("id")
			println("get article : " + id)
			webservices.GetArticle(writer, request)
		}),
		rest.Get	("/article/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			id := request.PathParam("id")
			println("get  articles by categ : " + id)
			webservices.GetArticleByCateg(writer, request)
		}),
		rest.Get	("/categories", func(writer rest.ResponseWriter, request *rest.Request) {
			println("get all categories")
			webservices.GetCategogies(writer, request)
		}),
		rest.Get	("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			id := request.PathParam("id")
			println("get categorie : " + id)
			webservices.GetCategogie(writer, request)
		}),
		//Create
		rest.Post	("/article", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Create article")
			webservices.CreateArticle(writer, request)
		}),
		rest.Post	("/categorie", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Create category")
			webservices.CreateCategogie(writer, request)
		}),
		//Update
		rest.Put	("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Update Article")
			webservices.UpdateArticle(writer, request)
		}),
		rest.Patch	("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Patch Article")
			webservices.PatchArticle(writer, request)
		}),
		rest.Post	("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Update Article")
			webservices.UpdateArticle(writer, request)
		}),
		rest.Put	("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Update Category")
			webservices.UpdateCategogie(writer, request)
		}),
		rest.Patch	("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Patch Category")
			webservices.PatchCategogie(writer, request)
		}),
		rest.Post	("/categorie/:id", 	func(writer rest.ResponseWriter, request *rest.Request) {
			println("Create category")
			webservices.UpdateCategogie(writer, request)
		}),
		//Delete
		rest.Delete	("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Delete Article")
			webservices.DeleteArticle(writer, request)
		}),
		rest.Delete	("/categorie/:id", 	func(writer rest.ResponseWriter, request *rest.Request) {
			println("Delete Category")
			webservices.DeleteCategogie(writer, request)
		}),
		rest.Delete	("/article/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Delete Articles By Category")
			webservices.DeleteArticlesByCateg(writer, request)
		}),

		//Fun
		rest.Post	("/fun/Data", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Rest Go Back Version")
			data.GetData(writer, request)
		}),
		rest.Get	("/fun/wiki/:title", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Get Wiki page")
			wikipedia.GetPage(writer, request)
		}),
		rest.Get	("/fun/yes", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Yes No Maybe")
			yesnomaybe.YesNoMaybe(writer, request)
		}),
		rest.Get	("/fun/color", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Random Color")
			color.RandomColor(writer, request)
		}),
		rest.Post	("/fun/trad/l33t", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Translte L33t")
			crypto.TranslteL33t(writer, request)
		}),
		rest.Post	("/fun/trad/morse", func(writer rest.ResponseWriter, request *rest.Request) {
			println("Translte Morse")
			crypto.TranslteMorse(writer, request)
		}),
	)
	if err != nil {
		log.Fatal(err)
	}
	api.SetApp(router)
	println("Server start !")
	log.Println("Server start !")
	log.Fatal(http.ListenAndServe(":8000", api.MakeHandler()))
}

func get8001(){
	println("RestMan is starting")
	http.HandleFunc("/fun/lissa", func(writer http.ResponseWriter, r *http.Request) {
		println("Lissajous")
		lissajous.LissajousWeb(writer, r)
	})
	println("Server start !")
	log.Fatal(http.ListenAndServe(":8001", nil))
}

func println(str string)  {
	logg += str+string('☻')
}