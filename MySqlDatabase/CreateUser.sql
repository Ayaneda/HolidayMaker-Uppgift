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
