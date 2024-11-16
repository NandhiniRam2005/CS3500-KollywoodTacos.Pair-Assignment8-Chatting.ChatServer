using System.Text.Json.Serialization;

namespace Snake.Client.Models;

public class Snake
{
	[JsonInclude]
	public int snake { get; set; }
	[JsonInclude]
	public string name { get; set; }
	[JsonInclude]
	public List<Point2D> body {get; set;}
	[JsonInclude]
	public Point2D dir { get; set;}
	[JsonInclude]
	public int score { get; set; }
	[JsonInclude]
	public bool died { get; set; }
	[JsonInclude]
	public bool alive { get; set; }
	[JsonInclude]
	public bool dc {  get; set; }
	[JsonInclude]
	public bool join { get; set; }
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
}
