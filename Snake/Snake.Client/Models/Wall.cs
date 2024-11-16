using System.Text.Json.Serialization;

namespace Snake.Client.Models;

public class Wall
{
	[JsonInclude]
	public int wall {  get; set; }
	[JsonInclude]
	public Point2D p1 { get; set; }
	[JsonInclude]
	public Point2D p2 { get; set; }

    public Wall(int wall, Point2D p1, Point2D p2)
    {
        this.wall = wall;
        this.p1 = p1;
        this.p2 = p2;
    }
}
