-- Jag skapade databas själv
CREATE DATABASE holiday_maker;

-- Jag create a user
CREATE USER 'operator'@'%'
IDENTIFIED BY 'abcd1234';
GRANT SELECT, INSERT
ON holiday_maker.customers
TO 'operator'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE
ON holiday_maker.bookings
TO 'operator'@'%';
GRANT SELECT
ON holiday_maker.hotels
TO 'operator'@'%';
GRANT SELECT
ON holiday_maker.rooms
TO 'operator'@'%';


CREATE TABLE hotels(
    id INT AUTO_INCREMENT
        PRIMARY KEY ,
    name VARCHAR(60) NOT NULL,
    city VARCHAR(60) NOT NULL,
    has_pool BOOL NULL,
    has_restaurant BOOL NULL,
    distance_to_beach INT CHECK (distance_to_beach > 0 ) NOT NULL,
    distance_to_city_center INT CHECK (distance_to_city_center > 0 ) NOT NULL,
    rating INT CHECK ( rating BETWEEN 1 AND 5)
);

CREATE TABLE rooms(
    id INT AUTO_INCREMENT
        PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    persons_capacity INT NOT NULL,
    price DECIMAL (10, 2) NOT NULL,
    hotel_id INT NOT NULL,
    CONSTRAINT rooms_hotels_id_fk #rooms (this table) hotels(table you want to join) id (hotels table, id is the column) fk(definition)
        FOREIGN KEY (hotel_id) REFERENCES hotels (id)
);


CREATE TABLE customers(
    id INT AUTO_INCREMENT
        PRIMARY KEY,
    name VARCHAR(60) NOT NULL,
    last_name VARCHAR(60) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    telephone VARCHAR(30) UNIQUE NULL,
    CONSTRAINT chk_customer_email CHECK (REGEXP_LIKE(email, '^[^@]+@[^@]+\.[^@]+$'))

);


CREATE TABLE bookings(
    id INT AUTO_INCREMENT
        PRIMARY KEY ,
    customer_id INT NOT NULL,
    room_id INT NOT NULL,
    start_date DATETIME NOT NULL,
    end_date DATETIME NOT NULL,
    num_persons INT NOT NULL,
        CONSTRAINT bookings_customers_id_fk
            FOREIGN KEY (customer_id) REFERENCES customers(id),
        CONSTRAINT bookings_rooms_id_fk
            FOREIGN KEY (room_id) REFERENCES rooms (id)
);

-- Create of AI after database structure
-- ========================================================
-- 1. HOTELL (5 st fördelade på 3 orter: Palma, Nice, Split)
-- ========================================================
INSERT INTO hotels (name, city, has_pool, has_restaurant, distance_to_beach, distance_to_city_center, rating) VALUES
('Hotel Sol y Mar', 'Palma', TRUE, TRUE, 150, 800, 4),
('Palma Boutique Suites', 'Palma', FALSE, TRUE, 900, 200, 5),
('Riviera Beach Resort', 'Nice', TRUE, TRUE, 50, 1200, 4),
('Hotel Promenade', 'Nice', FALSE, FALSE, 300, 450, 3),
('Adriatic Pearl Hotel', 'Split', TRUE, TRUE, 120, 600, 5);

-- ========================================================
-- 2. RUM (20 st fördelade på hotellen med kapacitet 1, 2 och 4 personer)
-- ========================================================
INSERT INTO rooms (name, persons_capacity, price, hotel_id) VALUES
-- Hotel Sol y Mar (hotel_id = 1)
('Standard Enkel', 1, 750.00, 1),
('Standard Dubbel Havsutsikt', 2, 1250.00, 1),
('Superior Dubbel', 2, 1450.00, 1),
('Familjesvit Sol', 4, 2200.00, 1),

-- Palma Boutique Suites (hotel_id = 2)
('Boutique Enkel', 1, 850.00, 2),
('Classic Queen', 2, 1350.00, 2),
('Deluxe King', 2, 1600.00, 2),
('Boutique Familjesvit', 4, 2500.00, 2),

