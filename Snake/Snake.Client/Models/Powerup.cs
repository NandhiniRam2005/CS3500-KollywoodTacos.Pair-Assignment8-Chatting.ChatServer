namespace Snake.Client.Models;

public class Powerup
{
    public int power {  get; set; }
    public Point2D loc { get; set; }
    public bool died { get; set; }
    public Powerup(int power, Point2D loc, bool died)
    {
        this.power = power;
        this.loc = loc;
        this.died = died;
    }
}
