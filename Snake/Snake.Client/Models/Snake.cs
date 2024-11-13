namespace Snake.Client.Models;

public class Snake
{
    public int snake { get; set; }
    public string name { get; set; }
    public List<Point2D> body {get; set;}
    public Point2D dir { get; set;}
    public int score { get; set; }
    public bool died { get; set; }
    public bool alive { get; set; }
    public bool dc {  get; set; }
    public bool join { get; set; }
    public Snake(int id, string name, List<Point2D> body, Point2D dir, int score, bool died, bool alive, bool dc, bool join)
    {
        this.snake = id;
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
