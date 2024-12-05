// <copyright file="NetworkController.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace Snake.Client.Controller;

using CS3500.Networking;
using Snake.Client.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static System.Formats.Asn1.AsnWriter;

/// <summary>
/// Author:    Joel Rodriguez,  Nandhini Ramanathan, and Professor Jim.
/// Partner:   None
/// Date:      December 6, 2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and [Joel Rodriguez and Nandhini Ramanathan] - This work may no
///            be copied for use in Academic Coursework.
///
/// I, Joel Rodriguez and Nandhini Ramanathan, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All
/// references used in the completion of the assignments are cited
/// in my README file.
///
/// File Contents
///     This class manages a network connection for a Snake game, handling communication
///     with a game server. It includes methods to connect to the server, receive updates, and send user input
///     commands. This class uses the NetworkConnection class to manage the underlying TCP connection and includes
///     methods to handle user input via key presses.
/// </summary>
public class NetworkController
{
    /// <summary>
    /// Stores the game ID retrieved from the database.
    /// </summary>
    private int gameID;

    /// <summary>
    /// Stores the time when the connection was established.
    /// </summary>
    private DateTime connectTime = DateTime.Now;

    /// <summary>
    /// Represents the current direction of the player's snake.
    /// </summary>
    private string snakeDirection = "Up";

    /// <summary>
    /// Keeps track of what ID's should be in the database.
    /// </summary>
    private int snakeCounter = 1;

    /// <summary>
    /// Keeps track of the max snake id in the game used for figuring out what snake to modify in the databse.
    /// </summary>
    private int maxSnakeID = 0;

    /// <summary>
    /// The initial endTime assigned to all players, put as a constant for ease of logic when disconnect multiple players.
    /// </summary>
    private string intitalEndTime = "2004-11-01 8:43:21";

    /// <summary>
    /// Gets or sets the network connection to the game server, used to send and receive data.
    /// </summary>
    public NetworkConnection ServerSnake { get; set; } = new(NullLogger.Instance);

    /// <summary>
    /// Gets or sets a value indicating whether the server's connected.
    /// </summary>
    public bool IsServerConnected { get; set; } = false;

