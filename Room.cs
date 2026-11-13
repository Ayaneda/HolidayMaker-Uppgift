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
    int Distance_to_city_center);