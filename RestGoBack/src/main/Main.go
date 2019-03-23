package main

import (
  "RestMan/RestGoBack/src/clock"
  "RestMan/RestGoBack/src/color"
  "RestMan/RestGoBack/src/data"
  "RestMan/RestGoBack/src/github"
  "RestMan/RestGoBack/src/lissajous"
  "RestMan/RestGoBack/src/webservices"
  "RestMan/RestGoBack/src/wikipedia"
  "RestMan/RestGoBack/src/yesnomaybe"

  "github.com/gorilla/mux"
  "github.com/zserge/webview"

  "log"
  "net"
  "net/http"
  "os"
  "path/filepath"
)

var dir string                           // current directory
var windowWidth, windowHeight = 720, 480 // width and height of the window

var logger = ""
var prefixChannel = make(chan string)

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
  // run the web server in a separate goroutine
  go app(prefixChannel)
  go get8000()
  prefix := <-prefixChannel
  // create a web view
  err := webview.Open("RestGoBack", prefix+"/public/html/index.html",
    windowWidth, windowHeight, true)
  if err != nil {
    log.Fatal(err)
  }
}

// web app
func app(prefixChannel chan string) {
  serveMux := http.NewServeMux()
  serveMux.Handle("/public/", http.StripPrefix("/public/", http.FileServer(http.Dir(dir+"/public"))))
  serveMux.HandleFunc("/terminal", getLog)
  serveMux.HandleFunc("/link", getLink)

  // get an ephemeral port, so we're guaranteed not to conflict with anything else
  listener, err := net.Listen("tcp", "127.0.0.1:0")
  if err != nil {
    panic(err)
  }
  portAddress := listener.Addr().String()
  prefixChannel <- "http://" + portAddress
  _ = listener.Close()
  server := &http.Server{
    Addr:    portAddress,
    Handler: serveMux,
  }
  _ = server.ListenAndServe()
}

func getLink(_ http.ResponseWriter, _ *http.Request) {
  printLogFront("info")
  prefix := <-prefixChannel
  // create a web view
  err := webview.Open("info", prefix+"/public/html/info.html",
    400, 400, false)
  if err != nil {
    log.Fatal(err)
  }
}

// get the game frames
func getLog(w http.ResponseWriter, _ *http.Request) {
  w.Header().Set("Cache-Control", "no-cache")
  _, _ = w.Write([]byte(logger))
}

func get8000() {
  printLogFront("RestMan is starting")
  log.Println("RestMan is starting")
  router := mux.NewRouter()

  //****************************
  // Article
  //****************************
  router.HandleFunc("/articles", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("get all articles")
    webservices.GetArticles(writer, request)
  }).Methods("GET")
  router.HandleFunc("/article/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("get an article")
    webservices.GetArticleById(writer, request)
  }).Methods("GET")
  router.HandleFunc("/article/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("get all articles by category")
    webservices.GetArticleByCateg(writer, request)
  }).Methods("GET")

  //Create
  router.HandleFunc("/article", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("get all article")
    webservices.CreateArticle(writer, request)
  }).Methods("POST")
  //Delete
  router.HandleFunc("/article/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("delete an article")
    webservices.DeleteArticle(writer, request)
  }).Methods("DELETE")
  router.HandleFunc("/article/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("delete all articles by category")
    webservices.DeleteArticlesByCateg(writer, request)
  }).Methods("DELETE")

  //Override
  router.HandleFunc("/article/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Override an article")
    webservices.OverrideArticle(writer, request)
  }).Methods("PUT")
  //Update
  router.HandleFunc("/article/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Update an article")
    webservices.UpdateArticle(writer, request)
  }).Methods("PATCH")
  router.HandleFunc("/article/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Update an article")
    webservices.UpdateArticle(writer, request)
  }).Methods("POST")

  //****************************
  // Category
  //****************************
  router.HandleFunc("/categories", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("get all categories")
    webservices.GetCategories(writer, request)
  }).Methods("GET")
  router.HandleFunc("/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("get a categorie")
    webservices.GetCategoryById(writer, request)
  }).Methods("GET")

  //Create
  router.HandleFunc("/categorie", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Create a categorie")
    webservices.CreateCategory(writer, request)
  }).Methods("POST")

  //Delete
  router.HandleFunc("/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Delete a categorie")
    webservices.DeleteCategory(writer, request)
  }).Methods("DELETE")
  //Override
  router.HandleFunc("/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Override a categorie")
    webservices.OverrideCategory(writer, request)
  }).Methods("PUT")
  //Update
  router.HandleFunc("/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Update a categorie")
    webservices.UpdateCategory(writer, request)
  }).Methods("PATCH")
  router.HandleFunc("/categorie/{id}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Update a categorie")
    webservices.UpdateCategory(writer, request)
  }).Methods("POST")

  //****************************
  // fun
  //****************************
  router.HandleFunc("/fun/data", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("RestGo Back Version")
    data.GetData(writer, request)
  }).Methods("POST")
  router.HandleFunc("/fun/color", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Random Color")
    color.RandomColor(writer, request)
  }).Methods("GET")
  router.HandleFunc("/fun/yes", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Yes No Maybe")
    yesnomaybe.YesNoMaybe(writer, request)
  }).Methods("GET")
  router.HandleFunc("/fun/wiki/{title}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Get Wiki page")
    wikipedia.GetPage(writer, request)
  }).Methods("GET")
  router.HandleFunc("/fun/github/{user}", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("github user")
    github.GetGithubLanguagePercent(writer, request)
  }).Methods("GET")
  router.HandleFunc("/fun/trad/l33t", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Translate L33t")
    github.GetGithubLanguagePercent(writer, request)
  }).Methods("POST")
  router.HandleFunc("/fun/trad/morse", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Translate Morse")
    lissajous.LissajousWeb(writer, request)
  }).Methods("POST")
  //****************************
  // Graphique
  //****************************
  router.HandleFunc("/fun/lissa", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Lissajous")
    lissajous.LissajousWeb(writer, request)
  }).Methods("GET")
  router.HandleFunc("/fun/clock", func(writer http.ResponseWriter, request *http.Request) {
    printLogFront("Clock")
    clock.ClockWeb(writer, request)
  }).Methods("GET")

  printLogFront("Server start !")
  log.Println("Server start !")
  log.Fatal(http.ListenAndServe(":8000", router))
}

func printLogFront(str string) {
  logger += str + string('☻')
}
