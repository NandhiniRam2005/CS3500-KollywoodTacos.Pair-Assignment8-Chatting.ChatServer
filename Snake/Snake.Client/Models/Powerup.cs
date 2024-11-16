using System.Text.Json.Serialization;

namespace Snake.Client.Models;

public class Powerup
{
	[JsonInclude]
	public int power {  get; set; }
	[JsonInclude]
	public Point2D loc { get; set; }
	[JsonInclude]
	public bool died { get; set; }
    public Powerup(int power, Point2D loc, bool died)
    {
        this.power = power;
        this.loc = loc;
        this.died = died;
    }
}
