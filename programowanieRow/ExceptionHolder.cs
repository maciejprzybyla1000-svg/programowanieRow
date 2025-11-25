namespace programowanieRow;

public class ExceptionHolder //exception handling na dodaktowe punkty :)
{
    public static int ReadInt(string prompt, int min, int max)
    {
        int result;
        while (true)
        {
            Console.WriteLine(prompt);
            String input = Console.ReadLine();
            
            if(int.TryParse(input, out result) && result >= min && result <= max)
            { 
                return result;
            }

            Console.WriteLine("Błędna wartość spróbuj ponownie ");
        }
    }
}