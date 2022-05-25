# Chat App Server
## a chatting app by Matan Shamir and Itamar Bachar - server side
1. [About](#About)
2. [Dependencies](#dependencies)  
3. [Pages-flow-and-explanation](#Pages-flow-and-explanation)
4. [How-to-open-the-app](#How-to-open-the-app)
5. [How-to-use-the-app](#How-to-use-the-app)
6. [Developers](#Developers)


## About
this is the server part of the second milestone in advanced programming 2 course.

## Dependencies
* Windows / Linux / macOS
* Git
* Visual Studio's newest version
## Web Server
here, we will expand explanation about the web server part of this project.
### Pages-flow-and-explanation - Ratings app
we have been asked to build a ratings app via MVC, without the react project. so if one browses to http://localhost:5243/ he receives an html of our ratings app, unlike the react projects which receives json. this is part of the server functions as a web server and not a web service.
from the actuall react chat app, only logged-in users are able to get tot this page, by pressing "go to ratings".
### Main page
this page is index.html, it consists of a larger view of the current average rating and a list of all ratings by now.
<br />
![image](https://user-images.githubusercontent.com/74719554/170249618-f5012393-5a4e-4253-90fd-e94855351c2d.png)

### Create new
by pressing "create new" you can create a new rating, specifying your details.
<br />
![image](https://user-images.githubusercontent.com/74719554/170249736-ee9a7f96-3a95-4ea3-9940-e8f039ef9760.png)
<br />
you can also edit, get more details or delete a comment by pressing:
![image](https://user-images.githubusercontent.com/74719554/170249946-611bfdcd-3314-421c-b0c9-bb13e9e7c3bd.png)
**as for now, everyone can edit anyones comments...**

## Web Service
this project also functions as a web service for a react app.
you can run this project, run the react app (they run from different servers)
the react app client-side has get,post,put and delete requests to our web service regarding users, contacts, conversations and messages.
test the react app from our **ChatApp** repo to see the web service functionality. dont forget to run it though!

## How-to-open-the-app

1. If you dont have visual studio's newest version, please install it.
2. Choose an empty project, and please do:
  ![image](https://user-images.githubusercontent.com/74719554/170250838-2316fed6-fa43-44ef-8953-2a6fa751ba92.png)
   This will open a window:
   ![image](https://user-images.githubusercontent.com/74719554/170250972-b5c02025-8ade-478d-84e7-d477229f7059.png)
enter this repo's location from the ![image](https://user-images.githubusercontent.com/74719554/170251059-59ce3781-092e-4e43-8c58-cb6c3e9df193.png)
part, and walla! you cloned our repo.
4. run the server.

## How-to-use-the-app
please refer **ChatApp** repository for more information on how to activate the client.

## Developers:
**Matan Shamir 206960239** <br />
**Itamar Bachar 295847376**
