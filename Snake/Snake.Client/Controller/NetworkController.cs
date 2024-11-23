// <copyright file="NetworkController.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace Snake.Client.Controller;

using System.Numerics;
using CS3500.Networking;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Snake.Client.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;
using Snake.Client.Pages;
using System.Diagnostics;

/// <summary>
/// Author:    Joel Rodriguez,  Nandhini Ramanathan, and Professor Jim.
/// Partner:   None
/// Date:      November 22, 2024
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
    /// Stores the time when the connection was established.
    /// </summary>
    private DateTime connectTime = DateTime.Now;

    /// <summary>
    /// Represents the current direction of the player's snake.
    /// </summary>
    private string snakeDirection = "Up";

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

            // Start a background task to continuously receive updates from the server
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        worldJSON = ServerSnake.ReadLine();

                        // Lock the `world` object to safely update its state with the new dat
                        lock (world)
                        {
                            world.UpdateWorld(worldJSON);
                        }
                    }
                }
                catch
                {
                }

                ServerSnake.Disconnect();
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
    /// <param name="world">The World object representing the game state.</param>
    /// </summary>
    public void HandleDisconnectingClient(World world)
    {
        ServerSnake.Disconnect();
        ServerSnake = new(NullLogger.Instance);

        lock (world)
        {
            world.Snakes.Remove(world.WorldID);
        }
    }
}
