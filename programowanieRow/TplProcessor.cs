using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace programowanieRow
{
    public class TplProcessor : IProcessor
    {
        private List<CancellationTokenSource> cancellationTokens = new List<CancellationTokenSource>();

        public void StartWork(
            Obliczenia.IFunction func,
            double a,
            double b,
            int n,
            Action<int> progressCallback,
            Action<double, bool> completedCallback)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cancellationTokens.Add(cts);

            Task.Run(() =>
            {
                try
                {
                    double result = CalkaTrapezyTPL(func, a, b, n, progressCallback, cts.Token);
                    
                    if (cts.Token.IsCancellationRequested)
                    {
                        completedCallback(double.NaN, true);
                    }
                    else
                    {
                        completedCallback(result, false);
                    }
                }
                catch (OperationCanceledException)
                {
                    completedCallback(double.NaN, true);
                }
            }, cts.Token);
        }

        private double CalkaTrapezyTPL(
            Obliczenia.IFunction func,
            double a,
            double b,
            int n,
            Action<int> progressCallback,
            CancellationToken token)
        {
            double dx = (b - a) / n;
            double sum = 0.5 * ((double)func.GetY((decimal)a) + (double)func.GetY((decimal)b));

            int lastReported = 0;

            for (int i = 1; i < n; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return double.NaN;
                }

                double x = a + i * dx;
                sum += (double)func.GetY((decimal)x);

                int progress = (i * 100) / n;
                if (progress >= lastReported + 10)
                {
                    progressCallback(progress);
                    lastReported = progress;
                }
            }

            progressCallback(100);
            return sum * dx;
        }

        public void CancelAll()
        {
            foreach (var cts in cancellationTokens)
            {
                cts.Cancel();
            }
        }
    }
}