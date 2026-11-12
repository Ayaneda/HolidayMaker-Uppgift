public sealed record Booking
{
    public int RoomId { get; init; }
    public int CustomerId { get; init; }
    public DateTime CheckInDate { get; init; }
    public DateTime CheckOutDate { get; init; }
    public int NumPersons { get; init; }

    public Booking(int roomId, int customerId, DateTime checkInDate, DateTime checkOutDate, int numPersons)
    {
        RoomId = roomId;
        CustomerId = customerId;
        CheckInDate = checkInDate.Date.AddHours(14);     //I automaticly set times where check in and check out are possible
        CheckOutDate = checkOutDate.Date.AddHours(11);
        NumPersons = numPersons;
    }
}