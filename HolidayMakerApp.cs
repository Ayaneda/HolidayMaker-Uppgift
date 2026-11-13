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
    //Check customer's reservations through customer id
    private async Task ShowCustomerBookingsAsync()
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

    //Creating a reservation with a flow (wait)

    //Update a booking
    private async Task UpdateBookingAsync()
    {
        ConsoleUi.PrintHeader("Uppdatera bokning");
        int bookingId = ui.ReadInt("Ange bokning-ID att uppdatera");
        int newNumPersons = ui.ReadInt("Ange nytt antal personer");

        int updateRows = await repository.GetReservationUpdateAsync(bookingId, newNumPersons);
        if (updateRows > 0)
        {
            ConsoleUi.PrintMessage("\nBokningen har uppdaterats! Här är de nya uppgifterna:");
            Booking? updatedBooking = await repository.GetLastReservationAsync(bookingId);
            if (updatedBooking is not null)
            {
                ConsoleUi.PrintMessage(updatedBooking.ToString());
            }
        }
    }

}