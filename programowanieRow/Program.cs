using System;
using System.ComponentModel;
using System.Diagnostics;
using programowanieRow;

internal class Program
{
    static void Main(string[] args)
    {
        string choice = "";
        while (choice != "0")
        {
            Console.Title = "=== MENU PROGRAMU ===";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=================================");
            Console.WriteLine("         Wybierz zadanie");
            Console.WriteLine("=================================");
            Console.ResetColor();

            Console.WriteLine("Zadanie 1 - Wyznaczanie wartości całki");
            Console.WriteLine("Zadanie 2 - Obliczenia z BackgroundWorker");
            Console.WriteLine("Zadanie 3 - Obliczenia z wyborem metody (BG/TPL/Thread/ThreadPool)");
            Console.WriteLine("Zadanie 4 - Porównanie wszystkich metod");
            Console.WriteLine("0 - Wyjście");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Wybierz numer zadania: ");
            Console.ResetColor();

            choice = Console.ReadLine();
            Console.Clear();

            switch (choice)
            {
                case "1":
                    zadanie1();
                    break;
                case "2":
                    zadanie2();
                    break;
                case "3":
                    zadanie3();
                    break;
                case "4":
                    zadanie4();
                    break;
                case "0":
                    Console.WriteLine("Koniec programu");
                    break;
                default:
                    Console.WriteLine("Niepoprawny wybór!");
                    break;
            }

            if (choice != "0")
            {
                Console.WriteLine("\nNaciśnij Enter, aby wrócić do menu");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    static void zadanie1()
    {
        Console.WriteLine("=== ZADANIE 1: Wyznaczanie wartości całki ===\n");

        Console.WriteLine("Wybierz funkcję:");
        Console.WriteLine("1) y = 2x + 2x^2");
        Console.WriteLine("2) y = 2x^2");
        Console.WriteLine("3) y = 2x - 3");
        int wyborFunkcji = ExceptionHolder.ReadInt("Twój wybór: ", 1, 3);

        var stopwatch = Stopwatch.StartNew();

        double wynik1 = Obliczenia.CalkaTrapezy(wyborFunkcji, -10, 10, 100000);
        double wynik2 = Obliczenia.CalkaTrapezy(wyborFunkcji, -5, 20, 100000);
        double wynik3 = Obliczenia.CalkaTrapezy(wyborFunkcji, -5, 0, 100000);

        stopwatch.Stop();

        Console.WriteLine($"\nPrzedział [-10 ; 10] = {wynik1}");
        Console.WriteLine($"Przedział [-5 ; 20] = {wynik2}");
        Console.WriteLine($"Przedział [-5 ; 0]  = {wynik3}");
        Console.WriteLine($"\nCzas wykonania: {stopwatch.ElapsedMilliseconds} ms");
    }

    static void zadanie2()
    {
        Console.WriteLine("=== ZADANIE 2: Obliczenia z BackgroundWorker ===\n");

        Console.WriteLine("Wybierz funkcję:");
        Console.WriteLine("1) y = 2x + 2x^2");
        Console.WriteLine("2) y = 2x^2");
        Console.WriteLine("3) y = 2x - 3");
        int wyborFunkcji = ExceptionHolder.ReadInt("Twój wybór: ", 1, 3);
        

        var func = new Obliczenia.WorkerFunction(wyborFunkcji);
        var processor = new BackgroundWorkerProcessor();
        
        RunCalculations(processor, func);
    }

    static void zadanie3()
    {
        Console.WriteLine("=== ZADANIE 3: Obliczenia z wyborem metody ===\n");

        Console.WriteLine("Wybierz metodę przetwarzania:");
        Console.WriteLine("1) BackgroundWorker");
        Console.WriteLine("2) TPL (Task Parallel Library)");
        Console.WriteLine("3) Thread");
        Console.WriteLine("4) ThreadPool");

        int wybor = ExceptionHolder.ReadInt("Twój wybór: ", 1, 4);

        Console.WriteLine("\nWybierz funkcję:");
        Console.WriteLine("1) y = 2x + 2x^2");
        Console.WriteLine("2) y = 2x^2");
        Console.WriteLine("3) y = 2x - 3");

        int wyborFunkcji = ExceptionHolder.ReadInt("Twój wybór: ", 1, 3);

        var func = new Obliczenia.WorkerFunction(wyborFunkcji);
        IProcessor processor;

        switch (wybor)
        {
            case 1:
                processor = new BackgroundWorkerProcessor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- BackgroundWorker ---");
                Console.ResetColor();
                break;
            case 2:
                processor = new TplProcessor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- TPL ---");
                Console.ResetColor();
                break;
            case 3:
                processor = new ThreadProcessor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- Thread ---");
                Console.ResetColor();
                break;
            case 4:
                processor = new ThreadPoolProcessor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n--- ThreadPool ---");
                Console.ResetColor();
                break;
            default:
                processor = new BackgroundWorkerProcessor();
                break;
        }

        RunCalculations(processor, func);
    }

    static void RunCalculations(IProcessor processor, Obliczenia.IFunction func)
    {
        var ranges = new (double A, double B, string Name)[]
        {
            (-10, 10, "Przedział 1"),
            (-5, 20, "Przedział 2"),
            (-5, 0, "Przedział 3")
        };

        double[] results = new double[3];
        long[] threadTimes = new long[3];
        int[] progress = new int[3];
        int finished = 0;
        object lockObj = new object();

        Console.WriteLine("\nNaciśnij ENTER, aby rozpocząć obliczenia.");
        Console.WriteLine("Naciśnij 'C' w trakcie obliczeń, aby anulować.\n");
        Console.ReadLine();

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 3; i++)
        {
            int index = i;

            processor.StartWork(
                func,
                ranges[i].A,
                ranges[i].B,
                100_000_000,
                
                (prog) =>
                {
                    lock (lockObj)
                    {
                        progress[index] = prog;
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($"{ranges[0].Name}: {progress[0],3}%  |  " +
                                      $"{ranges[1].Name}: {progress[1],3}%  |  " +
                                      $"{ranges[2].Name}: {progress[2],3}%   ");
                    }
                },
                
                (result, cancelled, threadTime) =>
                {
                    lock (lockObj)
                    {
                        threadTimes[index] = threadTime;

                        if (cancelled || double.IsNaN(result))
                        {
                            Console.WriteLine($"\n{ranges[index].Name} [{ranges[index].A} ; {ranges[index].B}] - ANULOWANO (Czas wątku: {threadTime} ms)");
                            results[index] = double.NaN;
                        }
                        else
                        {
                            results[index] = result;
                            Console.WriteLine($"\n{ranges[index].Name} [{ranges[index].A} ; {ranges[index].B}] = {results[index]:F6} ✓ (Czas wątku: {threadTime} ms)");
                        }

                        finished++;

                        if (finished == 3)
                        {
                            stopwatch.Stop();
                            Console.WriteLine("\n" + new string('=', 70));
                            Console.WriteLine("                         PODSUMOWANIE");
                            Console.WriteLine(new string('=', 70));

                            for (int j = 0; j < 3; j++)
                            {
                                if (!double.IsNaN(results[j]))
                                {
                                    Console.WriteLine($"{ranges[j].Name} [{ranges[j].A,4} ; {ranges[j].B,4}] = {results[j]:F6} | Czas: {threadTimes[j]} ms");
                                }
                                else
                                {
                                    Console.WriteLine($"{ranges[j].Name} [{ranges[j].A,4} ; {ranges[j].B,4}] = ANULOWANO | Czas: {threadTimes[j]} ms");
                                }
                            }

                            Console.WriteLine(new string('=', 70));
                            Console.WriteLine($"Całkowity czas wykonania aplikacji: {stopwatch.ElapsedMilliseconds} ms");
                            Console.WriteLine(new string('=', 70) + "\n");
                        }
                    }
                }
            );
        }

        Console.WriteLine("Obliczenia w toku...\n");
        Console.WriteLine("Przedział 1:   0%  |  Przedział 2:   0%  |  Przedział 3:   0%   ");

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.C)
                {
                    lock (lockObj)
                    {
                        Console.WriteLine("\n\nAnulowanie wszystkich operacji...\n");
                    }
                    processor.CancelAll();
                }
            }

            lock (lockObj)
            {
                if (finished == 3) break;
            }

            Thread.Sleep(50);
        }

