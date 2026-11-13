# Holiday Maker – Bokningssystem (MAI26) 

En C#-konsolapplikation med MySQL-databas för en resebyråoperatör som hanterar kunder, söker lediga semesterboenden och administrerar rumsbokningar.

---

## Funktioner

1. **Se kunds bokningar:** Sök fram en kunds alla bokningar via kund-ID (visar bokningsnummer, hotell, stad, rum, in-/utcheckning, antal nätter, antal personer och totalpris).
2. **Skapa ny bokning (Steg-för-steg-flöde):**
   - Söker befintlig kund via telefonnummer eller registrerar en ny kund om telefonnumret saknas.
   - Visar tillgängliga hotell och städer.
   - Söker lediga rum utifrån stad, incheckningsdatum, utcheckningsdatum, antal personer samt valfria filter (pool, restaurang, maxavstånd till strand/centrum) och sortering (pris per natt stigande eller betyg fallande).
   - Kontrollerar tillgänglighet och kapacitet vid bokningstillfället och visar bokningsbekräftelse med totalpris.
3. **Uppdatera bokning:** Ändra antal personer i en befintlig bokning (med kontroll mot rummets maxkapacitet) och visa den uppdaterade bokningen.
4. **Ta bort bokning (Avboka):** Visa kundens bokningar, ta bort vald bokning så att rummet frigörs och visa kundens uppdaterade bokningslista.
5. **Avbryt till huvudmeny:** Operatören kan i vilket steg som helst skriva `Q` för att avbryta pågående inmatning och återgå till huvudmenyn.

---

## Steg 1: Skapa databasen och lägg in testdata

Alla SQL-filer finns i mappen `MySqlDatabase`:

1. Öppna DataGrip.
2. Kopiera hela `CreateAllDataInOneGo.sql`innehåll för att i ett enda steg:
   - Skapa databasen `holiday_maker`.
   - Skapa databasanvändaren `operator` med begränsade rättigheter (*Principle of Least Privilege* – endast `SELECT` på `hotels`/`rooms`, `SELECT, INSERT` på `customers` samt `SELECT, INSERT, UPDATE, DELETE` på `bookings`).
   - Skapa tabellerna (`hotels`, `rooms`, `customers`, `bookings`) med `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE` och `CHECK`-begränsningar (inklusive `REGEXP_LIKE` för e-postvalidering).
   - Lägga in testdata ((AI genererad) 5 hotell på 3 orter: Palma, Nice och Split, 20 rum med kapacitet 1, 2 och 4 personer, 5 kunder och 25 bokningar under december 2026).
3. Skapa en skript och klistra allt och kör. 
4. Skapa en ny => Data Source => MySql => in user: skriv `operator` => losenord: `abcd1234` => kontrollera att port är: `3306 => Testa koppling => OK => Om inte visas databas => höger klick på ny data source => properties => Schemes => check on holiday_maker.

* Det finns utdelad filer om man vill kolla vad för sig.

---

## Steg 2: Kör programmet

Öppna en terminal i projektmappen och kör:

```bash
dotnet run
```

---

## Projektstruktur

- `Program.cs` – Startar applikationen, öppnar MySQL-anslutningen med felhantering och startar `HolidayMakerApp`.
- `HolidayMakerApp.cs` – Huvudloop och affärsflöde som knyter ihop användargränssnittet med databasen.
- `ConsoleUi.cs` – Hanterar all konsolinmatning, validering (`TryParse`, datumkontroller, `Q` för att återgå till huvudmenyn) samt utskrifter.
- `HolidayMakerRepository.cs` – Innehåller alla parameteriserade SQL-frågor mot MySQL-databasen.
- `Customer.cs`, `City.cs`, `Room.cs`, `Booking.cs`– Datamodeller (`record`-typer) med formaterade `ToString()`-utskrifter.
- `MySqlDatabase/`– SQL-skript för databasschema, användarrättigheter (`operator`), testdata och testade SQL-queries.

---

## Genomförda manuella tester (båda i dataGrip och C#)

Lösningen är verifierad mot uppgiftens testkrav:

1. **Lediga och upptagna rum:** Sökning på en period där ett rum har en bokning filtrerar bort det bokade rummet via `NOT EXISTS`, medan lediga rum på samma ort visas.
2. **Samma dag för utcheckning och incheckning:** Eftersom utcheckning sätts till kl. `11:00:00` och incheckning till kl. `14:00:00` (`bk.end_date > TIMESTAMP(@check_in, '14:00:00')` och `bk.start_date < TIMESTAMP(@check_out, '11:00:00')`) kan en ny kund boka och checka in samma datum som en tidigare kund checkar ut.
3. **Överlappande bokningar nekas:** Både delvis överlappande perioder och perioder som helt omsluter en befintlig bokning fångas av villkoret `bk.end_date > @check_in AND bk.start_date < @check_out`, både vid sökning och i `CreateBookingAsync`.
4. **Kapacitetskontroll vid sökning och bokning:** Rum med `persons_capacity < @num_persons` visas inte i sökresultatet och kan inte heller bokas i `CreateBookingAsync`.
5. **Kombinerade sökfilter och sortering:** Valfria filter (`NULL`-hanterade i SQL för pool, restaurang, strandavstånd och centrumavstånd) kan kombineras fritt, och resultatet kan sorteras på både lägsta pris (`ASC`) och högsta betyg (`DESC`).
6. **Ändring av antal personer:** `GetReservationUpdateAsync` använder `UPDATE bookings JOIN rooms ... WHERE rooms.persons_capacity >= @new_num_persons`, vilket nekar uppdatering om det nya antalet överstiger rummets kapacitet.
7. **Avbokning frigör rummet:** När en bokning tas bort med `DELETE FROM bookings WHERE id = @booking_id` dyker rummet direkt upp som ledigt igen vid nästa sökning för samma period.
8. **Persistens:** Alla skapade kunder, bokningar, ändringar och avbokningar sparas i MySQL-databasen och finns kvar efter att programmet startats om.
