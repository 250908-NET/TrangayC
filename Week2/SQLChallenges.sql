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
SELECT *
FROM dbo.Invoice
JOIN dbo.Customer
ON Invoice.CustomerId = Customer.CustomerId
JOIN dbo.Employee
ON Customer.SupportRepId = Employee.EmployeeId;
-- Which sales agent made the most sales in 2009?

-- How many customers are assigned to each sales agent?

-- Which track was purchased the most ing 20010?

-- Show the top three best selling artists.

-- Which customers have the same initials as at least one other customer?



-- ADVACED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?

-- 2. which artists did not record any tracks of the Latin genre?

-- 3. which video track has the longest length? (use media type table)

-- 4. find the names of the customers who live in the same city as the
--    boss employee (the one who reports to nobody)

-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?

-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.


-- DML exercises

-- 1. insert two new records into the employee table.

-- 2. insert two new records into the tracks table.

-- 3. update customer Aaron Mitchell's name to Robert Walter

-- 4. delete one of the employees you inserted.

-- 5. delete customer Robert Walter.
