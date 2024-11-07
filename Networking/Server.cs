// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace CS3500.Networking;

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
///    Represents a server task that waits for connections on a given
///    port and calls the provided delegate when a connection is made.
/// </summary>

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{
    //private static readonly ILogger _logger;

    static Server()
    {
        //using var loggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder.AddConsole(); // JIM: must nuget add Microsoft.Extensions.Logging.Console and Debug
        //    builder.AddDebug();
        //    builder.SetMinimumLevel(LogLevel.Trace);
        //});
        //_logger = loggerFactory.CreateLogger("Server");
    }

    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   It is run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer( Action<NetworkConnection> handleConnect, int port, ILogger _logger)
    {
        _logger.LogInformation("Attempting to create server");
        TcpListener server = new(IPAddress.Any, port);
        _logger.LogInformation("Successfully created server");
        server.Start();

        // Infinite loop to keep the server running and continuously accept new clients.
        while (true)
        {
            _logger.LogDebug("Attempting to accept new client to server");
            TcpClient client = server.AcceptTcpClient();
            _logger.LogInformation("Successfully accepted new client to server");
            _logger.LogDebug("Attempting to create NetworkConnection for client.");
            NetworkConnection connection = new NetworkConnection( client, _logger);
            _logger.LogDebug("Successfully created NetworkConnection for client");
            _logger.LogDebug("Attempting to create new thread for NetworkConnection for client and start thread.");
            Thread newClient = new Thread(() => handleConnect(connection));
            newClient.Start();
            _logger.LogInformation("Successfully created and started thread for client.");
        }
    }
}
