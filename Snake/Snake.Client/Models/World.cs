// <copyright file="World.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Ignore Spelling: json powerups
namespace Snake.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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
///     This class represents the game world in the Snake game.
///     The class maintains collections of walls, snakes, and power-ups. It supports JSON deserialization
///     to update the game state constantly, facilitating the addition, update, or removal of objects
///     based on the game's current state.
/// </summary>

/// <summary>
///   Represents the game world in the Snake game, including collections of walls, snakes, and power-ups.
///   The class supports updates to the game state using JSON deserialization.
/// </summary>
public class World
{
     /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class.
    /// </summary>
    /// <param name="walls">A dictionary of walls in the game world.</param>
    /// <param name="snakes">A dictionary of snakes in the game world.</param>
    /// <param name="powerups">A dictionary of power-ups in the game world.</param>
    /// <param name="width">The width of the game world.</param>
    /// <param name="height">The height of the game world.</param>
    /// <param name="worldID">The unique ID of the world.</param>
    public World(Dictionary<int, Wall> walls, Dictionary<int, Snake> snakes, Dictionary<int, Powerup> powerups, int width, int height, int worldID)
    {
        this.Walls = walls;
        this.Snakes = snakes;
        this.Powerups = powerups;
        this.Height = height;
        this.Width = width;
        this.WorldID = worldID;
    }

    /// <summary>
    /// Gets or sets a dictionary of walls in the game world.
    /// The key is the wall's unique ID, and the value is the Wall object.
    /// </summary>
    public Dictionary<int, Wall> Walls { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of snakes in the game world.
    /// The key is the snake's unique ID, and the value is the Snake object.
    /// </summary>
    public Dictionary<int, Snake> Snakes { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of power-ups in the game world.
    /// The key is the power-up's unique ID, and the value is the Powerup object.
    /// </summary>
    public Dictionary<int, Powerup> Powerups { get; set; }

    /// <summary>
    /// Gets or sets the width of the game world.
    /// This property is ignored during JSON serialization.
    /// </summary>
    [JsonIgnore]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the game world.
    /// This property is ignored during JSON serialization.
    /// </summary>
    [JsonIgnore]
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the unique ID of the world.
    /// </summary>
    public int WorldID { get; set; }

    /// <summary>
    /// Loads a game object from a JSON string and updates the world state accordingly.
    /// If the JSON represents a wall, it adds the wall to the walls dictionary.
    /// If the JSON represents a power-up, it updates or removes it from the powerups dictionary.
    /// If the JSON represents a snake, it updates or removes it from the snakes dictionary.
    /// </summary>
    /// <param name="jsonString">A JSON string representing a wall, power-up, or snake.</param>
    public void UpdateWorld(string jsonString)
    {
        if (jsonString.StartsWith(@"{""wall"""))
        {
            Wall wall = JsonSerializer.Deserialize<Wall>(jsonString)!;
            try
            {
                Walls.Add(wall.wall, wall);
            }
            catch
            {
            }
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
