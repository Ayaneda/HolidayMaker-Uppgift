using System.ComponentModel.Design;
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
    public async Task CreateBookingFlowAsync()
    {
        ConsoleUi.PrintHeader("Skapa ny bokning");
        // 1. Control there is a customer through telephon, else create a new customer
        string telephone = ui.ReadString("Ange kundens telefonnummer");
        var (customer, customerId) = await repository.GetCustomerAsync(telephone);

        if (customer is not null)
        {
            ConsoleUi.PrintMessage($"Kund hittades: {customer.Name} {customer.LastName} (Kund-ID: {customerId})");
        }
        else
        {
            ConsoleUi.PrintMessage("Ingen kund hittades med det telefonnumret. Vi registrerar en ny kund:");
            
            while (customerId == 0)
            {
                string name = ui.ReadString("Förnamn");
                string lastName = ui.ReadString("Efternamn");
                string email = ui.ReadString("E-post");

                customerId = await repository.CreateCustomerAsync(name, lastName, telephone, email);
                if (customerId == 0)
                {
                    ConsoleUi.PrintMessage("Försök igen med andra kunduppgifter (eller skriv 'Q' för huvudmenyn).");
                    telephone = ui.ReadString("Ange telefonnummer");
                }
            }
            ConsoleUi.PrintMessage($"Ny kund skapad med ID: {customerId}");
        }

        // 2. Show how many hotels and in what city they are 
        ConsoleUi.PrintHeader("Våra hotel och städer");
        List<HotelCity> hotelCities = await repository.GetAvaibleHotelCity();
        ConsoleUi.PrintList(hotelCities);

        // 3. Search for free room (save checkInDate, checkOutDate och numPersons for booking)
        DateOnly checkInDate;
        DateOnly checkOutDate;
        int numPersons;
        List<Room> freeRooms;

        while (true)
        {
            ConsoleUi.PrintHeader("Sök lediga rum");
            string city = ui.ReadString("Ange stad");
            checkInDate = ui.ReadDate("Incheckningsdatum");
            checkOutDate = ui.ReadDate("Utcheckningsdatum", minDate: checkInDate);
            numPersons = ui.ReadInt("Antal personer");
            bool? hasPool = ui.ReadOptionalBool("Krav på pool?");
            bool? hasRestaurant = ui.ReadOptionalBool("Krav på restaurang?");
            int? maxDistanceToBeach = ui.ReadOptionalInt("Max avstånd till strand (m)");
            int? maxDistanceToCity = ui.ReadOptionalInt("Max avstånd till centrum (m)");
            string sortBy = ui.ReadSortBy();

            freeRooms = await repository.GetAvaibleRoomsAsync(
                city, checkInDate, checkOutDate, numPersons, hasPool, hasRestaurant, maxDistanceToBeach, maxDistanceToCity, sortBy);

            if (freeRooms.Count > 0)
            {
                break;
            }
            
            ConsoleUi.PrintMessage("Inga lediga rum matchade dina val. Försök med andra filter (eller skriv 'Q' för meny)");
        }

        ConsoleUi.PrintHeader($"Lediga rum ({freeRooms.Count} st)");
        ConsoleUi.PrintList(freeRooms);

        //4. Choose a room and create a reservation with saved variables
        int roomId;
        while (true)
        {
            roomId = ui.ReadInt("Ange rumsnummer (Rum #) du vill boka");
            if (freeRooms.Any(r => r.Id == roomId))
            {
                break;
            }
            ConsoleUi.PrintMessage("Fel: Välj ett rumsnummer som finns i listan ovan.");
        }

        int newBookingId = await repository.CreateBookingAsync(
            roomId, customerId, checkInDate, checkOutDate, numPersons);
    }

    

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

    private async Task RemoveBookingAsync()
    {
        ConsoleUi.PrintHeader("Ta bort bokning");
        int customerId = ui.ReadInt("Ange kund-ID för att se kundens bokningar");

        List<Booking> bookings = await repository.GetAllReservationsCustomerAsync(customerId);
        if (bookings.Count == 0)
        {
            ConsoleUi.PrintMessage($"Kund #{customerId} har inga bokningar att ta bort.");
            return;
        }

        ConsoleUi.PrintList(bookings);

        int bookingId = ui.ReadInt("Ange bokning-ID att radera");
        int removedRows = await repository.GetRemoveReservationAsync(bookingId);
        if (removedRows > 0)
        {
            ConsoleUi.PrintMessage($"\nBokning #{bookingId} har raderats: Uppdaterad bokningslista:");
            List<Booking> remainingBookings = await repository.GetAllReservationsCustomerAsync(customerId);
            if (remainingBookings.Count == 0)
            {
                ConsoleUi.PrintMessage("Kunden har nu inga aktiva bokningar.");
            }
            else
            {
                ConsoleUi.PrintList(remainingBookings);
            }
        } 
    }
}