-- Riviera Beach Resort (hotel_id = 3)
('Budget Enkel', 1, 950.00, 3),
('Beach Double', 2, 1800.00, 3),
('Premium Balcony', 2, 2100.00, 3),
('Grand Family Suite', 4, 3200.00, 3),

-- Hotel Promenade (hotel_id = 4)
('Promenade Single', 1, 650.00, 4),
('Promenade Double', 2, 1050.00, 4),
('Twin Bed Room', 2, 1050.00, 4),
('Quad Room', 4, 1750.00, 4),

-- Adriatic Pearl Hotel (hotel_id = 5)
('Pearl Single', 1, 700.00, 5),
('Pearl Double Sea View', 2, 1300.00, 5),
('Executive Double', 2, 1550.00, 5),
('Pearl Family Residence', 4, 2400.00, 5);

-- ========================================================
-- 3. KUNDER (5 fiktiva kunder med giltiga e-postadresser)
-- ========================================================
INSERT INTO customers (name, last_name, email, telephone) VALUES
('Anna', 'Andersson', 'anna.andersson@example.se', '0701122334'),
('Erik', 'Lindgren', 'erik.lindgren@example.com', '0732233445'),
('Sofia', 'Nilsson', 'sofia.nilsson@example.se', '0763344556'),
('Johan', 'Berg', 'johan.berg@example.com', '0709988776'),
('Maria', 'Svensson', 'maria.svensson@example.se', '0724455667');

-- ========================================================
-- 4. BOKNINGAR (25 st i december 2026 med varierande längd och inbyggda luckor)
-- ========================================================
INSERT INTO bookings (customer_id, room_id, start_date, end_date, num_persons) VALUES
-- Början av december (1–12 dec)
(1, 1,  '2026-12-01 14:00:00', '2026-12-04 11:00:00', 1),
(2, 3,  '2026-12-01 14:00:00', '2026-12-05 11:00:00', 2),
(3, 4,  '2026-12-02 14:00:00', '2026-12-08 11:00:00', 4),
(4, 5,  '2026-12-03 14:00:00', '2026-12-06 11:00:00', 1),
(5, 7,  '2026-12-04 14:00:00', '2026-12-07 11:00:00', 2),
(1, 8,  '2026-12-05 14:00:00', '2026-12-12 11:00:00', 4),
(2, 10, '2026-12-05 14:00:00', '2026-12-07 11:00:00', 2),
(3, 11, '2026-12-06 14:00:00', '2026-12-09 11:00:00', 2),
(4, 12, '2026-12-07 14:00:00', '2026-12-14 11:00:00', 3),
(5, 1,  '2026-12-08 14:00:00', '2026-12-12 11:00:00', 1),

-- Mitten av december (13–21 dec)
(1, 2,  '2026-12-14 14:00:00', '2026-12-21 11:00:00', 2),
(2, 6,  '2026-12-14 14:00:00', '2026-12-18 11:00:00', 2),
(3, 9,  '2026-12-15 14:00:00', '2026-12-19 11:00:00', 1),
(4, 13, '2026-12-15 14:00:00', '2026-12-18 11:00:00', 1),
(5, 14, '2026-12-16 14:00:00', '2026-12-20 11:00:00', 2),
(1, 15, '2026-12-17 14:00:00', '2026-12-21 11:00:00', 2),
(2, 16, '2026-12-18 14:00:00', '2026-12-23 11:00:00', 4),
(3, 17, '2026-12-18 14:00:00', '2026-12-22 11:00:00', 1),
(4, 18, '2026-12-19 14:00:00', '2026-12-24 11:00:00', 2),

-- Jul & Nyår (20–31 dec)
(5, 4,  '2026-12-21 14:00:00', '2026-12-27 11:00:00', 4),
(1, 10, '2026-12-22 14:00:00', '2026-12-28 11:00:00', 2),
(2, 1,  '2026-12-23 14:00:00', '2026-12-26 11:00:00', 1),
(3, 6,  '2026-12-23 14:00:00', '2026-12-29 11:00:00', 2),
(4, 8,  '2026-12-24 14:00:00', '2026-12-30 11:00:00', 4),
(5, 14, '2026-12-27 14:00:00', '2026-12-31 11:00:00', 2);

