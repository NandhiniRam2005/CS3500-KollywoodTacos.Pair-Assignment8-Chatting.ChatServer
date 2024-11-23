// <copyright file="Point2D.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

namespace Snake.Client.Models;

using System.Drawing;
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
///      This class is a simple data structure representing a 2D point with integer coordinates (X, Y).
///      This class is primarily used to represent positions on the game grid in the Snake game, such as the
///      coordinates of walls, power-ups, or the snake's location.
/// </summary>

/// <summary>
/// Represents a 2D point with integer coordinates in the Snake game.
/// This class is used for positions on the game grid.
/// </summary>
public class Point2D
{
     /// <summary>
    /// Initializes a new instance of the <see cref="Point2D"/> class.
    /// </summary>
    /// <param name="x">The X-coordinate of the point.</param>
    /// <param name="y">The Y-coordinate of the point.</param>
    public Point2D(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Gets or sets the X-coordinate of the point.
    /// </summary>
    [JsonInclude]
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y-coordinate of the point.
    /// </summary>
    [JsonInclude]
    public int Y { get; set; }
}