using MySqlConnector;

// ANSLUTNING TILL DATABASEN
// Server=123.0.4.5;Port=3306;Database=shoppy;User ID=root;Password=rootroot;UseAffectedRows=false;

// känslig information som inte ska vara här. Egentligen.
string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=kurslosenord;UseAffectedRows=false;";

await using var connectionBooking = new MySqlConnection(connectionString);
await using var connectionCustomer = new MySqlConnection(connectionString);
await connectionBooking.OpenAsync();
await connectionCustomer.OpenAsync();

var repositoryBooking = new BookingRepository(connectionBooking);
var repositoryCustomer = new CustomerRepository(connectionCustomer);