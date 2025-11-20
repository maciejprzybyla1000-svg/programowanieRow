namespace programowanieRow;

public class Obliczenia
{
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
        
        for (int i = 0; i < n; i++)
        {
            double x = a + i * dx;  // Obliczamy kolejny punkt x
            double wartosc = ObliczFunkcje(wybor, x);  // Obliczamy f(x)
            sum += wartosc;
        }
        
        return sum * dx;
    }

}