```
Author:     Joel Rodriguez
Partner:    Nandhini Ramanathan
Start Date: 11-Nov-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  joel-rodriguez
Repo:     https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-kollywoodtacos
Commit Date: 06-December-2024 10:00 PM
Project:    Snake
Copyright:  CS 3500 and [Joel Rodriguez] - This work may not be copied for use in Academic Coursework.```
```
# Notes to Instructors
1. We spoke with TA Parker who told us that since logging was not asked for in the assignment instructions, we would not need to add logging to our solution.  
2. We were also told that the baseline for program prettiness was using images and colorful snakes. We were not required to make a 1-to-1 replication of Professor de St Germaines' solution. For example, we are not required to add powerups that explode upon eating them.  
3. We were also told by a TA that we needed to ensure that when put into release mode the output window would NOT show any extraneous messages. So we made sure of that.  
4. We were also told by a TA that disabling the connect button after it being clicked once was a totally valid solution, and we did not need to worry about any other edge cases. However, the assignment says we MUST find a way to get it working. The only solution we found was in our connect method telling our code to wait until the server has finished attempting to connect before executing other code. This is because without that little waiting period, Blazor does not realize that it is supposed to be updating its page and therefore will mistakenly either disable the connect button or mistakenly display that the connection was successful when it was not. The reason why in our lab code we did not need to add this code was because the variable which we were using to decide whether or not to display the connect button lived inside the Razor class (the server). But in our solution, since the server lives in a different class, Blazor has a hard time noticing when it does change. Which is why we MUST implement this solution.  
5. We were also told to abstract our work and create a network controller class. So we did.  
6. We were asked to "Set the Tone" on our Home page, which is why we added jokes here and there to make you, the TA, feel better. We understand how draining grading so many assignments can be and playing snake over and over again.  
7. On random occasions, probably like one out of 30-40ish playthroughs, we found that Blazor will randomly give up and stop drawing. We found it with a TA that this is a Blazor problem, not anything with our code.  
8. Our program freezes on our computer when there are about 30 snakes in the game concurrently. Once again, with the help of a TA, we found out that this is because of my computer not being fast enough (My CPU and Memory would get to 100%). The limit may be higher or lower with your computer. The client never crashes with fewer than 15 snakes unless you run it on a very very very old PC.  
9. We were asked to discuss what works and what does not work, so we made that its own section on the Home page.  
10. An important note, if the GUI Snake seems to lag/overlap, please check your task manager since when the CPU and memory is very high it causes this because of the work the computer is doing and please trye running it again :)
11. When we run our code (on one of our computers) the frame rate starts a bit slower and quickly picks up to about 60 just so you know.
12. On my Computer, sometimes (pretty rare) the snake's controllers (down, right, etc) is a little laggy. When I checked my Task manager the CPU is barely being used on Visual studio like it didn't want to keep up or use the CPU on it. This only happens sometimes, but has never happened on my partern's laptop just so you know if it happens to you.
13. In our model classes the stylecop complains of properties not being started with a capital letter. This is unavoidable as it how the server wants to properties to be named. We could use JSONPropertyName attribute but for some reason doing this would give a weird compiler error so we decided to go with the working solution. We spoke with a TA and they said that some warnings are fine to have in our code.\
# DataBase Assign notes to instructors.
1. We were told that both the webserver and the snake game can run on port 11000 and that it was expected by a TA. So that is what we did.
2. We were also told that adding a new property to the Snake model is not required and that we can do whatever we thought was best and worked best 
   this is why we decided to not add a property and have the max scores live inside the networkController class since it that is the only place it is used.
3. A TA said on piazza that we don't need to handle the case of multiple tabs being opened (multiple clients modifiying database). So we did not handle that special case.
4. We were told that we did not have to use ORM's in class and we could do it as we pleased, which is what we did.
5. Given that the client razor page should NOT do anything other than just display things we decided to abstract our database work into the network controller class and a SQLQueries class.
6. We are not sure if it is simply a machine problem but if you go to a games page and then back out and back into it it will take a while to load, however if you click the X (where it is loading) and then click  the page again it will load immediately.
7. Both our snake game and webserver run on port 11000 we were told that this is fine and expected.
8. We were told in lecture that when the main client disconnects it should update the leave time of all the players who were still playing, we implemented this 
9. We were told in lecture we only needed a disconnect button. Our disconnect button works perfectly and I believe it also works when you simply x out or refresh you can try both but stick to the disconnect button if possible.
 We originally had a lot of tests in our database some where our data base code was not polished. We deleted them all so it's not a mess for you and we have 5 good working examples in the database now
   you can look at those as well.
10. We found certain cosmetic errors occured when disconnecting AI's, however there was a piazza post where a TA said that cosmetic errors or snake game errors did not matter. All that mattered was that our database was working as expected.
11. We had this odd error where our database wouldn't update correctly 1 in every probably about 30 time on my machine. However when we ran it on my partners laptop about 30 times she never ran into the error than when I tried again the database never ran into the error again. So if you see a discrepancy on your end please close the client and relaunch it may be some odd error with the database.
# Assignment Specific Topics
Our assignment specific topics include creating a GUI that will connect to a networked Snake Server and display the status of the game.  
We also send commands to the Snake Server representing the “moves” made by the player of the game. We also learned how to draw elements (in the draw method)
using Canvas in Blazor. This client holds model classes for the elements in our snake game like the snake, wall, powerups, world, and Point@D (x and y coordinates/
a point in our game world). We have a controller for the snake's movements and for holding the server's state (connected or disconnected). We have a "view" which is 
the snake razor in pages which holds teh main drawing functionality; it uses the deserialized data to draw the elements. We also have a score board with all the players 
connected.

# Consulted Peers:
List any peers (or other people) in the class (or outside for that matter) that you talked with about the project for more than one minute.

1. Adharsh
2. Jacob

# References:
    1. Debug Logging levels - https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-8.0
    2. The lock statement - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock
    3. SQLDataReader - https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader?view=net-8.0-pp
    4. ExectureScalar - https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand.executescalar?view=net-8.0-pp