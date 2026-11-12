using MySqlConnector;

public class HolidayMakerRepository(MySqlConnection connection)
{
    //Check customer by email or telephoner number
    public async Task<(Customer? customer, int customer_id)> GetCustomerAsync(string telephone)
    {
        await using var command = new MySqlCommand(
            "SELECT * FROM customers WHERE telephone = @telephone", connection);
        command.Parameters.AddWithValue("@telephone", telephone);

        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int customer_id = reader.GetInt32("id");
            var customer = new Customer(
                reader.GetInt32("id"),
                reader.GetString("name"),
                reader.GetString("last_name"),
                reader.GetString("telephone"),
                reader.GetString("email")
                );
            return (customer, customer_id);
        }
        return (null, 0);
    }

    //Create a customer
    public async Task<int> CreateCustomerAsync(string name, string last_name, string telephone, string email )
    {
        try
        {
            await using var command = new MySqlCommand(
                "INSERT INTO customers (name, last_name, email, telephone) VALUES (@name, @last_name, @email, @telephone);", connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@last_name", last_name);
            command.Parameters.AddWithValue("@telephone", telephone);
            command.Parameters.AddWithValue("@email", email);
            await command.PrepareAsync();
            await command.ExecuteNonQueryAsync();
            return checked((int)command.LastInsertedId);
        }
        catch (MySqlException ex)
        {
            if (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                Console.WriteLine("Kunde inte skapa kund: Telefonnummer eller e-post finns redan.");
            }
            else
            {
                Console.WriteLine($"Kunde inte skapa kund: {ex.Message}");
            }
            return 0;
        }
    }
    //Look for free rooms with customers filters
    
    //Make a booking
    //Get cofirmation of booking 
    //Remove reservation
    

}