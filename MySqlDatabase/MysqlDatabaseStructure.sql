-- Jag skapade databas själv
CREATE DATABASE holiday_maker;

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