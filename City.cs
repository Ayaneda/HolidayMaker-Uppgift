public sealed record HotelCity (string HotelName, string CityName)
{
    public override string ToString()
    {
        return $"{HotelName} ({CityName})";
    }
}