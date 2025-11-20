using System;
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

            Console.WriteLine("Zadanie 1 - Wyznaczanie wartości całki dla jednej z funkcji");
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
                case "2" :
                    zadanie2();
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
                Console.WriteLine("Naciśnij Enter, aby wrócić do menu");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
    
    static void zadanie1()
    {
        Console.WriteLine("=== ZADANIE 1: Wyznaczanie wartości całki dla jednej z funkccji ===\n");

        Console.WriteLine("Wybierz funkcję:");
        Console.WriteLine("1) y = 2x + 2x^2");
        Console.WriteLine("2) y = 2x^2");
        Console.WriteLine("3) y = 2x - 3");

        Console.Write("Twój wybór: ");
        int wyborFunkcji = int.Parse(Console.ReadLine());

        Console.WriteLine();
        
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        
        
        
        double wynik1 = Obliczenia.CalkaTrapezy(wyborFunkcji, -10, 10, 100000);
        double wynik2 = Obliczenia.CalkaTrapezy(wyborFunkcji, -5, 20, 100000);
        double wynik3 = Obliczenia.CalkaTrapezy(wyborFunkcji, -5, 0, 100000);

        stopwatch.Stop();
        long elapsed_time = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"Przedział [-10 ; 10] = {wynik1}");
        Console.WriteLine($"Przedział [-5 ; 20] = {wynik2}");
        Console.WriteLine($"Przedział [-5 ; 0]  = {wynik3}");

        Console.WriteLine($"\nCzas wykonania: {elapsed_time} ms");

        Console.ReadKey();
    }

    static void zadanie2()
    {
        
    }
}




