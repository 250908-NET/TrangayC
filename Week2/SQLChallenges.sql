-- SETUP:
    -- Create a database server (docker)
        -- docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
    -- Connect to the server (Azure Data Studio / Database extension)
    -- Test your connection with a simple query (like a select)
    -- Execute the Chinook database (to create Chinook resources in your db)

    

-- On the Chinook DB, practice writing queries with the following exercises

-- BASIC CHALLENGES
USE MyDatabase;
GO
-- List all customers (full name, customer id, and country) who are not in the USA
SELECT FirstName, LastName, CustomerId, Country
FROM dbo.Customer
WHERE Country != 'USA';
-- List all customers from Brazil
SELECT FirstName, LastName, CustomerId, Country
FROM dbo.Customer
WHERE Country = 'Brazil';
-- List all sales agents
SELECT FirstName, LastName, EmployeeId
FROM dbo.Employee
WHERE Title = 'Sales Support Agent';
-- Retrieve a list of all countries in billing addresses on invoices
SELECT DISTINCT BillingCountry
FROM dbo.Invoice;
-- Retrieve how many invoices there were in 2009, and what was the sales total for that year?
SELECT COUNT(InvoiceId) as InvoiceCount, SUM(Total) as SalesTotal
FROM dbo.Invoice
WHERE InvoiceDate BETWEEN '2009-01-01' AND '2009-12-31';
    -- (challenge: find the invoice count sales total for every year using one query)
SELECT COUNT(InvoiceId) as InvoiceCount, SUM(Total) as SalesTotal, YEAR(InvoiceDate) as InvoiceYear
FROM dbo.Invoice
GROUP BY YEAR(InvoiceDate);
-- how many line items were there for invoice #37
SELECT COUNT(InvoiceLineId) as LineItemCount
FROM dbo.InvoiceLine
WHERE InvoiceId = 37;
-- how many invoices per country? BillingCountry  # of invoices -
SELECT BillingCountry, COUNT(InvoiceId) as InvoiceCount
FROM dbo.Invoice
GROUP BY BillingCountry;
-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT BillingCountry, SUM(Total) as SalesTotal
FROM dbo.Invoice
GROUP BY BillingCountry
ORDER BY SalesTotal DESC;
-- JOINS CHALLENGES
-- Every Album by Artist
SELECT Album.Title, Artist.Name
FROM dbo.Album
JOIN dbo.Artist
ON Album.ArtistId = Artist.ArtistId;
-- All songs of the rock genre
SELECT Track.Name, Genre.Name
FROM dbo.Track
JOIN dbo.Genre
ON Track.GenreId = Genre.GenreId
WHERE Genre.Name = 'Rock';
-- Show all invoices of customers from brazil (mailing address not billing)
SELECT *
FROM dbo.Invoice
JOIN dbo.Customer
ON Invoice.CustomerId = Customer.CustomerId
WHERE Customer.Country = 'Brazil';
-- Show all invoices together with the name of the sales agent for each one
SELECT Invoice.*, Employee.FirstName + ' ' + Employee.LastName as SalesAgent
FROM dbo.Invoice
JOIN dbo.Customer
ON Invoice.CustomerId = Customer.CustomerId
JOIN dbo.Employee
ON Customer.SupportRepId = Employee.EmployeeId;
-- Which sales agent made the most sales in 2009?
SELECT Employee.FirstName + ' ' + Employee.LastName as SalesAgent, SUM(Invoice.Total) as InvoiceTotal
FROM dbo.Invoice
JOIN dbo.Customer
ON Invoice.CustomerId = Customer.CustomerId
JOIN dbo.Employee
ON Customer.SupportRepId = Employee.EmployeeId
WHERE InvoiceDate BETWEEN '2009-01-01' AND '2009-12-31'
GROUP BY Employee.FirstName, Employee.LastName
ORDER BY InvoiceTotal DESC;
-- How many customers are assigned to each sales agent?
SELECT Employee.FirstName + ' ' + Employee.LastName as SalesAgent, COUNT(Customer.SupportRepId) as CustomerCount
FROM dbo.Customer
JOIN dbo.Employee
ON Customer.SupportRepId = Employee.EmployeeId
GROUP BY Employee.FirstName, Employee.LastName;
-- Which track was purchased the most ing 20010?
SELECT Track.Name, SUM(InvoiceLine.Quantity) as Quantity
FROM dbo.Track
JOIN dbo.InvoiceLine
ON Track.TrackId = InvoiceLine.TrackId
JOIN dbo.Invoice
ON InvoiceLine.InvoiceId = Invoice.InvoiceId
WHERE InvoiceDate BETWEEN '2010-01-01' AND '2010-12-31'
GROUP BY Track.Name
ORDER BY Quantity DESC;
-- Show the top three best selling artists.
SELECT TOP 3 Artist.Name, SUM(InvoiceLine.Quantity) as Quantity
FROM dbo.Artist
JOIN dbo.Album
ON Artist.ArtistId = Album.ArtistId
JOIN dbo.Track
ON Album.AlbumId = Track.AlbumId
JOIN dbo.InvoiceLine
ON Track.TrackId = InvoiceLine.TrackId
JOIN dbo.Invoice
ON InvoiceLine.InvoiceId = Invoice.InvoiceId
GROUP BY Artist.Name
ORDER BY Quantity DESC;
-- Which customers have the same initials as at least one other customer?
SELECT 
    c1.CustomerId,
    c1.FirstName + ' ' + c1.LastName as Customer1Name,
    c2.CustomerId,
    c2.FirstName + ' ' + c2.LastName as Customer2Name,
    UPPER(LEFT(c1.FirstName, 1) + ' ' + LEFT(c1.LastName, 1)) as Initials
