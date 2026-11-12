using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=operator;Password=abcd1234;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();

var repository = new HolidayMakerRepository(connection);

//Get the customer by telephone number
var (customer, customer_id) = await repository.GetCustomerAsync("0735486510");
Console.WriteLine(customer is null ? "Det finns ingen kund med den där telefon." : customer.ToString());