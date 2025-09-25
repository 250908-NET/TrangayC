using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using CareLink.WebApi.Data;

namespace CareLink.WebApi.Tests.TestUtilities;

public static class TestDbFactory
{
    public static (CareLinkDbContext context, SqliteConnection connection) CreateSqliteInMemoryDb()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<CareLinkDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new CareLinkDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }

    public static void Dispose(CareLinkDbContext context, SqliteConnection connection)
    {
        context.Dispose();
        connection.Close();
        connection.Dispose();
    }
}
