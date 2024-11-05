// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace CS3500.Chatting;

using CS3500.Networking;
using Microsoft.Extensions.Logging;
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
    /// All the connections to our Chat Server.
    /// </summary>
    private static HashSet<NetworkConnection> connections = new HashSet<NetworkConnection>();

    /// <summary>
    /// All the temporary connections to our chat server. It is used to provide a better way of locking.
    /// </summary>
    private static HashSet<NetworkConnection> tempConnections = new HashSet<NetworkConnection>();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000);
        Console.Read(); // don't stop the program.
    }

    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect(NetworkConnection connection)
    {
        // handle all messages until disconnect.
        lock (connections)
        {
            connections.Add(connection);
        }

        bool firstMessage = true;
        string userName = string.Empty;
        connection.Send("SERVER: Enter your name: ");
        try
        {
            while (true)
            {
                var message = connection.ReadLine();
                if (firstMessage)
                {
                    userName = message ?? throw new InvalidOperationException();
                    firstMessage = false;
                    BroadcastMessage("SERVER: " + userName + " has entered the chatroom");
                    continue;
                }

                string fullMessage = userName + ": " + message;

                tempConnections = new HashSet<NetworkConnection>();
                lock (connections)
                {
                    foreach (NetworkConnection connectionInList in connections)
                    {
                        tempConnections.Add(connectionInList);
                    }
                }

                BroadcastMessage(fullMessage);
            }
        }
        catch (Exception)
        {
            connection.Disconnect();
            lock (connections)
            {
                connections.Remove(connection);
            }
        }
    }

    private static void BroadcastMessage(string message)
    {
        foreach (NetworkConnection connection in tempConnections)
        {
            connection.Send(message);
        }
    }
}