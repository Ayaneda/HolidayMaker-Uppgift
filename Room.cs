public sealed record Room(
    int Id,
    string Name,
    int Persons_capacity,
    string Hotel,
    string City,
    decimal Price,
    int Rating,
    bool Has_pool,
    bool Has_restaurant,
    int Distance_to_beach,
    int Distance_to_city_center)
{
    public override string ToString()
    {
        string pool = Has_pool ? "Ja" : "Nej";
        string restaurant = Has_restaurant ? "Ja" : "Nej";

        return $"""
            Rum #{Id}: {Name} ({Persons_capacity} pers) - {Price:0.##} kr/natt
              Hotell: {Hotel} ({City}) | Betyg: {Rating}/5
              Pool: {pool} | Restaurang: {restaurant}
              Avstånd till strand: {Distance_to_beach} m | Avstånd till centrum: {Distance_to_city_center} m
            ------------------------------------------------------------
            """;
    }
}