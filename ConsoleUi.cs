using System.Globalization;
using System.Net.Mail;

public class ConsoleUi
{
    public int ShowMainMenu()
    {
        Console.WriteLine();
        Console.WriteLine("============================================================");
        Console.WriteLine("                     HOLIDAY MAKER                          ");
        Console.WriteLine("============================================================");
        Console.WriteLine("  1. Se kunds bokningar");
        Console.WriteLine("  2. Skapa ny bokning");
        Console.WriteLine("  3. Uppdatera bokning (antal personer)");
        Console.WriteLine("  4. Ta bort bokning");
        Console.WriteLine("  0. Avsluta programmet");
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine("  (Tips: Skriv 'Q' i vilket steg som helst för huvudmenyn)");
        Console.WriteLine("============================================================");
        while (true)
        {
            Console.Write("Välj ett alternativ (0-4): ");
            string? input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 4)
            {
                return choice;
            }
            Console.WriteLine("Fel: Välj en siffra mellan 0 och 4.");
        }
    }

    //Check if the user wamt to go back to the main menu
    private static void CheckCancel(string? input)
    {
        if(string.Equals(input, "Q", StringComparison.Ordinal))
        {
            throw new OperationCanceledException();
        }
    }

    public string ReadString(string prompt)
    {
        while(true)
        {
            Console.Write($"{prompt} (eller 'Q' för meny):");
            string? input = Console.ReadLine()?.Trim();
            CheckCancel(input);

            if(!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            Console.WriteLine("Fel: Fältet får inte vara tomt. Försökt igen.");
        }
    }

    public int ReadInt(string prompt, int min = 1)
    {
        while (true)
        {
            Console.Write($"{prompt} (eller 'Q' för meny): ");
            string? input = Console.ReadLine()?.Trim();
            CheckCancel(input);

            if (int.TryParse(input, out int value) && value >= min)
            {
                return value;
            }
            Console.WriteLine($"Fel: Ange ett jeltal (minst {min}).");
        }
    }

    public DateOnly ReadDate(string prompt, DateOnly? minDate = null)
    {
        while (true)
        {
            Console.Write($"{prompt} [yyyy-MM-dd] (eller 'Q' för meny): ");
            string? input = Console.ReadLine()?.Trim();
            CheckCancel(input);

            if (DateOnly.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture,DateTimeStyles.None, out DateOnly date))
            {
                if (minDate is null || date > minDate.Value)
                {
                    return date;
                }
                Console.WriteLine($"Fel: Datumet måste vara efter {minDate.Value:yyyy-MM-dd}.");
            }
            else
            {
                Console.WriteLine("Fel: Ange datum i formatet yyyy-MM-dd (t.ex. 2026-12-14).");
            }
        }
    }

    public bool? ReadOptionalBool (string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt} [j/n, Enter för att hoppa över, 'Q' för meny]: ");
            string? input = Console.ReadLine()?.Trim();
            CheckCancel(input);
            input = input?.ToLower();

            if(string.IsNullOrEmpty(input)) return null;
            if (input == "j" || input == "ja") return true;
            if (input == "n" || input == "nej") return false;
            Console.WriteLine("Fel: Skriv 'j' för ja, 'n' för nej, eller tryck Enter för att hoppa över.");
        }
    }

    public int? ReadOptionalInt(string prompt, int min = 0)
    {
        while (true)
        {
            Console.Write($"{prompt} [Enter för att hoppa över, 'Q' för meny]: ");
            string? input = Console.ReadLine()?.Trim();
            CheckCancel(input);

            if (string.IsNullOrEmpty(input)) return null;
            if (int.TryParse(input, out int value) && value >= min)
            {
                return value;
            }
            Console.WriteLine($"Fel: Ange ett heltal (minst {min}) eller tryck Enter för att hoppa över.");
        }
    }

    public string ReadSortBy()
    {
        while (true)
        {
            Console.Write("Sortera efter (i = Pris, 2 = Betyg) [Enter = Pris, 'Q' för meny]: ");
            string? input = Console.ReadLine()?.Trim();
            CheckCancel(input);

            if (string.IsNullOrEmpty(input) || input == "1" || input.Equals("price", StringComparison.OrdinalIgnoreCase))
                return "price";
            if (input == "2" || input.Equals("rating", StringComparison.OrdinalIgnoreCase))
                return "rating";

            Console.WriteLine("Fel: Välj 1 (Pris) eller 2 (Betyg).");
        }
        
    }

}