        Thread.Sleep(200);
    }
     static void zadanie4()
    {
        Console.WriteLine("=== ZADANIE 4: Porównanie wszystkich metod ===\n");

        Console.WriteLine("Wybierz funkcję:");
        Console.WriteLine("1) y = 2x + 2x^2");
        Console.WriteLine("2) y = 2x^2");
        Console.WriteLine("3) y = 2x - 3");

        int wyborFunkcji = ExceptionHolder.ReadInt("Twój wybór: ", 1, 3);

        var func = new Obliczenia.WorkerFunction(wyborFunkcji);

        var metody = new (string Nazwa, IProcessor Processor)[]
        {
            ("BackgroundWorker", new BackgroundWorkerProcessor()),
            ("TPL", new TplProcessor()),
            ("Thread", new ThreadProcessor()),
            ("ThreadPool", new ThreadPoolProcessor())
        };

        var wyniki = new List<(string Metoda, long[] CzasyWatkow, long CzasCalkowity)>();

        Console.WriteLine("\nNaciśnij ENTER, aby rozpocząć testy wszystkich metod.");
        Console.ReadLine();

        foreach (var metoda in metody)
        {
            string nazwaMetody = metoda.Nazwa;
            IProcessor processor = metoda.Processor;
            Console.Clear();
            Console.WriteLine($"=== Testowanie metody: {nazwaMetody} ===\n");

            var ranges = new (double A, double B, string Name)[]
            {
                (-10, 10, "Przedział 1"),
                (-5, 20, "Przedział 2"),
                (-5, 0, "Przedział 3")
            };

            double[] results = new double[3];
            long[] threadTimes = new long[3];
            int[] progress = new int[3];
            int finished = 0;
            object lockObj = new object();

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 3; i++)
            {
                int index = i;

                processor.StartWork(
                    func,
                    ranges[i].A,
                    ranges[i].B,
                    100_000_000,
                    (prog) =>
                    {
                        lock (lockObj)
                        {
                            progress[index] = prog;
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($"{ranges[0].Name}: {progress[0],3}%  |  " +
                                          $"{ranges[1].Name}: {progress[1],3}%  |  " +
                                          $"{ranges[2].Name}: {progress[2],3}%   ");
                        }
                    },
                    (result, cancelled, threadTime) =>
                    {
                        lock (lockObj)
                        {
                            threadTimes[index] = threadTime;
                            results[index] = cancelled ? double.NaN : result;
                            finished++;
                        }
                    }
                );
            }

            Console.WriteLine("\nObliczenia w toku...\n");
            Console.WriteLine("Przedział 1:   0%  |  Przedział 2:   0%  |  Przedział 3:   0%   ");

            while (true)
            {
                lock (lockObj)
                {
                    if (finished == 3) break;
                }
                Thread.Sleep(50);
            }

            stopwatch.Stop();
            wyniki.Add((nazwaMetody, threadTimes, stopwatch.ElapsedMilliseconds));

            Console.WriteLine($"\n\n✓ {nazwaMetody} zakończone - Czas całkowity: {stopwatch.ElapsedMilliseconds} ms");
            Thread.Sleep(1000);
        }

        
        Console.Clear();
        Console.WriteLine("=== PODSUMOWANIE PORÓWNANIA METOD ===\n");
        Console.WriteLine(new string('=', 80));
        Console.WriteLine($"{"Metoda",-20} | {"Wątek 1",-12} | {"Wątek 2",-12} | {"Wątek 3",-12} | {"Czas całkowity",-15}");
        Console.WriteLine(new string('=', 80));

        foreach (var (metoda, czasyWatkow, czasCalkowity) in wyniki)
        {
            Console.WriteLine($"{metoda,-20} | {czasyWatkow[0] + " ms",-12} | {czasyWatkow[1] + " ms",-12} | {czasyWatkow[2] + " ms",-12} | {czasCalkowity + " ms",-15}");
        }

        Console.WriteLine(new string('=', 80));
        
    }
    
}


