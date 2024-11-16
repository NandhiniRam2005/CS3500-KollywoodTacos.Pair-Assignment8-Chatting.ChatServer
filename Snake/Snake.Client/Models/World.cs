// Ignore Spelling: json Powerups

using System.Text.Json;

namespace Snake.Client.Models;

public class World
{
    public Dictionary<int, Wall> Walls { get; set; }
    public Dictionary<int, Snake> Snakes { get; set; }
    public Dictionary<int, Powerup> Powerups { get; set; }

    public int WorldSize { get; set; }

    public World() 
    { 
        Walls = new Dictionary<int, Wall>();
        Snakes = new Dictionary<int, Snake>();
        Powerups = new Dictionary<int, Powerup>();
        WorldSize = 0;
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