FROM dbo.Customer AS c1
JOIN dbo.Customer AS c2
ON UPPER(LEFT(c1.FirstName, 1) + ' ' + LEFT(c1.LastName, 1)) = UPPER(LEFT(c2.FirstName, 1) + ' ' + LEFT(c2.LastName, 1))
AND c1.CustomerId != c2.CustomerId
ORDER BY Initials;
-- ADVACED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?
SELECT Artist.Name
FROM dbo.Artist
WHERE Artist.ArtistId NOT IN (SELECT Album.ArtistId FROM dbo.Album);
-- 2. which artists did not record any tracks of the Latin genre?
SELECT Artist.Name
FROM dbo.Artist
WHERE Artist.ArtistId NOT IN (
    SELECT Artist.ArtistId
    FROM dbo.Artist
    JOIN dbo.Album
    ON Artist.ArtistId = Album.ArtistId
    JOIN dbo.Track
    ON Album.AlbumId = Track.AlbumId
    JOIN dbo.Genre
    ON Track.GenreId = Genre.GenreId
    WHERE Genre.Name = 'Latin'
)
-- 3. which video track has the longest length? (use media type table)
SELECT TOP 1 Track.Name, MediaType.Name, Track.Milliseconds
FROM dbo.Track
JOIN dbo.MediaType
ON Track.MediaTypeId = MediaType.MediaTypeId
ORDER BY Track.Milliseconds DESC;
-- 4. find the names of the customers who live in the same city as the
--    boss employee (the one who reports to nobody)
SELECT Customer.FirstName + ' ' + Customer.LastName as CustomerName, Employee.FirstName + ' ' + Employee.LastName as BossName, Customer.City
FROM dbo.Customer
JOIN dbo.Employee
ON Customer.City = Employee.City 
WHERE Employee.ReportsTo IS NULL;
-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?
SELECT COUNT(InvoiceLine.TrackId) as TrackCount, SUM(Invoice.Total) as TotalPrice
FROM dbo.InvoiceLine
JOIN dbo.Invoice
ON InvoiceLine.InvoiceId = Invoice.InvoiceId
JOIN dbo.Customer
ON Invoice.CustomerId = Customer.CustomerId
WHERE Customer.Country = 'Germany';
-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.
SELECT Customer.FirstName + ' ' + Customer.LastName as CustomerName, Customer.Country, Employee.FirstName + ' ' + Employee.LastName as EmployeeName, Employee.HireDate
FROM dbo.Customer
JOIN dbo.Employee
ON Customer.SupportRepId = Employee.EmployeeId
WHERE DATEDIFF(YEAR, Employee.HireDate, GETDATE()) < 35;

-- DML exercises

-- 1. insert two new records into the employee table.
INSERT INTO dbo.Employee (EmployeeId, FirstName, LastName, Title, ReportsTo, BirthDate, HireDate, Address, City, State, Country, PostalCode, Phone, Fax, Email)
VALUES
    (54, 'John', 'Doe', 'Sales Support Agent', 2, '1990-01-01', '2010-01-01', '123 Main St', 'Piscataway', 'NJ', 'USA', '12345', '(123) 456-7890', '(123) 456-7891', 'temp@example.com'),
    (55, 'Jane', 'Doe', 'Sales Engineer Agent', 2, '1992-01-01', '2012-01-01', '401 Main St', 'Edison', 'NJ', 'USA', '12345', '(123) 456-7890', '(123) 456-7891', 'temp2@example.com');
-- 2. insert two new records into the tracks table.
INSERT INTO dbo.Track (TrackId, Name, AlbumId, MediaTypeId, GenreId, Composer, Milliseconds, Bytes, UnitPrice)
VALUES
    (54, 'Track 1', 1, 1, 1, 'Composer 1', 1000, 1000, 1.00),
    (55, 'Track 2', 1, 1, 1, 'Composer 2', 2000, 2000, 1.00);
-- 3. update customer Aaron Mitchell's name to Robert Walter
UPDATE dbo.Customer
SET FirstName = 'Robert', LastName = 'Walter'
WHERE FirstName = 'Aaron' AND LastName = 'Mitchell';
-- 4. delete one of the employees you inserted.
DELETE FROM dbo.Employee
WHERE FirstName = 'John' AND LastName = 'Doe';
-- 5. delete customer Robert Walter.
DELETE FROM dbo.Customer
WHERE FirstName = 'Robert' AND LastName = 'Walter';
