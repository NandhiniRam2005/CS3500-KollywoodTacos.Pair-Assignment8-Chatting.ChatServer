// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using System.Numerics;
using CS3500.Networking;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Snake.Client.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Snake.Client.Controller;

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

///BLABLA
public class NetworkController
{
	/// <summary>
	/// The network connection to the game server, used to send and receive data.
	/// </summary>
	public NetworkConnection server = new(NullLogger.Instance);

	/// <summary>
	/// A string indicating the current status of the network connection.
	/// </summary>
	public string networkStatus = "Waiting For You to Connect";

	/// <summary>
	/// Stores the time when the connection was established.
	/// </summary>
	private DateTime ConnectTime = DateTime.Now;

	/// <summary>
	/// Represents the current direction of the player's snake.
	/// </summary>
	private String snakeDirection = "Up";

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

		await Task.Run(() =>
		{

			try
			{
				server.Connect(serverUrl, port);
				networkStatus = "Connected";
				ConnectTime = DateTime.Now;
			}
			catch (Exception e)
			{
				networkStatus = "Error";
			}
		});

		var worldJSON = "";

		// Once connected to the server, proceed with game initialization
		if (server.IsConnected)
		{
			server.Send(name);
			lock (world)
			{
				world.WorldID = int.Parse(server.ReadLine());
				world.Height = int.Parse(server.ReadLine()); // Maybe switch tryParse???
				world.Width = world.Height;
			}

			// Start a background task to continuously receive updates from the server
			await Task.Run(() =>
			{
				try
				{
					while (true)
					{

						worldJSON = server.ReadLine();

						// Lock the `world` object to safely update its state with the new dat
						lock (world)
						{
							world.load(worldJSON);
						}
					}
				}
				catch (Exception e)
				{

				}


				server.Disconnect();
			});
		}
	}

	/// <summary>
	/// Handles key press events from the user, interpreting them as movement commands for the snake.
	/// Sends the corresponding direction to the server if the snake is connected.
	/// </summary>
	/// <param name="keyEvent">The key event string representing the user's key press.</param>
	[JSInvokable]
	public void HandleKeyPress(String keyEvent)
	{
		if (!server.IsConnected)
		{
			return;
		}

		if (keyEvent == "ArrowUp" || keyEvent == "w" && snakeDirection != "DOWN")
		{
			server.Send(@"{""moving"":""up""}");
		}
		else if (keyEvent == "ArrowDown" || keyEvent == "s" && snakeDirection != "UP")
		{
			server.Send(@"{""moving"":""down""}");
		}
		else if (keyEvent == "ArrowLeft" || keyEvent == "a" && snakeDirection != "RIGHT")
		{
			server.Send(@"{""moving"": ""left""}");
		}
		else if (keyEvent == "ArrowRight" || keyEvent == "d" && snakeDirection != "LEFT")
		{
			server.Send(@"{""moving"":""right""}");
		}
	}
}
