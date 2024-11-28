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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static System.Formats.Asn1.AsnWriter;

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
    /// The information necessary for the program to connect to the Database.
    /// </summary>
    public static readonly string ConnectionString = string.Empty;

    public Dictionary<int, int> snakesMaxScores = new Dictionary<int, int>();

    private int gameID;

    static NetworkController()
    {
        var builder = new ConfigurationBuilder();

        builder.AddUserSecrets<NetworkController>();
        IConfigurationRoot configuration = builder.Build();
        var selectedSecrets = configuration.GetSection("Secrets");

        ConnectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "cs3500.eng.utah.edu, 14330",
            InitialCatalog = "F2024_DB_u1432722",
            UserID = "F2024_u1432722",
            Password = "Hanuman12345$",
            ConnectTimeout = 15,
            Encrypt = false,
        }.ConnectionString;
    }

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
        bool addPlayerToDatabase = false;
        int ID = 0;
        int currentMaxScore = 0;
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
            // Add a new row to our games table.
            gameID = 0;
            string startTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

            try
            {
                using SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();

                using SqlCommand command = new SqlCommand($"INSERT INTO Games VALUES ('{startTime}' , '{startTime}')", con);
                using SqlDataReader reader = command.ExecuteReader();

                // HTML stuff??
                while (reader.Read())
                {
                    Debug.WriteLine(
                        "{0} {1}",
                        reader.GetInt32(0),
                        reader.GetString(1));
                }
            }
            catch (SqlException exception)
            {
                Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }

            // Saving our gameID
            try
            {
                using SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();

                using (SqlCommand command = new SqlCommand($"SELECT ID FROM Games WHERE StartTime = '{startTime}' ", con))
                {
                    object result = command.ExecuteScalar(); // Execute query to get a single value

                    if (result != null)
                    {
                        gameID = Convert.ToInt32(result); // Convert the result to an int
                    }
                    else
                    {
                        Debug.WriteLine("No game found for the specified StartTime.");
                    }
                }
            }
            catch (SqlException exception)
            {
                Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }

            // Start a background task to continuously receive updates from the server
            await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("Hello");
                    while (true)
                    {
                        worldJSON = ServerSnake.ReadLine();
                        // Lock the `world` object to safely update its state with the new dat
                        lock (world)
                        {
                            if (worldJSON.Contains("snake") && ServerSnake.IsConnected)
                            {
                                // This parses the worldJSON and explicitly grabs the id and score.
                                string[] splitJson = worldJSON.Split(',');
                                string snakePortion = splitJson[0];
                                string[] splitSnakePortion = snakePortion.Split(':');
                                string stringID = splitSnakePortion[1];
                                int ID = int.Parse(stringID);
                                string[] scorePortion = worldJSON.Split("score\":");
                                string[] scorePortionReal = scorePortion[1].Split(",");
                                string stringScore = scorePortionReal[0];
                                int score = int.Parse(stringScore);
                                string[] splitNamePortion = worldJSON.Split("name\":\"");
                                string[] splitAfterNamePortion = splitNamePortion[1].Split("\"");
                                string name = splitAfterNamePortion[0];

                                // If the snake is new then lets add a row to our database.
                                if (!world.Snakes.ContainsKey(ID) && worldJSON.Contains("alive\":true"))
                                {
                                    Debug.WriteLine(worldJSON);
                                    snakesMaxScores.Add(ID, 0);
                                    string enterTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                                    try
                                    {
                                        using SqlConnection con = new SqlConnection(ConnectionString);

                                        con.Open();

                                        using SqlCommand command = new SqlCommand($"INSERT INTO Players (ID, Name, MaxScore, EnterTime, LeaveTime, GameID) VALUES ('{ID}', '{name}',  '{score}', '{enterTime}', '{enterTime}', '{gameID}' )", con);
                                        using SqlDataReader reader = command.ExecuteReader();

                                        // HTML step.
                                        while (reader.Read())
                                        {
                                            Debug.WriteLine(
                                                "{0} {1}",
                                                reader.GetInt32(0),
                                                reader.GetString(1));
                                        }

                                        addPlayerToDatabase = false;
                                    }
                                    catch (SqlException exception)
                                    {
                                        Debug.Write(worldJSON);
                                        Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
                                    }

                                    // add a row
                                }
                                else if (worldJSON.Contains("alive\":true"))
                                {
                                    // Update the max score if applicable.
                                    //Debug.WriteLine("CurrScore "+ score + " CurrMaxScore: " + currentMaxScore);

                                    if (score > snakesMaxScores[ID])
                                    {
                                        //world.Snakes[ID].MaxScore = score;
                                        snakesMaxScores[ID] = score;
                                        try
                                        {
                                            using SqlConnection con = new SqlConnection(ConnectionString);

                                            con.Open();

                                            using SqlCommand command = new SqlCommand($" UPDATE Players SET MaxScore='{score}' WHERE ID = '{ID}' AND GameID = '{gameID}' ", con);
                                            using SqlDataReader reader = command.ExecuteReader();
                                            Debug.WriteLine("I have occured");
                                            // HTML step.
                                            while (reader.Read())
                                            {
                                                Debug.WriteLine(
                                                    "{0} {1}",
                                                    reader.GetInt32(0),
                                                    reader.GetString(1));
                                            }
                                        }
                                        catch (SqlException exception)
                                        {
                                            Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
                                        }
                                    }
                                }
                            }



                            world.UpdateWorld(worldJSON);

                        }

                        //If we see that dc is true update this players endtime
                        if (worldJSON.Contains($"\"dc\":true"))
                        {
                            string[] splitJson = worldJSON.Split(',');
                            string snakePortion = splitJson[0];
                            string[] splitSnakePortion = snakePortion.Split(':');
                            string stringID = splitSnakePortion[1];
                            int ID = int.Parse(stringID);
                            string[] scorePortion = worldJSON.Split("score\":");
                            string[] scorePortionReal = scorePortion[1].Split(",");
                            string stringScore = scorePortionReal[0];
                            int score = int.Parse(stringScore);
                            string[] splitNamePortion = worldJSON.Split("name\":\"");
                            string[] splitAfterNamePortion = splitNamePortion[1].Split("\"");
                            string name = splitAfterNamePortion[0];
                            string endTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
                            try
                            {
                                using SqlConnection con = new SqlConnection(ConnectionString);

                                con.Open();

                                using SqlCommand command = new SqlCommand($"UPDATE Players SET LeaveTime = '{endTime}' WHERE ID = '{ID}' AND GameID = '{gameID}' ", con);
                                using SqlDataReader reader = command.ExecuteReader();

                                //HTML step.
                                while (reader.Read())
                                {
                                    Debug.WriteLine(
                                        "{0} {1}",
                                        reader.GetInt32(0),
                                        reader.GetString(1));
                                }
                            }
                            catch (SqlException exception)
                            {
                                Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
                            }
                            // NOW LETS CHECK TO SEE IF THEY ARE US (OUR CLIENT) (THIS CHECKS IF THEY DID NOT CLICK DISONNECT AND SIMPLY DID A REFRESH/X OUT
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                //ServerSnake.Disconnect();
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
            Debug.WriteLine("Server is not connected stop pressing me.");
            return;
        }

        if (keyEvent == "ArrowUp" || (keyEvent == "w" && snakeDirection != "DOWN"))
        {
            ServerSnake.Send(@"{""moving"":""up""}");
            //Debug.WriteLine("Up Pressed");
        }
        else if (keyEvent == "ArrowDown" || (keyEvent == "s" && snakeDirection != "UP"))
        {
            ServerSnake.Send(@"{""moving"":""down""}");
            //Debug.WriteLine("Down Pressed");

        }
        else if (keyEvent == "ArrowLeft" || (keyEvent == "a" && snakeDirection != "RIGHT"))
        {
            ServerSnake.Send(@"{""moving"": ""left""}");
            //Debug.WriteLine("Left Pressed");

        }
        else if (keyEvent == "ArrowRight" || (keyEvent == "d" && snakeDirection != "LEFT"))
        {
            ServerSnake.Send(@"{""moving"":""right""}");
            //Debug.WriteLine("Right Pressed");

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
        string endTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");
        lock (world)
        {
            world.Snakes.Remove(world.WorldID);



            try
            {
                using SqlConnection con = new SqlConnection(ConnectionString);

                con.Open();

                using SqlCommand command = new SqlCommand($"UPDATE Players SET LeaveTime = '{endTime}' WHERE ID = '{world.WorldID}' AND GameID = '{gameID}' ", con);
                using SqlDataReader reader = command.ExecuteReader();

                //HTML step.
                while (reader.Read())
                {
                    Debug.WriteLine(
                        "{0} {1}",
                        reader.GetInt32(0),
                        reader.GetString(1));
                }
            }
            catch (SqlException exception)
            {
                Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            }
            // NOW LETS CHECK TO SEE IF THEY ARE US (OUR CLIENT) (THIS CHECKS IF THEY DID NOT CLICK DISONNECT AND SIMPLY DID A REFRESH/X OUT

        }


        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);

            con.Open();

            using SqlCommand command = new SqlCommand($" UPDATE Games SET EndTime='{endTime}' WHERE ID = '{gameID}'", con);
            using SqlDataReader reader = command.ExecuteReader();

            //HTML step.
            while (reader.Read())
            {
                Debug.WriteLine(
                    "{0} {1}",
                    reader.GetInt32(0),
                    reader.GetString(1));
            }
        }
        catch (SqlException exception)
        {
            Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }

        // Now we get in Players where ID = the ID and GameID = the GameID we gonna add a leaveTime AND go to Games and update the endTime where
        // it is the Gane ID (look to your notes for example.
    }
}