    /// <summary>
    /// Gets or sets the particular error message if there is any.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Handles the network connection to the game server. Establishes a connection,
    /// retrieves initial world information, and continuously receives updates from the server.
    /// </summary>
    /// <param name="world">The World object representing the game state.</param>
    /// <param name="serverUrl">The URL of the server to connect to.</param>
    /// <param name="port">The port number used for the connection.</param>
    /// <param name="name">The name of the snake (player).</param>
    public async void HandleNetwork(World world, string serverUrl, int port, string name)
    {
        {
            try
            {
                ServerSnake.Connect(serverUrl, port);
                IsServerConnected = true;
                connectTime = DateTime.Now;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                IsServerConnected = false;
            }
        }

        var worldJSON = string.Empty;

        // Once connected to the server, proceed with game initialization
        if (ServerSnake.IsConnected)
        {
            ServerSnake.Send(name);
            lock (world)
            {
                world.WorldID = int.Parse(ServerSnake.ReadLine());
                world.Height = int.Parse(ServerSnake.ReadLine());
                world.Width = world.Height;
            }

            // Add a new row to our games table by querying the database.
            gameID = SQLQueries.AddGameAndReturnID();
            snakeCounter = 1;

            // Start a background task to continuously receive updates from the server
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        worldJSON = ServerSnake.ReadLine();

                        // Lock the `world` object to safely update its state with the new data
                        lock (world)
                        {
                            if (worldJSON.Contains("snake") && ServerSnake.IsConnected)
                            {
                                // This parses the worldJSON and explicitly grabs the id, score, and name.
                                string[] splitJson = worldJSON.Split(',');

                                string snakePortion = splitJson[0];
                                string[] splitSnakePortion = snakePortion.Split(':');
                                string stringID = splitSnakePortion[1];
                                int snakeID = int.Parse(stringID);

                                string[] scorePortion = worldJSON.Split("score\":");
                                string[] scorePortionReal = scorePortion[1].Split(",");
                                string stringScore = scorePortionReal[0];
                                int score = int.Parse(stringScore);

                                string[] splitNamePortion = worldJSON.Split("name\":\"");
                                string[] splitAfterNamePortion = splitNamePortion[1].Split("\"");
                                string name = splitAfterNamePortion[0];

                                if (snakeID > maxSnakeID)
                                {
                                    maxSnakeID = snakeID;
                                }

                                // If the snake is new then lets add a row to our database.
                                if (!world.Snakes.ContainsKey(snakeID) && worldJSON.Contains("alive\":true"))
                                {
                                    string enterTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

                                    // Query the database to add a row to the players table.
                                    SQLQueries.AddPlayer(snakeCounter, name, score, enterTime, intitalEndTime, gameID);
                                    snakeCounter++;
                                    world.UpdateWorld(worldJSON);
                                    Debug.WriteLine("World has found a new snake!");
                                }

                                // If the snake already exists in the database then update it if necessary.
                                else if (worldJSON.Contains("alive\":true"))
                                {
                                    int maxScore = world.Snakes[snakeID].MaxScore;

                                    // Update the max score if applicable.
                                    if (score > world.Snakes[snakeID].MaxScore)
                                    {
                                        maxScore = score;

                                        int possibleSnakeToChange = -1 * ((maxSnakeID - (snakeCounter - 1)) - snakeID);
                                        if (possibleSnakeToChange > 0)
                                        {
                                            SQLQueries.UpdatePlayerMaxScore(score, possibleSnakeToChange, gameID);
                                        }
                                        else
                                        {
                                            SQLQueries.UpdatePlayerMaxScore(score, snakeID + 1, gameID);
                                        }
                                    }

                                    world.UpdateWorld(worldJSON);
                                    world.Snakes[snakeID].MaxScore = maxScore;
                                }
                            }
                            else if(ServerSnake.IsConnected)
                            {
                                world.UpdateWorld(worldJSON);
                            }
                        }

                        // If we see that dc is true update this players endtime.
                        if (worldJSON.Contains($"\"dc\":true"))
                        {
                            // This parses the worldJSON and explicitly grabs the id. This code is needed again and not able to be put into a helper method since both cases of the code get different information.
                            string[] splitJson = worldJSON.Split(',');
                            string snakePortion = splitJson[0];
                            string[] splitSnakePortion = snakePortion.Split(':');
                            string stringID = splitSnakePortion[1];
                            int snakeID = int.Parse(stringID);

                            string endTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

                            // Query the database to update the player's leave time.
                            int possibleSnakeToChange = -1 * ((maxSnakeID - (snakeCounter - 1)) - snakeID);
                            if (possibleSnakeToChange > 0)
                            {
                                SQLQueries.UpdateLeaveTimeForPlayer(endTime, possibleSnakeToChange, gameID);
                            }
                            else
                            {
                                SQLQueries.UpdateLeaveTimeForPlayer(endTime, snakeID + 1, gameID);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });
        }
    }

    /// <summary>
    /// Handles key press events from the user, interpreting them as movement commands for the snake.
    /// Sends the corresponding direction to the server if the snake is connected.
    /// </summary>
    /// <param name="keyEvent">The key event string representing the user's key press.</param>
    [JSInvokable]
    public void HandleKeyPress(string keyEvent)
    {
        if (!ServerSnake.IsConnected)
        {
            return;
        }

        if (keyEvent == "ArrowUp" || (keyEvent == "w" && snakeDirection != "DOWN"))
        {
            ServerSnake.Send(@"{""moving"":""up""}");
        }
        else if (keyEvent == "ArrowDown" || (keyEvent == "s" && snakeDirection != "UP"))
        {
            ServerSnake.Send(@"{""moving"":""down""}");
        }
        else if (keyEvent == "ArrowLeft" || (keyEvent == "a" && snakeDirection != "RIGHT"))
        {
            ServerSnake.Send(@"{""moving"": ""left""}");
        }
        else if (keyEvent == "ArrowRight" || (keyEvent == "d" && snakeDirection != "LEFT"))
        {
            ServerSnake.Send(@"{""moving"":""right""}");
        }
    }

    /// <summary>
    /// This method disconnects the server and removes the user's snake from the game world.
    /// </summary>
    /// <param name="world">The World object representing the game state.</param>
    public void HandleDisconnectingClient(World world)
    {
        ServerSnake.Disconnect();
        ServerSnake = new(NullLogger.Instance);
        string endTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

        lock (world)
        {
            foreach (Snake snake in world.Snakes.Values)
            {
                world.Snakes.Remove(snake.snake);
            }

            // Query the data base to update player's leave time when client disconnects.
            SQLQueries.UpdateLeaveTimeAllPlayersInGame(intitalEndTime, endTime, gameID);
        }

        // Query the data base to update game's leave time when client disconnects.
        SQLQueries.UpdateGameEndTime(endTime, gameID);
    }
}
