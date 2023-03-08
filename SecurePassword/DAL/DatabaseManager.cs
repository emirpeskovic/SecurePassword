using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SecurePassword.Models.Common;

namespace SecurePassword.DAL;

/// <summary>
///     The DatabaseManager class.
///     This class is used to create a context to the database.
///     It also provides a method to perform actions on the database.
/// </summary>
public class DatabaseManager
{
    /// <summary>
    ///     Our connection string.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    ///     Our logger.
    /// </summary>
    private readonly ILogger<DatabaseManager> _logger;

    /// <summary>
    ///     Constructor for the DatabaseManager.
    /// </summary>
    /// <param name="config">The configuration from appsettings.json</param>
    /// <param name="logger">The logger</param>
    public DatabaseManager(IConfiguration config, ILogger<DatabaseManager> logger)
    {
        // Get the database configuration from appsettings.json
        var section = config.GetSection("Database");
        var host = section["DB_HOST"];
        var port = section["DB_PORT"];
        var user = section["DB_USER"];
        var password = section["DB_PASS"];
        var database = section["DB_NAME"];

        // Create the connection string
        _connectionString = $"User ID={user};Password={password};Host={host};Port={port};Database={database};Pooling=true;";

        // Set the logger
        _logger = logger;
    }

    /// <summary>
    ///     Creates a new context for the database.
    /// </summary>
    /// <returns>DatabaseContext as DbContext</returns>
    private DbContext CreateContext()
    {
        // We use a try-catch block to catch any exceptions that may occur
        try
        {
            // Create a new DbContextOptionsBuilder and use the connection string
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseNpgsql(_connectionString);

            // Create a new context and return it
            return new DatabaseContext(optionsBuilder.Options);
        }
        catch (DbException dbe)
        {
            // If an exception occurs, log it and throw it
            _logger.LogError(dbe, "Could not create context to database.");
            throw;
        }
    }

    /// <summary>
    ///     Uses the context to perform an action.
    ///     Will rollback changes if an exception is thrown.
    /// </summary>
    /// <param name="action">User-defined action</param>
    public void UseContext(Action<DbContext> action)
    {
        // We get a new context
        using var context = CreateContext();

        // We try to perform the action and use try-catch to catch any exceptions
        try
        {
            // We perform the action
            action.Invoke(context);

            // We save the changes
            context.SaveChanges();
        }
        catch (DbException dbe)
        {
            // If an exception occurs, we rollback the transaction
            if (context.Database.CurrentTransaction != null)
                context.Database.RollbackTransaction();

            // We also log the exception
            _logger.LogError(dbe, "Could not save changes to database.");
        }
    }

    /// <summary>
    ///     Uses the context asynchronously to perform an action.
    ///     Will rollback changes if an exception is thrown.
    /// </summary>
    /// <param name="action">User-defined action</param>
    public async Task UseContextAsync(Action<DbContext> action)
    {
        // We get a new context
        await using var context = CreateContext();

        // We try to perform the action and use try-catch to catch any exceptions
        try
        {
            // We perform the action
            action.Invoke(context);

            // We save the changes
            await context.SaveChangesAsync();
        }
        catch (DbException dbe)
        {
            // If an exception occurs, we rollback the transaction
            if (context.Database.CurrentTransaction != null)
                await context.Database.RollbackTransactionAsync();

            // We also log the exception
            _logger.LogError(dbe, "Could not save changes to database.");
        }
    }

    /// <summary>
    ///     Returns a single entity from the database.
    /// </summary>
    /// <param name="match">If defined, tries to find an entity with the matched predicate.</param>
    /// <typeparam name="T">The entity that implements IEntity</typeparam>
    /// <returns>The entity</returns>
    public T? GetOne<T>(Func<T, bool>? match = null) where T : class, IEntity
    {
        // We get a new context
        var context = (DatabaseContext)CreateContext();

        // We return the entity if it exists, otherwise we return null
        return context.Entity<T>().FirstOrDefault(match ?? (_ => true));
    }

    /// <summary>
    ///     Returns an IEnumerable of entities from the database.
    /// </summary>
    /// <param name="page">How many entities to skip * count</param>
    /// <param name="count">The amount of entities to take</param>
    /// <typeparam name="T">The entity type</typeparam>
    /// <returns>An IEnumerable of entity</returns>
    public IEnumerable<T> GetMany<T>(int page = 0, int count = 0) where T : class, IEntity
    {
        // We get a new context
        var context = (DatabaseContext)CreateContext();

        // We return all entities if count is 0, otherwise we return a page of entities
        return count == 0 ? context.Entity<T>() : context.Entity<T>().Skip(page * count).Take(count);
    }
    
    public void Add<T>(T entity) where T : class, IEntity
    {
        UseContext(context => context.Add(entity));
    }
}