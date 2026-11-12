using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=operator;Password=abcd1234;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();

var repository = new HolidayMakerRepository(connection);
