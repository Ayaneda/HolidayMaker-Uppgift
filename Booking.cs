public sealed record Booking(
    int BookingId, 
    string HotelName,
    string City,
    int RoomId,
    string RoomName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumNights,
    int NumPersons,
    decimal TotalPrice)
{
    public override string ToString()
    {

        return $"""
            ============================================================
                          BOKNINGSBEKRÄFTELSE #{BookingId}
            ============================================================
              Hotell:      {HotelName} ({City})
              Rum:         #{RoomId} - {RoomName}
              Gäster:      {NumPersons} pers
              Incheckning: {CheckIn:yyyy-MM-dd HH:mm}
              Utcheckning: {CheckOut:yyyy-MM-dd HH:mm} ({NumNights} nätter)
            ------------------------------------------------------------
              TOTALPRIS:   {TotalPrice:0.##} kr
            ============================================================
            """;
    }
}