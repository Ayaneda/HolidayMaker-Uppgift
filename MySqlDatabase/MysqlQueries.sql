# Mysql queries

-- 1. Registrera en kund
INSERT INTO customers (name, last_name, email, telephone) VALUES
    ('Ulf', 'Nilsson', 'ulf.nils@example.com', '0735486510');

# Check all customers

#Select en existerande kund
SELECT * FROM customers
    WHERE email = 'ulf.nils@example.com' OR telephone = '0735486510' ;

-- 2. Söka lediga rum genom att mata ort, incheckingsdatum, utcheckingsdatum och antal personer.
-- Resultad ska vara visa tillgängliga rum.
SELECT r.id AS 'Room number',
        r.name AS Name,
        r.persons_capacity AS Capacity,
        h.name AS Hotel ,
        h.city AS City,
        r.price AS 'Price/night in kr',
        has_pool AS Pool,
        has_restaurant AS Restaurant,
        distance_to_beach AS 'Distance to the beach',
        distance_to_city_center AS 'Distance to city center'
    FROM rooms r
    JOIN hotels h on r.hotel_id = h.id
    WHERE
        h.city = 'Palma'
        AND r.persons_capacity >= '1'
        AND NOT EXISTS(SELECT *
                       FROM bookings bk
                       WHERE room_id = r.id
                            AND bk.end_date > '2026-12-14 14:00:00'
                            AND bk.start_date < '2026-12-18 11:00:00'
        )
        AND distance_to_beach <= 400
        AND distance_to_city_center <= 1000
ORDER BY price ASC;

-- 3. Boka till kunden efter filtrering
INSERT INTO bookings (room_id, customer_id, start_date, end_date, num_persons)
SELECT r.id, 4, '2026-12-14 14:00:00', '2026-12-18 11:00:00', 4
FROM rooms r
    WHERE r.id = 6
    AND persons_capacity >= 4
        AND NOT EXISTS (
        SELECT 1
        FROM bookings bk
        WHERE bk.room_id = 6
          AND bk.end_date > '2026-12-14 14:00:00'
          AND bk.start_date < '2026-12-18 11:00:00'
);


-- Kontrollera att man inte overlappa bokningar - return 0 rad
INSERT INTO bookings (room_id, customer_id, start_date, end_date, num_persons)
SELECT r.id, 4, '2026-12-16 14:00:00', '2026-12-20 11:00:00', 4
FROM rooms r
    WHERE r.id = 6
    AND r.persons_capacity >= 4
    AND NOT EXISTS (
    SELECT 1
    FROM bookings bk
    WHERE bk.room_id = 6
      AND bk.end_date > '2026-12-16 14:00:00'
      AND bk.start_date < '2026-12-20 11:00:00'
);



-- Bekräftelse of boking
SELECT bookings.id AS 'Reservation number',
        SUM(price * DATEDIFF(bookings.end_date, bookings.start_date)) AS 'Total price'
        FROM bookings
        JOIN rooms r
        on bookings.room_id = r.id
        WHERE bookings.id = last_insert_id()
        GROUP BY bookings.id;

-- 4. Visa kundens bokningar
SELECT bookings.id AS 'Reservation number',
       h.name AS Hotel,
       h.city,
       bookings.room_id AS Room,
       r.name AS 'Room name',
       bookings.start_date AS 'From',
       bookings.end_date AS 'To',
       bookings.num_persons AS 'Number of persons',
       DATEDIFF(bookings.end_date, bookings.start_date) AS Nights,
       SUM(price * DATEDIFF(bookings.end_date, bookings.start_date)) AS 'Total price'
FROM bookings
         JOIN rooms r on bookings.room_id = r.id
         JOIN hotels h on r.hotel_id = h.id
WHERE customer_id = 4
GROUP BY bookings.id;

-- 5. Ändra antal personer för existerande bokning
UPDATE bookings
JOIN rooms ON bookings.room_id = rooms.id
SET bookings.num_persons = 3
WHERE bookings.id = 29
  AND rooms.persons_capacity >= 3;

-- 6. Avboka
DELETE FROM bookings WHERE bookings.id = 29;


-- jämföra resultad med c#
SELECT r.id AS 'Room number',
        r.name AS Name,
        r.persons_capacity AS Capacity,
        h.name AS Hotel ,
        h.city AS City,
        r.price AS 'Price/night in kr',
        has_pool AS Pool,
        has_restaurant AS Restaurant,
        distance_to_beach AS 'Distance to the beach',
        distance_to_city_center AS 'Distance to city center'
    FROM rooms r
    JOIN hotels h on r.hotel_id = h.id
    WHERE
        h.city = 'Palma'
        AND r.persons_capacity >= '1'
        AND NOT EXISTS(SELECT *
                       FROM bookings bk
                       WHERE room_id = r.id
                            AND bk.end_date > '2026-12-14 14:00:00'
                            AND bk.start_date < '2026-12-18 11:00:00'
        )
ORDER BY price ASC;




-- Query to C#
SELECT r.id AS 'Room number', r.name AS Name,
            r.persons_capacity AS Capacity,
            h.name AS Hotel,
            h.city AS City,
            r.price AS 'Price/night in kr',
            h.rating AS Rating,
            h.has_pool AS Pool,
            h.has_restaurant AS Restaurant,
            h.distance_to_beach AS 'Distance to the beach',
            h.distance_to_city_center AS 'Distance to city center'
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
                AND bk.start_date < @check_out
                AND bk.end_date   > @check_in
                )
            AND (@has_pool IS NULL OR h.has_pool = @has_pool)
            AND (@has_restaurant IS NULL OR h.has_restaurant = @has_restaurant)
            AND (@distance_to_beach IS NULL OR h.distance_to_beach <= @distance_to_beach)
            AND (@distance_to_center IS NULL OR h.distance_to_city_center <= @distance_to_center)
            ORDER BY
                CASE WHEN @Sortby = 'price' THEN r.price END ASC,
                CASE WHEN @Sortby = 'rating' THEN h.rating END DESC;

-- Kolla efter ort
SELECT h.name, h.city FROM hotels h;

