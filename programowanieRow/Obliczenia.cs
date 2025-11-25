using System;
using System.ComponentModel;

namespace programowanieRow
{
    public class Obliczenia
    {
        public interface IFunction
        {
            decimal GetY(decimal x);
        }

        private static double Funkcja1(double x)
        {
            return 2 * x + 2 * x * x;
        }

        private static double Funkcja2(double x)
        {
            return 2 * x * x;
        }

        private static double Funkcja3(double x)
        {
            return 2 * x - 3;
        }

        public static double ObliczFunkcje(int wybor, double x)
        {
            switch (wybor)
            {
                case 1:
                    return Funkcja1(x);
                case 2:
                    return Funkcja2(x);
                case 3:
                    return Funkcja3(x);
                default:
                    throw new ArgumentException("Niepoprawny wybór funkcji!");
            }
        }
        
        public static double CalkaTrapezy(int wybor, double a, double b, int n)
        {
            double sum = 0;
            double dx = (b - a) / n;

            sum = 0.5 * (ObliczFunkcje(wybor, a) + ObliczFunkcje(wybor, b));

            for (int i = 1; i < n; i++)
            {
                double x = a + i * dx;
                sum += ObliczFunkcje(wybor, x);
            }

            return sum * dx;
        }
        
        public static double CalkaTrapezyBG(
            IFunction func,
            double a,
            double b,
            int n,
            BackgroundWorker worker)
        {
            double dx = (b - a) / n;
            double sum = 0.5 * ((double)func.GetY((decimal)a) + (double)func.GetY((decimal)b));

            int lastReported = 0;

            for (int i = 1; i < n; i++)
            {
                if (worker.CancellationPending)
                {
                    return double.NaN;
                }

                double x = a + i * dx;
                sum += (double)func.GetY((decimal)x);

                int progress = (i * 100) / n;
                if (progress >= lastReported + 10)
                {
                    worker.ReportProgress(progress);
                    lastReported = progress;
                }
            }

            worker.ReportProgress(100);
            return sum * dx;
        }

        public class WorkerFunction : IFunction
        {
            private int wybor;
            public string Name { get; private set; }

            public WorkerFunction(int wybor)
            {
                this.wybor = wybor;
                switch (wybor)
                {
                    case 1:
                        Name = "y = 2x + 2x^2";
                        break;
                    case 2:
                        Name = "y = 2x^2";
                        break;
                    case 3:
                        Name = "y = 2x - 3";
                        break;
                }
            }

            public decimal GetY(decimal x)
            {
                return (decimal)ObliczFunkcje(wybor, (double)x);
            }
        }
    }
}