// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace CS3500.Chatting;

using CS3500.Networking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;

/// <summary>
/// Author:    Joel Rodriguez,  Nandhini Ramanathan, and Professor Jim.
/// Partner:   None
/// Date:      November 1, 2024
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
///      A simple ChatServer that handles clients at the same time using threading and is able to broadcast
///      messages across the server.
/// </summary>

/// <summary>
///   A simple ChatServer that handles clients at the same time using threading and is able to broadcast
///   messages across the server.
/// </summary>
public partial class ChatServer
{
    /// <summary>
    /// A logger for recording server events, warnings, and errors. 
    /// </summary>
    private static readonly ILogger<ChatServer>? _logger;

    /// <summary>
    /// Stores all the connections to our Chat Server.
    /// </summary>
    private static HashSet<NetworkConnection> connections = new HashSet<NetworkConnection>();

    /// <summary>
    /// All the temporary connections to our chat server. It is used to provide a better way of locking.
    /// </summary>
    private static HashSet<NetworkConnection> tempConnections = new HashSet<NetworkConnection>();

    /// <summary>
    /// Initializes static members of the <see cref="ChatServer"/> class.
    /// A static constructor for ChatServer to initialize the logger.
    /// </summary>
    /// </summary>
    static ChatServer()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        _logger = loggerFactory.CreateLogger<ChatServer>();
    }

    /// <summary>
    ///   Entry point for the ChatServer program, starting the server on port 11000.
    /// </summary>
    /// <param name="args"> ignored. </param>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    /// Handles new client connections by receiving and responding to messages in a loop.
    /// </summary>
    private static void HandleConnect(NetworkConnection connection)
    {
        // Add the new connection to the set of active connections. Locks used to avoid race conditions.
        _logger?.LogDebug("Locking connections backing storage.");
        lock (connections)
        {
            connections.Add(connection);
        }

        _logger?.LogDebug("Unlocked connections backing storage.");

        bool firstMessage = true;
        string userName = string.Empty;
        connection.Send("SERVER: Enter your name: ");
        try
        {
            while (true)
            {
                // Read client message and handle username if it's the first message
                var message = connection.ReadLine();
                if (firstMessage)
                {
                    userName = message ?? throw new InvalidOperationException();
                    firstMessage = false;
                    _logger?.LogInformation(userName + " has joined the server!");
                    _logger?.LogInformation("Broadcasting message to clients");
                    BroadcastMessage("SERVER: " + userName + " has entered the chat room");
                    _logger?.LogInformation("Successfully broad casted message to clients");
                    continue;
                }

                string fullMessage = userName + ": " + message;

                tempConnections = new HashSet<NetworkConnection>();

                // Copy connections to tempConnections for broadcasting, applying a better strategy for locking.
                _logger?.LogDebug("Locking connections backing storage.");
                lock (connections)
                {
                    foreach (NetworkConnection connectionInList in connections)
                    {
                        tempConnections.Add(connectionInList);
                    }
                }

                _logger?.LogDebug("Unlocked connections backing storage.");
                _logger?.LogInformation($"Broadcasting message: {fullMessage} from {userName} to clients");
                BroadcastMessage(fullMessage);
                _logger?.LogInformation("Successfully broad casted message to clients");
            }
        }
        catch (Exception)
        {
            // Dispose the disconnected client
            _logger?.LogWarning("Client has disconnected and messages could not be sent.");
            _logger?.LogDebug("Attempting to disconnect connection to said client.");
            connection.Disconnect();
            _logger?.LogDebug("Successfully disconnected connection to said client.");
            _logger?.LogDebug("Locking connections backing storage.");
            lock (connections)
            {
                connections.Remove(connection);
            }

            _logger?.LogDebug("Unlocked connections backing storage.");
        }
    }

    /// <summary>
    /// Sends a message to all currently connected clients in the tempConnections set.
    /// </summary>
    /// <param name="message">The message to broadcast.</param>
    private static void BroadcastMessage(string message)
    {
        foreach (NetworkConnection connection in tempConnections)
        {
            connection.Send(message);
        }
    }
}