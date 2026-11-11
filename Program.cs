using MySqlConnector;

// ANSLUTNING TILL DATABASEN
// Server=123.0.4.5;Port=3306;Database=shoppy;User ID=root;Password=rootroot;UseAffectedRows=false;

// känslig information som inte ska vara här. Egentligen.
string connectionString = "Server=localhost;Database=holiday_maker;User ID=root;Password=kurslosenord;UseAffectedRows=false;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();


var repository = new HolidayMakerRepository(connection);
