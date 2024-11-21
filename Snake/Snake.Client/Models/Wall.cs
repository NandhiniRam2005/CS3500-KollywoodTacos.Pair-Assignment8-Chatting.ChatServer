// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;
namespace Snake.Client.Models;

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
///     This class defines a wall in the Snake game.
///     A wall is represented by a unique wall ID and two endpoints (p1 and p2),
///     each of which is a Point2D object.
/// </summary>

/// <summary>
/// Represents a wall in the Snake game, defined by two endpoints and a unique wall ID.
/// </summary>
public class Wall
{
	/// <summary>
	/// Gets or sets the unique wall ID for the wall.
	/// </summary>
	[JsonInclude]
	public int wall {  get; set; }

	/// <summary>
	/// Gets or sets one endpoint of the wall.
	/// </summary>
	[JsonInclude]
	public Point2D p1 { get; set; }

	/// <summary>
	/// Gets or sets another endpoint of the wall.
	/// </summary>
	[JsonInclude]
	public Point2D p2 { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="Wall"/> class.
	/// </summary>
	/// <param name="wall">The unique wall ID for the wall.</param>
	/// <param name="p1">One of the wall.</param>
	/// <param name="p2">Another endpoint of the wall.</param>
	public Wall(int wall, Point2D p1, Point2D p2)
    {
        this.wall = wall;
        this.p1 = p1;
        this.p2 = p2;
    }
}
