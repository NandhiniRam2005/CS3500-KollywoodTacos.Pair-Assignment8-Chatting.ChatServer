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
line of code that just instantiates a new instance of the network connection, which allows teh user to reconnect back again.

We were a little worried that we logged too much in our code, but a TA also told us that having more logging statement is alright 
as long as we have levels. We incorporated this by logging BIG events like broadcasting messages, connecting, and disconnecting to the server
at the Info level. We logged lesser important events in Debug such as attempting to connect or disconnect from the server. We also log
small events such as entering and exiting methods successfully like when trying to read or write the message at the Trace level. We also 
have log warnings where appropriate like when the server unexpectedly disconnects, and the message was not sent through.

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
