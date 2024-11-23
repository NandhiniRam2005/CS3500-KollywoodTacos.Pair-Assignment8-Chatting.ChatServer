// <copyright file="Snake.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace Snake.Client.Models;
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
///     This class represents a player-controlled snake in the Snake game.
///     It includes the snake's unique ID, name, position on the game grid (body and direction),
///     game status (alive, dead, or disconnected), and score. It handles the representation of a snake's state,
///     including movement direction, whether it has died, and if it has joined or left the game.
/// </summary>

/// <summary>
///   Represents a player-controlled snake in the Snake game, including its ID, name, position,
///   score, status, and connection information.
/// </summary>
public class Snake
{
     /// <summary>
    /// Initializes a new instance of the <see cref="Snake"/> class.
    /// </summary>
    /// <param name="snake">The unique ID of the snake.</param>
    /// <param name="name">The name of the snake or player.</param>
    /// <param name="body">The body of the snake, represented as a list of points.</param>
    /// <param name="dir">The direction the snake is moving.</param>
    /// <param name="score">The score of the snake.</param>
    /// <param name="died">Indicates whether the snake has died.</param>
    /// <param name="alive">Indicates whether the snake is alive.</param>
    /// <param name="dc">Indicates whether the snake has disconnected.</param>
    /// <param name="join">Indicates whether the snake has joined the game.</param>
    public Snake(int snake, string name, List<Point2D> body, Point2D dir, int score, bool died, bool alive, bool dc, bool join)
    {
        this.snake = snake;
        this.name = name;
        this.body = body;
        this.dir = dir;
        this.score = score;
        this.died = died;
        this.alive = alive;
        this.dc = dc;
        this.join = join;
    }

    /// <summary>
    /// Gets or sets the unique ID of the snake.
    /// </summary>
    [JsonInclude]
    public int snake { get; set; }

    /// <summary>
    /// Gets or sets the name of the snake/player.
    /// </summary>
    [JsonInclude]
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the body of the snake, represented as a list of points.
    /// Each point indicates a part of the snake's body.
    /// </summary>
    [JsonInclude]
    public List<Point2D> body { get; set; }

    /// <summary>
    /// Gets or sets the direction the snake is moving.
    /// </summary>
    [JsonInclude]
    public Point2D dir { get; set; }

    /// <summary>
    /// Gets or sets the score of the snake.
    /// </summary>
    [JsonInclude]
    public int score { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake has died.
    /// </summary>
    [JsonInclude]
    public bool died { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake is currently alive.
    /// </summary>
    [JsonInclude]
    public bool alive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake has disconnected.
    /// </summary>
    [JsonInclude]
    public bool dc {  get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the snake has joined the game.
    /// </summary>
    [JsonInclude]
    public bool join { get; set; }
}
