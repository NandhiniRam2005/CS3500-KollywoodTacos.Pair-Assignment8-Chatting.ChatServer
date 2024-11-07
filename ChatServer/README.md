```
Author:     Joel Rodriguez
Partner:    Nandhini Ramanathan
Start Date: 01-Nov-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  joel-rodriguez
Repo:     https://github.com/uofu-cs3500-20-fall2024/assignment-eight-chatting-kollywoodtacos
Commit Date: 8-November-2024 10:00 PM
Project:    ChatServer
Copyright:  CS 3500 and [Joel Rodriguez] - This work may not be copied for use in Academic Coursework.```
```

# Comments to Evaluators:
A TA told us to only create two loggers for the entire solution, including both
the ChatServer and NetworkConnection classes. So we created only two instances of
ILoger for the server and one for the client, rather than multiple
loggers across all our classes.

- The ChatServer logger handles logging of server events such as client connections, message
  broadcasting, and error warnings.

# Assignment Specific Topics
1. Instrumenting your code to log the "right" message, to the "right" place, at the "right" time..
2. Understand and describe how networking code works
3. Understand how to protect critical regions from race conditions encountered in parallel programs. 
We apply all these assignment specific topics by adding threading to handle multiple clients at the same time. We also do this by adding and
implementing a Network Connection Class. We also implement Debugging in this ChatServer class.
# Consulted Peers:

List any peers (or other people) in the class (or outside for that matter) that you talked with about the project for more than one minute.

1. Nandhini Ramanathan

# References:
    1. Debug Logging levels - https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-8.0
    2. Threads in C# - https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread?view=net-8.0
    3. The lock statement - https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock
