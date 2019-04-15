# ___API :___

## Run the Back: 
[![Run the Back](https://img.shields.io/badge/ReadMe-Back-75CEDE.svg)](https://github.com/CharlesLgn/RestGo/blob/master/RestGoBack/README.md)

## Note
All request for `article` and `category`  can be receive in __XML__, __JSON__ and __YAML__
But for post request, just JSON is read

## Article
 - GET :
   - __`localhost:8000/articles`__
     - Get all articles store
     - `X-Article-Id` : if there is a `X-Article-Id` in the Header of the request,
     Get just the article whit this id
     - `X-display-categ`  : if you write `true` in this Header,
     the data you receive appears with the _categorie id and lib_  
   - __`localhost:8000/article/{id}`__
     - Get the article whit this id  
      If the id does not exit, it return an 400 error
     - `X-display-categ`  : if you write `true` in this Header,
            the data you receive appears with the _categorie id and lib_
   - __`localhost:8000/article/categorie/{id}`__
     - Get all article from a categorie whit this id  
      If the id does not exit, it return an 400 error
     - `X-display-categ`  : if you write `true` in this Header,
          the data you receive appears with the _categorie id and lib_
 - POST :
   - __`localhost:8000/article`__
     - Create an article
     - Json to send (all can't be omit) :
     ```JSON
     {
       "lib"    : "string",
       "price"  : 0.00,
       "idCateg": 0
     }
     ```
   - __`localhost:8000/article/{id}`__
     - Update the article whit this id
     - Json example to send (all can be omit) :
     ```JSON
     {
       "lib"    : "string",
       "price"  : 0.00,
       "idCateg": 0
     }
     ```
 - PUT :
   - __`localhost:8000/article/{id}`__
     - Overwrite the article whit this id
     - Json to send (all can't be omit) :
     ```JSON
     {
       "lib"    : string,
       "price"  : double,
       "idCateg": int
     }
     ```
 - PATCH :
   - __`localhost:8000/article/{id}`__
     - Update the article whit this id
     - Json to send (all can be omit) :
     ```JSON
     {
       "lib"    : string,
       "price"  : double,
       "idCateg": int
     }
     ```
 - DELETE :
   - __`localhost:8000/article/{id}`__
     - Delete the article whit this id

***

## Categorie
 - GET :
   - __`localhost:8000/categories`__
     - Get all categories store
     - `X-Categorie-Id` : if there is a `X-Categorie-Id` in the Header of the request,  
     Get just the categorie whit this id
   - __`localhost:8000/categorie/{id}`__
     - Get the categorie whit this id  
      If the id doesn't exit, it return an 400 eror
 - POST :
   - __`localhost:8000/categorie`__
     - Create an categorie
     - Json to send (all can't be omit) :
     ```JSON
     {
       "lib" : string
     }
     ```
   - __`localhost:8000/categorie/{id}`__
     - Update the categorie whit this id
     - Json to send (all can be omit) :
     ```JSON
     {
       "lib" : string
     }
     ```
 - PUT :
   - __`localhost:8000/article/{id}`__
     - Overwrite the categorie whit this id
     - Json to send (all can't be omit) :
     ```JSON
     {
       "lib" : string
     }
     ```
 - PATCH :
   - __`localhost:8000/categorie/{id}`__
     - Update the categorie whit this id
     - Json to send (all can be omit) :
     ```JSON
     {
       "lib" : string
     }
     ```
 - DELETE :
   - __`localhost:8000/categorie/{id}`__
     - Delete the categorie whit this id

***
## Fun API
 - GET :
   - __`localhost:8000/fun/color`__
     - Get data of a random color
   - __`localhost:8000/fun/yes`__
     - Get the answer `yes`, `no` or `maybe`  
   - __`localhost:8000/fun/wiki/{title}`__
     - Get the wikipedia resume of this title  
   - __`localhost:8000/fun/github/{user}`__ (Beta)
     - Get percent of languages use by a user
   - __`localhost:8000/fun/lissa`__
     - Get an picture of a Lissajous curve   
 - POST :
   - __`localhost:8000/fun/data`__
     - RestGo Back version
     - Json to send (some can be omit) :
     ```JSON
     {
       "url":string,                    //can't be omit
       "content-type":string,           //if omit : Application/json
       "method":string,                 //if omit : GET
       "lang":string,                   //can be nothing
       "param":string (as json format)  //can be nothing
     }
     ```
   - __`localhost:8000/fun/trad/l33t`__
     - L33t translator
     - Json to send (can't be omit) :
     ```JSON
     {
       "message":string
     }
     ```
   - __`localhost:8000/fun/trad/morse`__
     - Morse translator
     - Json to send (can't be omit) :
     ```JSON
     {
       "message":string
     }
     ```

lissajous :  
[![gif](./lissa.gif)](./lissa.gif)
