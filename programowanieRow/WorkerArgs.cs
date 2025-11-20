namespace programowanieRow;

public class WorkerArgs
{
    public Obliczenia.IFunction Function { get; set; }
    public double Start { get; set; }
    public double End {get; set;}
    public int Steps { get; set; } = 100000;
}