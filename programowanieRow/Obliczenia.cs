namespace programowanieRow;

public class Obliczenia
{
    
    public interface IFunction
    {
        decimal GetY(decimal x); 
        string Name { get; }
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
        
        for (int i = 0; i < n; i++)
        {
            double x = a + i * dx;  // Obliczamy kolejny punkt x
            double wartosc = ObliczFunkcje(wybor, x);  // Obliczamy f(x)
            sum += wartosc;
        }
        
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
                case 1 :
                    Name = "Funkcja 1: y = 2x + 2x^2";
                    break;
                case 2 :
                    Name = "Funkcja 2: y = 2x^2";
                    break;
                case 3 :
                    Name = "Funkcja 3: y = 2x - 3";
                    break;
                
            }
        }

        public decimal GetY(decimal x)
        {
            return (decimal)Obliczenia.ObliczFunkcje(wybor, (double)x);
        }

    }
}