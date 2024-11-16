using System.Text.Json.Serialization;

namespace Snake.Client.Models;

public class Point2D
{
    [JsonInclude]
    public int X { get; set; }
	[JsonInclude]
	public int Y { get; set; }
    public Point2D(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}

