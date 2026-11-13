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

}