public sealed record Booking
{
    public int RoomId { get; init; }
    public int CustomerId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumPersons { get; init; }
    public bool HasPool {get; init; }
    public bool HasRestaurant {get; init; }
    public int MaxDistanceToBeach {get; init; }
    public int MaxDistanceToCity {get; init; }


    public Booking(int roomId, int customerId, DateTime checkInDate, DateTime checkOutDate, 
        int numPersons, bool hasPool, bool hasRestaurant, int maxDistanceToBeach, int maxDistanceToCity )
    {
        RoomId = roomId;
        CustomerId = customerId;
        CheckInDate = checkInDate.Date.AddHours(14);     //I automaticly set times where check in and check out are possible
        CheckOutDate = checkOutDate.Date.AddHours(11);
        NumPersons = numPersons;
        HasPool = hasPool;
        HasRestaurant = hasRestaurant;
        MaxDistanceToBeach = maxDistanceToBeach;
        MaxDistanceToCity = maxDistanceToCity;
    }
}