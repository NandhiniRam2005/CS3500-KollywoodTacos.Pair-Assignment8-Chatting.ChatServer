// <copyright file="Powerup.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Ignore Spelling: Powerup loc
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
///     This class defines a power-up object in the Snake game.
///     Each power-up is identified by a unique ID and has a specific location on the map.
///     The class includes properties to indicate if the power-up is still active or if it has been eaten (died).
/// </summary>
/// </summary>

/// <summary>
/// Represents a power-up in the Snake game, with a unique ID, location, and status indicating if it has been eaten (died) or still active.
/// </summary>
public class Powerup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Powerup"/> class.
    /// </summary>
    /// <param name="power">The unique ID for the power-up.</param>
    /// <param name="loc">The location of the power-up.</param>
    /// <param name="died">Indicates whether the power-up has been eaten (died) or still active.</param>
    public Powerup(int power, Point2D loc, bool died)
    {
        this.power = power;
        this.loc = loc;
        this.died = died;
    }

    /// <summary>
    /// Gets or sets the unique power up ID for the power-up.
    /// </summary>
    [JsonInclude]
    public int power {  get; set; }

    /// <summary>
    /// Gets or sets the location of the power-up.
    /// </summary>
    [JsonInclude]
    public Point2D loc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the power-up has been eaten (died) or still active".
    /// </summary>
    [JsonInclude]
    public bool died { get; set; }
}
