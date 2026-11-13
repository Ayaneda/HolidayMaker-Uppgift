using System.Runtime.InteropServices;
using MySqlConnector;
public class HolidayMakerApp(HolidayMakerRepository repository, ConsoleUi ui)
{
    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            try
            {
                int choice = ui.ShowMainMenu();
                switch (choice)
                {
                    case 1:
                        await ShowCustomerBookingsAsync();
                        break;
                    case 2:
                        await CreateBookingFlowAsync();
                        break;
                    case 3:
                        await UpdateBookingAsync();
                        break;
                    case 4:
                        await RemoveBookingAsync();
                        break;
                    case 0:
                        Console.WriteLine("Avslutar HolidayMaker. Hej då!");
                        running = false;
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n--> Återgår till huvudmenyn...");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"\nDatabasfel: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nEtt oväntat fel uppstod: {ex.Message}");
            }
        }
    }
}