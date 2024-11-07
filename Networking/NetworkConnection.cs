// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace CS3500.Networking;

using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
///     This class encapsulates a network connection, managing the core components of a TCP connection such as the client,
///     reader, and writer.It provides methods for connecting, disconnecting, sending, and receiving messages over the network.
///     This class also integrates logging to track connection status, data transmission, and errors.
/// </summary>

/// <summary>
///   Wraps the StreamReader/Writer/TcpClient together so we
///   don't have to keep creating all three for network actions.
/// </summary>
public sealed class NetworkConnection : IDisposable
{
    /// <summary>
    /// The logger which will be used to log our logging for logging purposes (particularly for logging and logging other things).
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// The client responsible for managing the underlying network connection. The connection/socket abstraction.
    /// </summary>
    private TcpClient _tcpClient = new();

    /// <summary>
    /// Reader to read incoming data from the connection.
    /// </summary>
    private StreamReader? _reader = null;

    /// <summary>
    /// Writer to send data over the connection.
    /// </summary>
    private StreamWriter? _writer = null;

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.
    ///   </para>
    /// </summary>
    /// <param name="tcpClient">
    ///   An already existing TcpClient.
    /// </param>
    /// <param name="logger"> The logging interface. </param>
    public NetworkConnection(TcpClient tcpClient, ILogger logger)
    {
        _logger = logger;

        _tcpClient = tcpClient;
        if (IsConnected)
        {
            // Only establish the reader/writer if the provided TcpClient is already connected.
            _reader = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8);
            _writer = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkConnection"/> class.
    /// </summary>
    /// <para>
    /// Create a NetworkConnection object. The TCP client will be unconnected at the start.
    /// </para>
    /// <param name="logger">
    /// The logging interface.
    /// </param>
    public NetworkConnection(ILogger logger)
        : this(new TcpClient(), logger)
    {
    }

    /// <summary>
    /// Gets a value indicating whether the socket is connected.
    /// </summary>
    public bool IsConnected
    {
        get
        {
            return _tcpClient.Connected;
        }
    }

    /// <summary>
    ///   Try to connect to the given host:port.
    /// </summary>
    /// <param name="host"> The URL or IP address, e.g., www.cs.utah.edu, or  127.0.0.1. </param>
    /// <param name="port"> The port, e.g., 11000. </param>
    public void Connect(string host, int port)
    {
        try
        {
            _logger.LogDebug("Attempting to connect to host and port.");
            _tcpClient.Connect(host, port);
            _logger.LogDebug("Successfully connected to host and port.");
            _reader = new StreamReader(_tcpClient.GetStream(), Encoding.UTF8);
            _writer = new StreamWriter(_tcpClient.GetStream(), Encoding.UTF8) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        }
        catch
        {
            _logger.LogError("Client failed to connect which resulted in error. Possible reasons could be loss of Internet connection.");
        }
    }

    /// <summary>
    ///   Sends a message to the remote server.  If the <paramref name="message"/> contains
    ///   new lines, these will be treated on the receiving side as multiple messages.
    ///   This method should attach a newline to the end of the <paramref name="message"/>
    ///   (by using WriteLine).
    ///   If this operation can not be completed (e.g. because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <param name="message"> The string of characters to send. </param>
    public void Send(string message)
    {
        try
        {
            _logger.LogDebug("Attempting to write message.");
            _writer!.WriteLine(message);
            _logger.LogDebug("Successfully wrote message.");
        }
        catch
        {
            _logger.LogWarning("Failed to write message. Threw InvalidOperationException");
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    ///   Read a message from the remote side of the connection.  The message will contain
    ///   all characters up to the first new line. See <see cref="Send"/>.
    ///   If this operation can not be completed (e.g. because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <returns> The contents of the message. </returns>
    public string ReadLine()
    {
        try
        {
            if (!IsConnected)
            {
                _logger.LogWarning("Failed to read message. Threw InvalidOperationException due to client no longer being connected");
                throw new InvalidOperationException();
            }

            _logger.LogDebug("Attempting to read message.");
            string message = _reader!.ReadLine() ?? throw new InvalidOperationException();
            _logger.LogDebug("Successfully read message.");

            return message;
        }
        catch
        {
            _logger.LogWarning("Failed to read message. Threw InvalidOperationException");
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    ///   If connected, disconnect the connection and clean
    ///   up (dispose) any streams.
    /// </summary>
    public void Disconnect()
    {
        _logger.LogDebug("Checking to see if client is still connected.");
        if (IsConnected)
        {
            _logger.LogDebug("Client is still connected. Attempting to dispose and disconnect.");
            _writer?.Dispose();
            _reader?.Dispose();
            _tcpClient.Dispose();
        }

        _logger.LogDebug("Successfully disposed and disconnected.");
    }

    /// <summary>
    ///   Automatically called with a using statement (see IDisposable).
    /// </summary>
    public void Dispose()
    {
        Disconnect();
    }
}
