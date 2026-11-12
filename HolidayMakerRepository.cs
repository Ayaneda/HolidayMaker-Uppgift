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
    //Look for free rooms with customers filters
    //Make a booking
    //Get cofirmation of booking 
    //Remove reservation
    

}