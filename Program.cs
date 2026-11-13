using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=operator;Password=abcd1234;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();

var repository = new HolidayMakerRepository(connection);
/*
//Get the customer by telephone number

var (customer, customer_id) = await repository.GetCustomerAsync("0735486510");
Console.WriteLine(customer is null ? "Det finns ingen kund med den där telefon." : customer.ToString());

//-----------------------------------
//Create a customer if there isn't any

customer_id = await repository.CreateCustomerAsync("Pernilla", "Anderssons", "0730854695", "perni.and@example.com");
if (customer_id > 0)
{
    Console.WriteLine($"Skapad customer med id: {customer_id}"); 
}
//-----------------------------------
//Check avaible hotel - city 

List<HotelCity> hotelCities = await repository.GetAvaibleHotelCity();
Console.WriteLine("-----Our Hotels-----");
foreach (HotelCity hotelCity in hotelCities)
{
    Console.WriteLine(hotelCity);
}

//-----------------------------------
//Look for avaible room
List<Room> freeRooms = await repository.GetAvaibleRoomsAsync(
    city: "Palma",
    checkInDate: DateOnly.ParseExact("2026-12-14", "yyyy-MM-dd"),
    checkOutDate: DateOnly.ParseExact("2026-12-18", "yyyy-MM-dd"),
    numPersons: 1,
    hasPool: null,
    hasRestaurant: null,
    maxDistanceToBeach: null,
    maxDistanceToCity: null,
    sortBy: "price");

Console.WriteLine("-----Lediga rum-----");
Console.WriteLine("------------------------------------------------------------");
foreach (Room room in freeRooms)
{
    Console.WriteLine(room);
}

//-----------------------------------
//Create a booking
int newReservationId = await repository.CreateBookingAsync(
    room_id: 5,
    customer_id: 2,
    checkInDate: DateOnly.ParseExact("2026-12-14", "yyyy-MM-dd"),
    checkOutDate: DateOnly.ParseExact("2026-12-18", "yyyy-MM-dd"),
    num_persons: 1);

if (newReservationId > 0)
{
    Console.WriteLine($"Skapad bokning med id: {newReservationId}");
}

//-----------------------------------
//Check last booking
Booking? created = await repository.GetLastReservationAsync(32);   //it shoud be newReservationId but I am testing one on one
Console.WriteLine(created is null ? "Bokningen saknas." : created.ToString());

//-----------------------------------

//Check all reservations to customer from customer id

List<Booking> customersBooking = await repository.GetAllReservationsCustomerAsync(2);
Console.WriteLine("-----Customer reservations-----");
Console.WriteLine("------------------------------------------------------------");
foreach (Booking booking in customersBooking)
{
    Console.WriteLine(booking);
}
//-----------------------------------
*/

//Update number of persons
