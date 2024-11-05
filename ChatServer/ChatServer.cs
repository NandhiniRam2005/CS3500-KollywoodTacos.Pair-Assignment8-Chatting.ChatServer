// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{
    private static HashSet<NetworkConnection> connections = new HashSet<NetworkConnection>();
    private static HashSet<NetworkConnection> tempConnections = new HashSet<NetworkConnection>();
    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
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