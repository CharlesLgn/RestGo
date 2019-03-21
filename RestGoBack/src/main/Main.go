package main

import (
  "RestMan/RestGoBack/src/color"
  "RestMan/RestGoBack/src/crypto"
  "RestMan/RestGoBack/src/data"
  "RestMan/RestGoBack/src/github"
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
  go get8001()
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
  mux := http.NewServeMux()
  mux.Handle("/public/", http.StripPrefix("/public/", http.FileServer(http.Dir(dir+"/public"))))
  mux.HandleFunc("/terminal", getLog)
  mux.HandleFunc("/link", getLink)

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
    Handler: mux,
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
  webservices.InitArticleWithXml()
  webservices.InitCategoryWithXml()
  api := rest.NewApi()
  api.Use(rest.DefaultDevStack...)

  router, err := rest.MakeRouter(
    //Select
    rest.Get("/articles", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("get all article")
      webservices.GetArticles(writer, request)
    }),
    rest.Get("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      id := request.PathParam("id")
      printLogFront("get article : " + id)
      webservices.GetArticle(writer, request)
    }),
    rest.Get("/article/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      id := request.PathParam("id")
      printLogFront("get  articles by category : " + id)
      webservices.GetArticleByCateg(writer, request)
    }),
    rest.Get("/categories", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("get all categories")
      webservices.GetCategogies(writer, request)
    }),
    rest.Get("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      id := request.PathParam("id")
      printLogFront("get categorie : " + id)
      webservices.GetCategogie(writer, request)
    }),
    //Create
    rest.Post("/article", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Create article")
      webservices.CreateArticle(writer, request)
    }),
    rest.Post("/categorie", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Create category")
      webservices.CreateCategogie(writer, request)
    }),
    //Update
    rest.Put("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Update Article")
      webservices.UpdateArticle(writer, request)
    }),
    rest.Patch("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Patch Article")
      webservices.PatchArticle(writer, request)
    }),
    rest.Post("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Update Article")
      webservices.UpdateArticle(writer, request)
    }),
    rest.Put("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Update Category")
      webservices.UpdateCategogie(writer, request)
    }),
    rest.Patch("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Patch Category")
      webservices.PatchCategogie(writer, request)
    }),
    rest.Post("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Create category")
      webservices.UpdateCategogie(writer, request)
    }),
    //Delete
    rest.Delete("/article/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Delete Article")
      webservices.DeleteArticle(writer, request)
    }),
    rest.Delete("/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Delete Category")
      webservices.DeleteCategogie(writer, request)
    }),
    rest.Delete("/article/categorie/:id", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Delete Articles By Category")
      webservices.DeleteArticlesByCateg(writer, request)
    }),

    //Fun
    rest.Post("/fun/Data", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Rest Go Back Version")
      data.GetData(writer, request)
    }),
    rest.Get("/fun/wiki/:title", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Get Wiki page")
      wikipedia.GetPage(writer, request)
    }),
    rest.Get("/fun/yes", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Yes No Maybe")
      yesnomaybe.YesNoMaybe(writer, request)
    }),
    rest.Get("/fun/color", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Random Color")
      color.RandomColor(writer, request)
    }),
    rest.Post("/fun/trad/l33t", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Translate L33t")
      crypto.TranslteL33t(writer, request)
    }),
    rest.Post("/fun/trad/morse", func(writer rest.ResponseWriter, request *rest.Request) {
      printLogFront("Translate Morse")
      crypto.TranslteMorse(writer, request)
    }),
    rest.Get("/fun/github/:user", func(writer rest.ResponseWriter, request *rest.Request) {
      user := request.PathParam("user")
      printLogFront("github user : " + user)
      github.GetGithubLanguagePercent(writer, request)
    }),
  )
  if err != nil {
    log.Fatal(err)
  }
  api.SetApp(router)
  printLogFront("Server start !")
  log.Println("Server start !")
  log.Fatal(http.ListenAndServe(":8000", api.MakeHandler()))
}

func get8001() {
  printLogFront("RestMan is starting")
  http.HandleFunc("/fun/lissa", func(writer http.ResponseWriter, r *http.Request) {
    printLogFront("Lissajous")
    lissajous.LissajousWeb(writer, r)
  })
  printLogFront("Server start !")
  log.Fatal(http.ListenAndServe(":8001", nil))
}

func printLogFront(str string) {
  logger += str + string('☻')
}
