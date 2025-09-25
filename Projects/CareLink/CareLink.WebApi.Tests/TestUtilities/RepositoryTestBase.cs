using System;
using CareLink.WebApi.Data;
using Microsoft.Data.Sqlite;

namespace CareLink.WebApi.Tests.TestUtilities;

/// <summary>
/// Base class for repository integration tests using a shared Sqlite in-memory database per test.
/// Ensures proper creation and disposal of the DbContext and underlying connection.
/// </summary>
public abstract class RepositoryTestBase : IDisposable
{
    protected readonly CareLinkDbContext Ctx;
    private readonly SqliteConnection _conn;

    protected RepositoryTestBase()
    {
        (Ctx, _conn) = TestDbFactory.CreateSqliteInMemoryDb();
    }

    public void Dispose()
    {
        TestDbFactory.Dispose(Ctx, _conn);
    }
}
