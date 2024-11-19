// Ignore Spelling: json Powerups

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snake.Client.Models;

public class World
{
    public Dictionary<int, Wall> Walls { get; set; }
    public Dictionary<int, Snake> Snakes { get; set; }
    public Dictionary<int, Powerup> Powerups { get; set; }

    [JsonIgnore]
    public int Width { get; set; }
	[JsonIgnore]
	public int Height { get; set; }

    public int WorldID { get; set; }

	public World(Dictionary<int, Wall> Walls, Dictionary<int, Snake> Snakes, Dictionary<int, Powerup> Powerups, int width, int height, int worldID)
	{
        this.Walls = Walls;
        this.Snakes = Snakes;
        this.Powerups = Powerups;
		this.Height = height;
		this.Width = width;
		this.WorldID = worldID;
	}

	public void load(string jsonString)
    {
        if (jsonString.StartsWith(@"{""wall"""))
        {
            Wall wall = JsonSerializer.Deserialize<Wall>(jsonString)!;
            Walls.Add(wall.wall,wall);
 
        }
        else if (jsonString.StartsWith(@"{""power"""))
        {
            Powerup powerUp = JsonSerializer.Deserialize<Powerup>(jsonString)!;
            if (!powerUp.died)
            {
                if (!Powerups.ContainsKey(powerUp.power))
                {
					Powerups.Add(powerUp.power, powerUp);
				}
                else
                {
                    Powerups[powerUp.power] = powerUp;
                }
            }
            else
            {
                Powerups.Remove(powerUp.power);
            }
        }
        else
        {
            Snake snake = JsonSerializer.Deserialize<Snake>(jsonString)!;
            if (!snake.died)
            {
				if (!Snakes.ContainsKey(snake.snake))
				{
					Snakes.Add(snake.snake, snake);
				}
				else
				{
					Snakes[snake.snake] = snake;
				}
			}
            else
            {
                Snakes.Remove(snake.snake);
            }
        }
    }
}
