using MySqlConnector;

string connectionString = "Server=localhost;Database=holiday_maker;User ID=operator;Password=abcd1234;";

try
{
    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync();

    var repository = new HolidayMakerRepository(connection);
    var ui = new ConsoleUi();
    var app = new HolidayMakerApp(repository, ui);

    await app.RunAsync();
}
catch (MySqlException ex)
{
    switch (ex.ErrorCode)
    {
        case MySqlErrorCode.UnableToConnectToHost:
            ConsoleUi.PrintMessage("Fel: Kunde inte nå MySQL-servern. Kontrollera att servern är igång.");
            break;
        case MySqlErrorCode.AccessDenied:
            ConsoleUi.PrintMessage("Fel: Fel användarnamn eller lösenord till databasen.");
            break;
        case MySqlErrorCode.UnknownDatabase:
            ConsoleUi.PrintMessage("Fel: Databasen 'holiday_maker' hittades inte.");
            break;
        default:
            ConsoleUi.PrintMessage($"Databasfel ({ex.ErrorCode}): {ex.Message}");
            break;
    }
}
catch (Exception ex)
{
    ConsoleUi.PrintMessage($"Ett oväntad fel uppstod vid start: {ex.Message}");
}
