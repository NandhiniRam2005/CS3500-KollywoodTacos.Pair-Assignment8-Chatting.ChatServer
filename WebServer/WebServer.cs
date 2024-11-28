namespace CS3500.WebServer;

using System.Diagnostics;
using CS3500.Networking;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;

public partial class WebServer
{
    /// <summary>
    /// A logger for recording server events, warnings, and errors.
    /// </summary>
    private static readonly ILogger<WebServer> _logger;

    /// <summary>
    /// Stores all the connections to our Chat Server.
    /// </summary>
    private static HashSet<NetworkConnection> connections = new HashSet<NetworkConnection>();

    /// <summary>
    /// All the temporary connections to our chat server. It is used to provide a better way of locking.
    /// </summary>
    private static HashSet<NetworkConnection> tempConnections = new HashSet<NetworkConnection>();

    /// <summary>
    /// The information necessary for the program to connect to the Database.
    /// </summary>
    public static readonly string ConnectionString = string.Empty;

    /// <summary>
    /// Initializes static members of the <see cref="ChatServer"/> class.
    /// A static constructor for ChatServer to initialize the logger.
    /// </summary>
    /// </summary>
    static WebServer()
    {
        var builder = new ConfigurationBuilder();

        builder.AddUserSecrets<WebServer>();
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

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        _logger = loggerFactory.CreateLogger<WebServer>();
    }

    private static void Main(string[] args)
    {
        Server.StartServer(HandleConnect, 11_000, _logger);

    }

    private static void HandleConnect(NetworkConnection connection)
    {
        Debug.WriteLine("someone wants to talk");
        string line = connection.ReadLine();
        Debug.WriteLine($"Their request: {line}");
        StringBuilder gamesTableHTML = new StringBuilder();
        StringBuilder playersTableHTML = new StringBuilder();
        string Website = "HTTP/1.1 200 OK\r\nConnection:close\r\nContent-Type:text/html;charset=UTF-8\r\nContent-Length:";
        string html = "<!DOCTYPE html><html lang=\"en\"><head><title>Snake Games Database</title><style>body{font-family:Arial,sans-serif;background-color:#f9f9f9;color:#333;text-align:center;margin:0;padding:0;display:flex;justify-content:center;align-items:center;height:100vh;}h3{color:#0056b3;margin-bottom:20px;}a{display:inline-block;background-color:#0056b3;color:white;text-decoration:none;padding:10px 20px;border-radius:5px;font-size:16px;transition:background-color 0.3s ease;}a:hover{background-color:#003d7a;}</style></head><body><div><h3>Welcome to the Snake Games Database!</h3><a href=\"/games\">View Games</a></div></body></html>\n";

        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            using (SqlCommand command = new SqlCommand($"SELECT * FROM Games ", con))
            {
                gamesTableHTML = new StringBuilder();

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

        if (line.Equals("GET / HTTP/1.1"))
        {
            Website = Website + html.Length + "\r\n\r\n";
            connection.Send(Website + html);
        }
        if (line.Equals("GET /games HTTP/1.1"))
        {
            html = "<html><h3>List of all Games</h3><style>body{font-family:Arial,sans-serif;margin:20px;background-color:#f9f9f9;color:#333;}h3{color:#0056b3;}table{border-collapse:collapse;width:100%;margin-top:20px;background-color:#fff;box-shadow:0 4px 8px rgba(0,0,0,0.1);}th,td{border:1px solid #ddd;padding:8px;text-align:left;}th{background-color:#0056b3;color:white;}tr:nth-child(even){background-color:#f2f2f2;}tr:hover{background-color:#ddd;}a{text-decoration:none;color:#0056b3;}a:hover{text-decoration:underline;}</style>\n<table border=\"1\"><thead><tr><td>ID</td><td>Start</td><td>End</td></tr></thead><tbody>" + gamesTableHTML.ToString() + "</tbody></table></html>";
            Website = Website + html.Length + "\r\n\r\n";
            connection.Send(Website + html);
        }
        if (line.Contains("gid="))
        {
            string[] splitRequest = line.Split("gid=");
            string[] idPortion = splitRequest[1].Split("HTTP");
            string gameID = idPortion[0];

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
            html = $"<html><h3>Stats for Game {gameID}</h3><style>body{{font-family:Arial,sans-serif;margin:20px;background-color:#f9f9f9;color:#333;}}h3{{color:#0056b3;}}table{{border-collapse:collapse;width:100%;margin-top:20px;background-color:#fff;box-shadow:0 4px 8px rgba(0,0,0,0.1);}}th,td{{border:1px solid #ddd;padding:8px;text-align:left;}}th{{background-color:#0056b3;color:white;}}tr:nth-child(even){{background-color:#f2f2f2;}}tr:hover{{background-color:#ddd;}}a{{text-decoration:none;color:#0056b3;}}a:hover{{text-decoration:underline;}}</style><table border=\"1\"><thead><tr><td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td></tr></thead><tbody>" + playersTableHTML.ToString() + "</tbody></table></html>";
            Website = Website + html.Length + "\r\n\r\n";
            connection.Send(Website + html);
        }
        // GET /games HTTP/1.1
        // GET /games?gid= HTTP/1.1


    }
}

