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
                        ConsoleUi.PrintMessage("Avslutar HolidayMaker. Hej då!");
                        running = false;
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                ConsoleUi.PrintMessage("\n--> Återgår till huvudmenyn...");
            }
            catch (MySqlException ex)
            {
                ConsoleUi.PrintMessage($"\nDatabasfel: {ex.Message}");
            }
            catch (Exception ex)
            {
                ConsoleUi.PrintMessage($"\nEtt oväntat fel uppstod: {ex.Message}");
            }
        }
    }
    private async Task ShowCustomerBookingAsync()
    {
        ConsoleUi.PrintHeader("Se kunds bokningar");
        int customerId = ui.ReadInt("Ange kund-ID");

        List<Booking> bookings = await repository.GetAllReservationsCustomerAsync(customerId);
        if (bookings.Count == 0)
        {
            ConsoleUi.PrintMessage($"Inga bokningar hittades for kund med ID {customerId}.");
            return;
        }

        ConsoleUi.PrintMessage($"Hittade {bookings.Count} bokning(ar) för kund #{customerId}:");
        ConsoleUi.PrintList(bookings);
    }

}