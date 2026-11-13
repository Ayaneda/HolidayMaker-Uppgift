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
    public async Task<int> CreateBookingAsync(
        int room_id,
        int customer_id, 
        DateOnly checkInDate, 
        DateOnly checkOutDate, 
        int num_persons )
    {
            //room_id, customer_id, start_date, end_date, num_persons
        try
        {
            await using var command = new MySqlCommand(
                @"INSERT INTO bookings (room_id, customer_id, start_date, end_date, num_persons) 
                SELECT r.id, @customer_id, TIMESTAMP(@check_in, '14:00:00'), TIMESTAMP(@check_out, '11:00:00'), @num_persons
                FROM rooms r
                WHERE r.id = @room_id
                AND r.persons_capacity >= @num_persons
                AND NOT EXISTS (
                    SELECT 1
                    FROM bookings bk
                    WHERE bk.room_id = @room_id
                    AND bk.end_date   > TIMESTAMP(@check_in, '14:00:00')
                    AND bk.start_date < TIMESTAMP(@check_out, '11:00:00')
                );", connection);
            command.Parameters.AddWithValue("@room_id", room_id);
            command.Parameters.AddWithValue("@customer_id", customer_id);
            command.Parameters.AddWithValue("@check_in", checkInDate);
            command.Parameters.AddWithValue("@check_out", checkOutDate);
            command.Parameters.AddWithValue("@num_persons", num_persons);
            await command.PrepareAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                Console.WriteLine("Kunde inte skapa bokning: Rummet är inte ledigt eller rymmer inte antalet personer.");
                return 0;
            }
            return checked((int)command.LastInsertedId);
        }
        catch (MySqlException ex)
        {
            if (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                Console.WriteLine("Kunde inte skapa bokning: Det finns en bokning redan.");
            }
            else
            {
                Console.WriteLine($"Kunde inte skapa bokning: {ex.Message}");
            }
            return 0;
        }
    }
    //Get cofirmation of booking last inserted
    public async Task<Booking?> GetLastReservationAsync(int id)
    {
        await using var command = new MySqlCommand(
            @"SELECT bookings.id,
            h.name AS hotel_name,
            h.city,
            bookings.room_id,
            r.name AS room_name,
            bookings.start_date,
            bookings.end_date,
            DATEDIFF(bookings.end_date, bookings.start_date) AS nights,
            bookings.num_persons,
            (r.price * DATEDIFF(bookings.end_date, bookings.start_date)) AS Total_price
            FROM bookings
                JOIN rooms r ON bookings.room_id = r.id
                JOIN hotels h ON r.hotel_id = h.id
                WHERE bookings.id = @id;"
            , connection);
        command.Parameters.AddWithValue("@id", id);

        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Booking(
                reader.GetInt32("id"),
                reader.GetString("hotel_name"),
                reader.GetString("city"),
                reader.GetInt32("room_id"),
                reader.GetString("room_name"),
                reader.GetDateTime("start_date"),
                reader.GetDateTime("end_date"),
                reader.GetInt32("nights"),
                reader.GetInt32("num_persons"),
                reader.GetDecimal("Total_price"));
        }
        return null;
    }

    //Show customers all reservations by customers id
    public async Task<List<Booking>> GetAllReservationsCustomerAsync(int id)
    {
        await using var command = new MySqlCommand(
            @"SELECT bookings.id,
            h.name AS hotel_name,
            h.city,
            bookings.room_id,
            r.name AS room_name,
            bookings.start_date,
            bookings.end_date,
            DATEDIFF(bookings.end_date, bookings.start_date) AS nights,
            bookings.num_persons,
            (r.price * DATEDIFF(bookings.end_date, bookings.start_date)) AS Total_price
            FROM bookings
                JOIN rooms r ON bookings.room_id = r.id
                JOIN hotels h ON r.hotel_id = h.id
            WHERE bookings.customer_id = @id
            ORDER BY bookings.start_date;"
            , connection);
        command.Parameters.AddWithValue("@id", id);

        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        var customersBookings = new List<Booking>();

        while (await reader.ReadAsync())
        {
            customersBookings.Add(new Booking (
                reader.GetInt32("id"),
                reader.GetString("hotel_name"),
                reader.GetString("city"),
                reader.GetInt32("room_id"),
                reader.GetString("room_name"),
                reader.GetDateTime("start_date"),
                reader.GetDateTime("end_date"),
                reader.GetInt32("nights"),
                reader.GetInt32("num_persons"),
                reader.GetDecimal("Total_price")));
        }
        return customersBookings;
    }
    //Update num persons
    public async Task<int> GetReservationUpdateAsync(int id, int newNumPersons)
    {
        await using var command = new MySqlCommand(
            @"UPDATE bookings
            JOIN rooms ON bookings.room_id = rooms.id
            SET bookings.num_persons = @new_num_persons
            WHERE bookings.id = @booking_id
            AND rooms.persons_capacity >= @new_num_persons;", connection);

        command.Parameters.AddWithValue("@booking_id", id);
        command.Parameters.AddWithValue("@new_num_persons", newNumPersons);

        await command.PrepareAsync();
        int rowsAffected = await command.ExecuteNonQueryAsync();
        if (rowsAffected == 0)
        {
            Console.WriteLine("Kunde inte uppdatera bokning: Bokningen saknas eller rummet rymmer inte antalet personer.");
        }
        return rowsAffected;
    }
    //Remove reservation
    public async Task<int> GetRemoveReservationAsync(int id)
    {
        await using var command = new MySqlCommand(
            @"DELETE FROM bookings WHERE bookings.id = @booking_id;"
            ,connection);
        
        command.Parameters.AddWithValue("@booking_id", id);

        await command.PrepareAsync();
        int rowsAffected = await command.ExecuteNonQueryAsync();
        if (rowsAffected == 0)
        {
            Console.WriteLine("Kunde inte radera bokning: Bokningen saknas.");
        }
        return rowsAffected;
    }

}