```
Author:     Joel Rodriguez
Partner:    Nandhini Ramanathan
Start Date: 01-Nov-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  joel-rodriguez
Repo:     https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-kollywoodtacos
Commit Date: 8-November-2024 10:00 PM
Project:   ChatClient.Client
Copyright:  CS 3500 and [Joel Rodriguez] - This work may not be copied for use in Academic Coursework.```
```

# Comments to Evaluators:

Originally, after a client disconnected, they couldn't reconnect to the server again because when you disconnect,
the method disposes the network connection object that it uses. So, in our DisconnectFromServer, after disconnected we added a 
line of code that just instantiates a new instance of the network connection, which allows the user to reconnect back again. However
every time the user reconnects the server will treat them as a new user (new tcpClient) so they wont be "logged in" to their name. We chose
to create a new networking object every time the disconnect and reconnect.

We were a little worried that we logged too much in our code, but a TA also told us that having more logging statement is alright 
as long as we have levels. We incorporated this by logging BIG events like broadcasting messages, connecting, and disconnecting to the server
at the Info level. We logged lesser important events in Debug such as attempting to connect or disconnect from the server. We also log
small events such as entering and exiting methods successfully like when trying to read or write the message at the Trace level. We also 
have log warnings where appropriate like when the server unexpectedly disconnects, and the message was not sent through. However in class Professor 
de St Germaine said to mainly use debug and information level debugging. (Slide 13 Lecture 21). We decided with prof's guidelines.\

Also in lecture he said we got to up to Minimum log level once we are done and ready to submit to the Information level (See lecture 21 around
the 13 minute mark.) We did that as well. So if you notice that we don't have much logs it is because we upped the min level like prof told us to.

# Partnership Information
All of our code was done using pair programming. We found that it would be too difficult and unfair for us to split up the work so we
decided to not split up the work.Our schedules aligned very well so we decided to just meet up whenever we could on campus to work on
the assignment.On occasion we would also work together over discord on late nights when we were feeling motivated.The only work
we did on our own time at times was searching up bugs.When we encountered a bug in our code we would pause and do some googling on our own
and rejoin when we had a found a solution on line.

We both felt that our partnership was a success.We were partners in 2420 so we are already familiar with each others coding styles and this
assignment was a breeze because of that.The first reason why our partnership was so successful was because both of us having very different
ways of approaching code problems. For example there was many times during our implementation where one of us would stop the other person
while they were trying to code something and suggest a simpler and faster way of doing things.Another way our partnership was successful
was that we were able to bounce ideas of each other and give each other good and constructive feedback.Neither of us were afraid to tell
each other that we thought an idea wouldn't add much to the spreadsheet or would be too hard too implement. To help the code process go faster 
when we realized that one us was getting tired of typing or was getting their brain fried we would switch off drivers and all the other person
to start. We would take periodic breaks to ensure we only coded quality code. We chose not to assign tasks since that felt unfair to the both of us.

Although are partnership was great their was places where we could improve. For example I felt that we had two problems that sometimes made us program
slowly and not be to get the assignment done as quickly as we could of. One of the things that held us back was our similar class schedule we would frequently
get distracted talking about other class homework like 3810 homework since we have the same class together. Another problem we had was talking too much, we
always went to do work in the cade lab together with all of our friends and we found that we kept distracting each other with conversations about Bronny James
and Jujutsu Kaisen.We need to be able to focus more on future assignments.

# Assignment Specific Topics
1. Instrumenting your code to log the "right" message, to the "right" place, at the "right" time..
2. Understand and describe how networking code works
3. Understand how to protect critical regions from race conditions encountered in parallel programs. 
We apply all these assignment specific topics by adding threading to handle multiple clients at the same time. We also do this by adding and
implementing a Network Connection Class. We also implement Debugging in this ChatClient.Client class.
# Consulted Peers:

List any peers (or other people) in the class (or outside for that matter) that you talked with about the project for more than one minute.

1. Nandhini Ramanathan

# References:
	 1. Debug Logging levels - https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-8.0
	 2. Threads in C# - https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-8.0
	 3. IDisposable Interface - https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0
