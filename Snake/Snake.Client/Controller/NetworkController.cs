using System.Numerics;
using CS3500.Networking;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Snake.Client.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.JSInterop;

namespace Snake.Client.Controller;

public class NetworkController
{
	public NetworkConnection server = new(NullLogger.Instance);
	public string networkStatus = "Waiting For You to Connect";
	private int frameNumberNetwork = 0;
	private DateTime ConnectTime = DateTime.Now;
	private String snakeDirection = "Up";


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
		// Once the server has connected do all this good stuff.
		if (server.IsConnected)
		{
			server.Send(name); // Sends the name of the snake.
			lock (world)
			{
				world.WorldID = int.Parse(server.ReadLine());
				world.Height = int.Parse(server.ReadLine()); // Maybe switch tryParse
				world.Width = world.Height;
			}
			await Task.Run(() =>
			{
				try
				{
					while (true)
					{

						worldJSON = server.ReadLine();

						lock (world) // make a copy in the lock and then do the work on that copy
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
	[JSInvokable]
	public void HandleKeyPress(String keyEvent)
	{
		if (!server.IsConnected)
		{
			return;
		}
		//Code for handing
		//Send the key that was pressed to the server (says in assignment maybe?)
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
