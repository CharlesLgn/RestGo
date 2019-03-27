# RestMan - Back
[![Ligony Charles](https://img.shields.io/badge/Charles-LinkedIn-1E90E7.svg)](https://www.linkedin.com/in/charles-ligony-893177134/)
[![Challouatte Cyril](https://img.shields.io/badge/Cyril-LinkedIn-1E90E7.svg)](https://www.linkedin.com/in/cyril-challouatte-824021160/)  
[![Okya project](https://img.shields.io/badge/%C3%98kya-Official-0c2461.svg)]()

Backend part of a project made for Licence Pro GL (_3rd year University_)  

***
## Difficulties: 
[![Difficulties](https://img.shields.io/badge/ReadMe-Difficulties-important.svg)](https://github.com/CharlesLgn/RestGo/blob/master/dificulties/README.md#dificulties-in-the-back-)

***
### _Run the Front:_  
[![Run the Front](https://img.shields.io/badge/ReadMe-Front-5BC7F8.svg)](https://github.com/CharlesLgn/RestGo/blob/master/RestManFront/README.md)  

***

### _Prerequisites:_

 - Golang : [1.11.15 or higher](https://golang.org/)
 - You need to check that you have `$GOROOT` in your environement variable
 - a directory in `:/user/go/src`

***

The project is built so you just need to run __./RestGo/RestGoBack/out/RestGoBack.exe__  
but if you make some change, here is the process to rebuild it

### _Build the project:_
 - Open a cmd and go to you directory `:/user/go/src`
     * if you have a C driver : `cd C:/user/go/src`
 - clone the project :
     * `git clone https://github.com/CharlesLgn/RestGo.git`
 - go in the back part :
     * `cd ./RestMan/RestGoBack`
 - add all library using :
     * `go get github.com/ant0ine/go-json-rest/rest` (json lib)
     * `go get -u github.com/gorilla/mux` (rest lib)
     * `go get github.com/antchfx/xmlquery` (xml lib)
     * `go get github.com/zserge/webview` (webview lib)
     
 - build all :
     * be on RestGoBack : `cd ./RestMan/RestGoBack`
     * `go build -ldflags="-H windowsgui" -o ./out/RestGoBack.exe ./src/main`
 - run project :
    * `./out/RestGoBack.exe`

___NB___: you will need internet until the application is not build 

***
### _API :_
[![Api](https://img.shields.io/badge/ReadMe-Api%20Back-important.svg)](https://github.com/CharlesLgn/RestGo/blob/master/RestGoBack/Api.md)
