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
        Console.WriteLine("  (Tips: Skriv 'q' i vilket steg som helst för huvudmenyn)");
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
}