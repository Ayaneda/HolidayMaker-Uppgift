using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=operator;Password=abcd1234;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();

var repository = new HolidayMakerRepository(connection);
/*
//Get the customer by telephone number
var (customer, customer_id) = await repository.GetCustomerAsync("0735486510");
Console.WriteLine(customer is null ? "Det finns ingen kund med den där telefon." : customer.ToString());

//Create a customer if there isn't any

customer_id = await repository.CreateCustomerAsync("Pernilla", "Anderssons", "0730854695", "perni.and@example.com");
if (customer_id > 0)
{
    Console.WriteLine($"Skapad customer med id: {customer_id}"); 
}
*/
//Look for avaible room
List<Room> freeRooms = await repository.GetAvaibleRoomsAsync(
    city: "Palma",
    checkInDate: new DateOnly(2026, 12, 14),
    checkOutDate: new DateOnly(2026, 12, 18),
    numPersons: 1,
    hasPool: null,
    hasRestaurant: null,
    maxDistanceToBeach: null,
    maxDistanceToCity: null,
    sortBy: "price");

Console.WriteLine("-----Lediga rum-----");
foreach (Room room in freeRooms)
{
    Console.WriteLine(room);
}