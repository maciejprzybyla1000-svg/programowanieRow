namespace programowanieRow
{
    public class WorkerArgs
    {
        public Obliczenia.IFunction Function { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public int Steps { get; set; }

        public WorkerArgs(Obliczenia.IFunction func, double start, double end, int steps)
        {
            Function = func;
            Start = start;
            End = end;
            Steps = steps;
        }
    }
}