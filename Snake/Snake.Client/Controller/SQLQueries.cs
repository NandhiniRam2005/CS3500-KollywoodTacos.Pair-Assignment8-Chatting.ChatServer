// <copyright file="SQLQueries.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
namespace Snake.Client.Controller;

using System.Diagnostics;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Snake.Client.Models;
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
///    This class contains commonly used SQLQueries in the context of a snake game and what would be needed from
///    a snake games data base. Such as adding players, adding games, or updating certain properties of our database
///    via SQL queries.
/// </summary>
public class SQLQueries
{
    /// <summary>
    /// The information necessary for the program to connect to the Database.
    /// </summary>
    public static readonly string ConnectionString = string.Empty;

    /// <summary>
    /// Initializes static members of the <see cref="SQLQueries"/> class.
    /// Constructor to initialize the database connection string using user secrets.
    /// </summary>
    static SQLQueries()
    {
        var builder = new ConfigurationBuilder();

        builder.AddUserSecrets<SQLQueries>();
        IConfigurationRoot configuration = builder.Build();
        var selectedSecrets = configuration.GetSection("Secrets");

        ConnectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "cs3500.eng.utah.edu, 14330",
            InitialCatalog = "F2024_DB_u1432722",
            UserID = selectedSecrets["UserID"],
            Password = selectedSecrets["UserPassword"],
            ConnectTimeout = 15,
            Encrypt = false,
        }.ConnectionString;
    }

    /// <summary>
    /// Adds a game to our database and returns the id of that game that was added.
    /// </summary>
    /// <returns> The id of the game that was just added.</returns>
    public static int AddGameAndReturnID()
    {
        string startTime = DateTime.Now.ToString("yyyy-MM-dd H:mm:ss");

        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlCommand command = new SqlCommand($"INSERT INTO Games VALUES ('{startTime}' , '{startTime}');  SELECT SCOPE_IDENTITY();", con);

            return Convert.ToInt32(command.ExecuteScalar());
        }
        catch (SqlException exception)
        {
            Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Adds a player to our games database.
    /// </summary>
    /// <param name="snakeID"> The id of the new player/snake. </param>
    /// <param name="name"> The name of said player/snake. </param>
    /// <param name="score"> The initial score of said player (0).</param>
    /// <param name="enterTime">The time when the player/snake joined.</param>
    /// <param name="leaveTime"> The initial leave time which is needed by SQL.</param>
    /// <param name="gameID"> The id of the game for which the snake joined.</param>
    public static void AddPlayer(int snakeID, string name, int score, string enterTime, string leaveTime, int gameID)
    {
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlCommand command = new SqlCommand($"INSERT INTO Players (ID, Name, MaxScore, EnterTime, LeaveTime, GameID) VALUES ('{snakeID}', '{name}',  '{score}', '{enterTime}', '{leaveTime}', '{gameID}' )", con);
            using SqlDataReader reader = command.ExecuteReader();
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

    /// <summary>
    /// Updates the max score for a player.
    /// </summary>
    /// <param name="score">The new max score for the player.</param>
    /// <param name="snakeID"> The id of the snake to update.</param>
    /// <param name="gameID">The id where the snake's max score should be updated.</param>
    public static void UpdatePlayerMaxScore(int score, int snakeID, int gameID)
    {
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlCommand command = new SqlCommand($"UPDATE Players SET MaxScore='{score}' WHERE ID = '{snakeID}' AND GameID = '{gameID}' ", con);
            using SqlDataReader reader = command.ExecuteReader();
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

    /// <summary>
    /// Updates the leave time for a specific player that has just left a game.
    /// </summary>
    /// <param name="endTime"> The given end time/leave time.</param>
    /// <param name="snakeID"> The id of that specific snake/player.</param>
    /// <param name="gameID"> The id of that players game.</param>
    public static void UpdateLeaveTimeForPlayer(string endTime, int snakeID, int gameID )
    {
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlCommand command = new SqlCommand($"UPDATE Players SET LeaveTime = '{endTime}' WHERE ID = '{snakeID}' AND GameID = '{gameID}' ", con);
            using SqlDataReader reader = command.ExecuteReader();
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

    /// <summary>
    /// Updates the end time of the players who were part of a game that was just ended therefore all games have ended.
    /// </summary>
    /// <param name="initialEndTime"> The initial end time of each player used to see if the endtime of a player needs to be updated.</param>
    /// <param name="endTime"> The given end time for the players. </param>
    /// <param name="gameID">The gameID of the game that just ended.</param>
    public static void UpdateLeaveTimeAllPlayersInGame(string initialEndTime, string endTime, int gameID)
    {
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlCommand command = new SqlCommand($"UPDATE Players SET LeaveTime = '{endTime}' WHERE GameID = '{gameID}' AND LeaveTime = '{initialEndTime}' ", con);
            using SqlDataReader reader = command.ExecuteReader();
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

    /// <summary>
    /// Updates the end time of the given game.
    /// </summary>
    /// <param name="endTime"> The given end time.</param>
    /// <param name="gameID"> The id of the game that has just ended.</param>
    public static void UpdateGameEndTime(string endTime, int gameID)
    {
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using SqlCommand command = new SqlCommand($" UPDATE Games SET EndTime='{endTime}' WHERE ID = '{gameID}'", con);
            using SqlDataReader reader = command.ExecuteReader();
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

    /// <summary>
    /// Gets all the games in a database and stores it's html in a stringbuilder.
    /// </summary>
    /// <returns> All the games in the database in a stringbuilder.</returns>
    public static StringBuilder GetAllGamesInDataBase()
    {
        StringBuilder gamesTableHTML = new StringBuilder();
        try
        {
            // Establish connection to the database and retrieve game information.
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using (SqlCommand command = new SqlCommand($"SELECT * FROM Games ", con))
            {
                // Reads the data returned by the query and appends it to the HTML table.
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gamesTableHTML.Append("<tr>");
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    gamesTableHTML.Append($"<td><a href=\"/games?gid={reader[i].ToString()}\">{reader[i].ToString()}</a></td>");
                                    break;
                                case 1:
                                case 2:
                                    gamesTableHTML.Append($"<td>{reader[i].ToString()}</td>");
                                    break;
                                default:
                                    break;
                            }
                        }

                        gamesTableHTML.Append("</tr>");
                    }
                }
            }
        }
        catch (SqlException exception)
        {
            Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }

        return gamesTableHTML;
    }

    /// <summary>
    /// Gets all the players in a game and returns that as a Stringbuilder in its html form.
    /// </summary>
    /// <param name="gameID">The id for which we are getting players from.</param>
    /// <returns> The players in a specific game.</returns>
    public static StringBuilder GetPlayersInGame(string gameID)
    {
        StringBuilder playersTableHTML = new StringBuilder();
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using (SqlCommand command = new SqlCommand($"SELECT * FROM Players WHERE GameID = '{gameID}'", con))
            {
                playersTableHTML = new StringBuilder();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        playersTableHTML.Append("<tr>");
                        for (int i = 0; i < reader.FieldCount - 1; i++)
                        {
                            playersTableHTML.Append($"<td>{reader[i].ToString()}</td>");
                        }

                        playersTableHTML.Append("</tr>");
                    }
                }
            }
        }
        catch (SqlException exception)
        {
            Debug.WriteLine($"Error in SQL connection:\n   - {exception.Message}");
        }

        return playersTableHTML;
    }
}
