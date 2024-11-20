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
