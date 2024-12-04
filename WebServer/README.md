```
Author:     Joel Rodriguez
Partner:    Nandhini Ramanathan
Start Date: 25-Nov-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  joel-rodriguez
Repo:     https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-kollywoodtacos
Commit Date: 6-December-2024 10:00 PM
Project:    WebServer
Copyright:  CS 3500 and [Joel Rodriguez] - This work may not be copied for use in Academic Coursework.```
```

# Comments to Evaluators:
1. We were told that both the webserver and the snake game can run on port 11000 and that it was expected by a TA. So that is what we did.
2. We were also told that adding a new property to the Snake model is not required and that we can do whatever we thought was best and worked best 
   this is why we decided to not add a property and have the max scores live inside the networkController class since it that is the only place it is used.
3. Our webserver does not use a Players or Games class we were told that this was not necessary for getting 100 by a TA. We were told that as long as were able to get things
   working then there was not anything that we were required to do. For example we did not use ORM's.

# Assignment Specific Topics
Our assignment specific topics include creating a Webserver that is connected to our data base to show the Snake Game's status oof the 
games and players playing it. It is designed to handle HTTP requests and responses. The server processes incoming client requests and 
supports basic web server functionalities. This Web Server console app has three main basic html functionalities from showing
the home page, the list of games (games table from database), and links to the player's table showing their name, ID, max score, start and 
end time.

# Consulted Peers:
List any peers (or other people) in the class (or outside for that matter) that you talked with about the project for more than one minute.

1. Adharsh
2. Jacob

# References:
      1. SQLDataReader - https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader?view=net-8.0-pp
      2. ExectureScalar - https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand.executescalar?view=net-8.0-pp
      3. GET Requests - https://www.w3schools.com/tags/ref_httpmethods.asp
