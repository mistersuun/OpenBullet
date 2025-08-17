using System.Linq.Expressions;

namespace OpenBullet.Core.Data;

/// <summary>
/// Generic repository interface for database operations
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets an entity by its ID
    /// </summary>
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching a predicate
    /// </summary>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single entity matching a predicate
    /// </summary>
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by ID
    /// </summary>
    Task<bool> DeleteByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entities matching a predicate
    /// </summary>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching a predicate
    /// </summary>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entities match a predicate
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities with pagination
    /// </summary>
    Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities ordered by a key
    /// </summary>
    Task<IEnumerable<TEntity>> GetOrderedAsync<TKey>(Expression<Func<TEntity, TKey>> orderBy, bool descending = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// Paged result wrapper
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Database context interface
/// </summary>
public interface IOpenBulletContext
{
    /// <summary>
    /// Configurations repository
    /// </summary>
    IRepository<ConfigurationEntity> Configurations { get; }

    /// <summary>
    /// Jobs repository
    /// </summary>
    IRepository<JobEntity> Jobs { get; }

    /// <summary>
    /// Job results repository
    /// </summary>
    IRepository<JobResultEntity> JobResults { get; }

    /// <summary>
    /// Proxies repository
    /// </summary>
    IRepository<ProxyEntity> Proxies { get; }

    /// <summary>
    /// Settings repository
    /// </summary>
    IRepository<SettingEntity> Settings { get; }

    /// <summary>
    /// Saves all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes raw SQL
    /// </summary>
    Task<int> ExecuteSqlAsync(string sql, object[]? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Backs up the database
    /// </summary>
    Task<bool> BackupAsync(string backupPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores the database from backup
    /// </summary>
    Task<bool> RestoreAsync(string backupPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes the database
    /// </summary>
    Task OptimizeAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Database transaction interface
/// </summary>
public interface IDbTransaction : IDisposable
{
    /// <summary>
    /// Commits the transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Database configuration options
/// </summary>
public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SQLite;
    public bool EnableLogging { get; set; } = false;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public int CommandTimeout { get; set; } = 30;
    public bool AutoMigrate { get; set; } = true;
    public string BackupDirectory { get; set; } = "Backups";
    public TimeSpan BackupInterval { get; set; } = TimeSpan.FromHours(24);
    public int MaxBackupCount { get; set; } = 7;
    public Dictionary<string, object> ProviderOptions { get; set; } = new();
}

/// <summary>
/// Supported database providers
/// </summary>
public enum DatabaseProvider
{
    SQLite,
    LiteDB,
    InMemory
}

/// <summary>
/// Query filter for database operations
/// </summary>
public class QueryFilter
{
    public int? Limit { get; set; }
    public int? Offset { get; set; }
    public string? OrderBy { get; set; }
    public bool OrderDescending { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new();
}

/// <summary>
/// Database health information
/// </summary>
public class DatabaseHealth
{
    public bool IsHealthy { get; set; }
    public string Version { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int TableCount { get; set; }
    public DateTime LastBackup { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public Dictionary<string, int> RecordCounts { get; set; } = new();
    public List<string> Issues { get; set; } = new();
}
