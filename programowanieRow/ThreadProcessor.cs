using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace programowanieRow
{
    public class ThreadProcessor : IProcessor
    {
        private List<Thread> threads = new List<Thread>();
        private List<bool> cancellationFlags = new List<bool>();

        public void StartWork(
            Obliczenia.IFunction func,
            double a,
            double b,
            int n,
            Action<int> progressCallback,
            Action<double, bool, long> completedCallback)
        {
            int index = cancellationFlags.Count;
            cancellationFlags.Add(false);

            var stopwatch = Stopwatch.StartNew();

            Thread thread = new Thread(() =>
            {
                try
                {
                    double result = CalkaTrapezyThread(func, a, b, n, progressCallback, index);

                    stopwatch.Stop();

                    if (cancellationFlags[index])
                    {
                        completedCallback(double.NaN, true, stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        completedCallback(result, false, stopwatch.ElapsedMilliseconds);
                    }
                }
                catch (Exception)
                {
                    stopwatch.Stop();
                    completedCallback(double.NaN, true, stopwatch.ElapsedMilliseconds);
                }
            });

            thread.IsBackground = true;
            threads.Add(thread);
            thread.Start();
        }

        private double CalkaTrapezyThread(
            Obliczenia.IFunction func,
            double a,
            double b,
            int n,
            Action<int> progressCallback,
            int index)
        {
            double dx = (b - a) / n;
            double sum = 0.5 * ((double)func.GetY((decimal)a) + (double)func.GetY((decimal)b));

            int lastReported = 0;

            for (int i = 1; i < n; i++)
            {
                if (cancellationFlags[index])
                {
                    return double.NaN;
                }

                double x = a + i * dx;
                sum += (double)func.GetY((decimal)x);

                int progress = (i * 100) / n;
                if (progress > lastReported)
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
            for (int i = 0; i < cancellationFlags.Count; i++)
            {
                cancellationFlags[i] = true;
            }
        }
    }
}