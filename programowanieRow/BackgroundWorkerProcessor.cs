using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace programowanieRow
{
    public class BackgroundWorkerProcessor : IProcessor
    {
        private List<BackgroundWorker> workers = new List<BackgroundWorker>();

        public void StartWork(
            Obliczenia.IFunction func,
            double a,
            double b,
            int n,
            Action<int> progressCallback,
            Action<double, bool, long> completedCallback)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            var stopwatch = Stopwatch.StartNew();

            worker.DoWork += (sender, e) =>
            {
                var args = (WorkerArgs)e.Argument;
                var bgWorker = (BackgroundWorker)sender;
                
                double result = Obliczenia.CalkaTrapezyBG(
                    args.Function,
                    args.Start,
                    args.End,
                    args.Steps,
                    bgWorker
                );
                
                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
                
                e.Result = result;
            };

            worker.ProgressChanged += (sender, e) =>
            {
                progressCallback(e.ProgressPercentage);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                stopwatch.Stop();
                bool cancelled = e.Cancelled || (e.Result is double d && double.IsNaN(d));
                double result = cancelled ? double.NaN : (double)e.Result;
                completedCallback(result, cancelled, stopwatch.ElapsedMilliseconds);
            };

            workers.Add(worker);
            var workerArgs = new WorkerArgs(func, a, b, n);
            worker.RunWorkerAsync(workerArgs);
        }

        public void CancelAll()
        {
            foreach (var worker in workers)
            {
                if (worker.IsBusy)
                {
                    worker.CancelAsync();
                }
            }
        }
    }
}