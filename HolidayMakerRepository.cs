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
    public async Task<int> CreateCustomerAsync(
        string name, 
        string last_name, 
        string telephone, 
        string email )
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
    //Check which cities are avaible
    public async Task<List<HotelCity>> GetAvaibleHotelCity()
    {
        await using var command = new MySqlCommand(
            "SELECT h.name, h.city FROM hotels h;", connection);
            await using var reader = await command.ExecuteReaderAsync();

        var hotelCities = new List<HotelCity>();
        
        while (await reader.ReadAsync())
        {
            hotelCities.Add(new HotelCity(
                reader.GetString("name"),
                reader.GetString("city")
                ));
        }
        return hotelCities;   
    }
    
    //Look for free rooms with customers filters
    public async Task<List<Room>> GetAvaibleRoomsAsync(
        string city,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int numPersons,
        bool? hasPool = null,
        bool? hasRestaurant = null,
        int? maxDistanceToBeach = null,
        int? maxDistanceToCity = null,
        string sortBy = "price")
    {
        await using var command = new MySqlCommand(
            @"SELECT r.id, 
            r.name AS Room_name, 
            r.persons_capacity, 
            h.name AS hotel, 
            h.city, 
            r.price,
            h.rating, 
            h.has_pool, 
            h.has_restaurant, 
            h.distance_to_beach, 
            h.distance_to_city_center
            FROM rooms r 
            JOIN hotels h 
            ON r.hotel_id = h.id 
            WHERE 
            h.city = @city 
            AND r.persons_capacity >= @num_persons 
            AND NOT EXISTS (
                SELECT 1 
                FROM bookings bk 
                WHERE bk.room_id = r.id 
                AND bk.end_date   > TIMESTAMP(@check_in, '14:00:00')
                AND bk.start_date < TIMESTAMP(@check_out, '11:00:00')              
                )
            AND (@has_pool IS NULL OR h.has_pool = @has_pool)
            AND (@has_restaurant IS NULL OR h.has_restaurant = @has_restaurant)
            AND (@distance_to_beach IS NULL OR h.distance_to_beach <= @distance_to_beach)
            AND (@distance_to_center IS NULL OR h.distance_to_city_center <= @distance_to_center)
            ORDER BY 
                CASE WHEN @Sortby = 'price' THEN r.price END ASC,
                CASE WHEN @Sortby = 'rating' THEN h.rating END DESC;", connection);
        
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@check_in", checkInDate);
        command.Parameters.AddWithValue("@check_out", checkOutDate);
        command.Parameters.AddWithValue("@num_persons", numPersons);
        command.Parameters.AddWithValue("@has_pool", (object?)hasPool ?? DBNull.Value);
        command.Parameters.AddWithValue("@has_restaurant", (object?)hasRestaurant ?? DBNull.Value);
        command.Parameters.AddWithValue("@distance_to_beach", (object?)maxDistanceToBeach ?? DBNull.Value);
        command.Parameters.AddWithValue("@distance_to_center", (object?)maxDistanceToCity ?? DBNull.Value);
        command.Parameters.AddWithValue("@Sortby", sortBy);

        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        var freeRooms = new List<Room>();

        while (await reader.ReadAsync())
        {
            freeRooms.Add(new Room
            (
                reader.GetInt32("id"),
                reader.GetString("Room_name"),
                reader.GetInt32("persons_capacity"),
                reader.GetString("hotel"),
                reader.GetString("city"),
                reader.GetDecimal("price"),
                reader.GetInt32("rating"),
                reader.GetBoolean("has_pool"),
                reader.GetBoolean("has_restaurant"),
                reader.GetInt32("distance_to_beach"),
                reader.GetInt32("distance_to_city_center")
            ));
        }

        return freeRooms;
    }


    //Make a booking
    //room_id, customer_id, start_date, end_date, num_persons
    //Get cofirmation of booking 
    //Remove reservation
    

}