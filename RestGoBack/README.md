# RestMan - Back
[![Ligony Charles](https://img.shields.io/badge/Charles-LinkedIn-1E90E7.svg)](https://www.linkedin.com/in/charles-ligony-893177134/)
[![Challouatte Cyril](https://img.shields.io/badge/Cyril-LinkedIn-1E90E7.svg)](https://www.linkedin.com/in/cyril-challouatte-824021160/)  
[![Okya project](https://img.shields.io/badge/%C3%98kya-Official-0c2461.svg)]()

Backend part of a project made for Licence Pro GL (_3rd year University_)  

***

### _Run the Front:_  
[![Run the Front](https://img.shields.io/badge/ReadMe-Front-5BC7F8.svg)]()  

***

### _Prerequisites:_

 - Golang : [1.11.15](https://golang.org/)
 - You need to check that you have `$GOROOT` in your environement variable
 - a directory in `:/user/go/src`

***

### _Build the project:_
 - Open a cmd and go to you directory `:/user/go/src`
     * if you have a C driver : `cd C:/user/go/src`
 - clone the project :
     * `git clone https://github.com/CharlesLgn/RestMan.git`
 - go in the back part :
     * `cd ./RestMan/RestGoBack`
 - add all library using :
     * `go get github.com/ant0ine/go-json-rest/rest` (json lib)
     * `go get -u github.com/gorilla/mux` (rest lib)
     * `go get github.com/antchfx/xmlquery` (xml lib)
     * `go get github.com/zserge/webview` (webview lib)
     
 - build all :
     * `go build -ldflags="-H windowsgui" -o ./out/RestGoBack.exe`
 - run project :
    * `./out/RestGoBack.exe`

***
### _API :_

<table>
 <tr>
  <th>Name</th>
  <th>Method</th>
  <th>Json-data to send</th>
  <th>URL</th>
  <th>Descritpion</th>
 </tr>
 <tr>
  <th colspan="5">Article</th>
 </tr>
 <tr>
  <td>GetArticles</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/articles</td>
  <td>Get all articles store</td>
 </tr>
 <tr>
  <td>GetArticle</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/article/:id</td>
  <td>Get the article whit this id<br>
      If the id doesn't exit, it return an 400 eror</td>
 </tr>
 <tr>
  <td>CreateArticle</td>
  <td>POST</td>
  <td><p>{<br>
   'lib':string,<br>
   'price':double,<br>
   'idCeteg':int<br>
   }</p></td>
  <td>localhost:8000/article</td>
  <td>Create an article<br>
      If categ doesn't exist, it return an error</td>
 </tr>
 <tr>
  <td>UpdateArticle</td>
  <td>POST/PUT</td>
  <td><p>{<br>
   'lib':string,<br>
   'price':double,<br>
   'idCeteg':int<br>
   }</p></td>
  <td>localhost:8000/article/:id</td>
  <td>Update an article<br>
      If id doesn't exist, it return an error</td>
 </tr>
 <tr>
  <td>OverwriteArticle</td>
  <td>PATCH</td>
  <td><p>{<br>
   'lib':string,<br>
   'price':double,<br>
   'idCeteg':int<br>
   }</p></td>
  <td>localhost:8000/article/:id</td>
  <td>overwrite an article<br>
      If id doesn't exist, it return an error</td>
 </tr>
 <tr>
  <td>DeleteArticle</td>
  <td>DELETE</td>
  <td></td>
  <td>localhost:8000/article/:id</td>
  <td>Delete the article whit this id<br>
      If the id doesn't exit, it return an 400 eror</td>
 </tr>
 <tr>
  <td>GetArticleByCateg</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/article/categorie/:id</td>
  <td>Get all article from a categorie whit this id<br>
      If the id doesn't exit, it return an 400 eror</td>
 </tr>
 <tr>
  <td>DeleteArticleByCateg</td>
  <td>DELETE</td>
  <td></td>
  <td>localhost:8000/article/categorie/:id</td>
  <td>Delete all article from a categorie whit this id<br>
      If the id doesn't exit, it return an 400 eror</td>
 </tr>
 <tr>
  <th colspan="5">Category</th>
 </tr>
 <tr>
  <td>GetCategories</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/categorie</td>
  <td>Get all categories store</td>
 </tr>
 <tr>
  <td>GetCategory</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/categorie/:id</td>
  <td>Get the category whit this id<br>
      If the id doesn't exit, it return an 400 eror</td>
 </tr>
 <tr>
  <td>CreateCategory</td>
  <td>POST</td>
  <td><p>{'lib':string}</p></td>
  <td>localhost:8000/categorie</td>
  <td>Create a category<br>
      If categ doesn't exist, it return an error</td>
 </tr>
 <tr>
  <td>UpdateCategory</td>
  <td>POST/PUT</td>
  <td><p>{'lib':string}</p></td>
  <td>localhost:8000/categorie/:id</td>
  <td>Update a category<br>
      If id doesn't exist, it return an error</td>
 </tr>
 <tr>
  <td>OverwriteCategory</td>
  <td>PATCH</td>
  <td><p>{'lib':string}</p></td>
  <td>localhost:8000/categorie/:id</td>
  <td>overwrite a category<br>
      If id doesn't exist, it return an error</td>
 </tr>
 <tr>
  <td>DeleteCategory</td>
  <td>DELETE</td>
  <td></td>
  <td>localhost:8000/categorie/:id</td>
  <td>Delete the category whit this id<br>
      If the id doesn't exit, it return an 400 eror</td>
 </tr>
 <tr>
  <th colspan="5">Fun API</th>
 </tr>
 <tr>
  <td>RestGoBackVersion</td>
  <td>POST</td>
  <td>{<br>
   "url":string,<br>
   "content-type":string,<br>
   "method":string,<br>
   "lang":string,<br>
   "param":string (as json format)<br>
   }</td>
  <td>localhost:8000/fun/Data</td>
  <td>send a request to the api you want and give you an awnser (it made the same work as RestGo Front but in back)</td>
 </tr>
 <tr>
  <td>YesNoMaybe</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/fun/yes</td>
  <td>send you yes, no or maybe</td>
 </tr>
 <tr>
  <td>RandomColor</td>
  <td>GET</td>
  <td></td>
  <td>localhost:8000/fun/color</td>
  <td>give you a random color. you can see it on google et it give you the % of light and saturation</td>
 </tr>
 <tr>
  <td>MorseTrad</td>
  <td>POST</td>
  <td>{'message':string}</td>
  <td>localhost:8000/fun/trad/morse</td>
  <td>get the text send in morse</td>
 </tr>
 <tr>
  <td>L33tTrad</td>
  <td>POST</td>
  <td>{'message':string}</td>
  <td>localhost:8000/fun/trad/L33t</td>
  <td>get the text send in L33t</td>
 </tr>
 <tr>
  <td>LissajousGIF</td>
  <td>Get</td>
  <td></td>
  <td>localhost:8001/fun/lissa</td>
  <td>get a random Lisajous effect as GIF</td>
 </tr>
</table>

lissajous :  
[![gif](./lissa.gif)](./lissa.gif)

