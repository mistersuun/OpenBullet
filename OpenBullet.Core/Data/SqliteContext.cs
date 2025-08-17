using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace OpenBullet.Core.Data;

/// <summary>
/// SQLite Entity Framework context
/// </summary>
public class SqliteContext : DbContext, IOpenBulletContext
{
    private readonly DatabaseOptions _options;
    private readonly ILogger<SqliteContext> _logger;

    public SqliteContext(DatabaseOptions options, ILogger<SqliteContext> logger)
    {
        _options = options;
        _logger = logger;
    }

    public DbSet<ConfigurationEntity> ConfigurationEntities { get; set; } = null!;
    public DbSet<JobEntity> JobEntities { get; set; } = null!;
    public DbSet<JobResultEntity> JobResultEntities { get; set; } = null!;
    public DbSet<ProxyEntity> ProxyEntities { get; set; } = null!;
    public DbSet<SettingEntity> SettingEntities { get; set; } = null!;

    // Repository implementations
    private IRepository<ConfigurationEntity>? _configurations;
    private IRepository<JobEntity>? _jobs;
    private IRepository<JobResultEntity>? _jobResults;
    private IRepository<ProxyEntity>? _proxies;
    private IRepository<SettingEntity>? _settings;

    public IRepository<ConfigurationEntity> Configurations => 
        _configurations ??= new SqliteRepository<ConfigurationEntity>(this, _logger);

    public IRepository<JobEntity> Jobs => 
        _jobs ??= new SqliteRepository<JobEntity>(this, _logger);

    public IRepository<JobResultEntity> JobResults => 
        _jobResults ??= new SqliteRepository<JobResultEntity>(this, _logger);

    public IRepository<ProxyEntity> Proxies => 
        _proxies ??= new SqliteRepository<ProxyEntity>(this, _logger);

    public IRepository<SettingEntity> Settings => 
        _settings ??= new SqliteRepository<SettingEntity>(this, _logger);

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (_options.Provider)
        {
            case DatabaseProvider.SQLite:
                optionsBuilder.UseSqlite(_options.ConnectionString);
                break;
                
            case DatabaseProvider.InMemory:
                // Use the connection string as the database name for InMemory
                var databaseName = string.IsNullOrEmpty(_options.ConnectionString) 
                    ? "OpenBulletInMemory" 
                    : _options.ConnectionString;
                optionsBuilder.UseInMemoryDatabase(databaseName);
                break;
                
            default:
                // Fallback to SQLite for backward compatibility
                optionsBuilder.UseSqlite(_options.ConnectionString);
                break;
        }

        if (_options.EnableLogging)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            optionsBuilder.EnableSensitiveDataLogging(_options.EnableSensitiveDataLogging);
        }

        // Note: CommandTimeout is configured differently in .NET 8
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Configuration entity
        modelBuilder.Entity<ConfigurationEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasMany(e => e.Jobs)
                  .WithOne(j => j.Configuration)
                  .HasForeignKey(j => j.ConfigurationId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Job entity
        modelBuilder.Entity<JobEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartedAt);
            entity.HasIndex(e => e.CompletedAt);
            entity.HasIndex(e => e.ConfigurationId);

            entity.HasMany(e => e.Results)
                  .WithOne(r => r.Job)
                  .HasForeignKey(r => r.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Duration)
                  .HasConversion(
                      v => v.HasValue ? v.Value.Ticks : (long?)null,
                      v => v.HasValue ? new TimeSpan(v.Value) : (TimeSpan?)null);
        });

        // Configure JobResult entity
        modelBuilder.Entity<JobResultEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.JobId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Success);
            entity.HasIndex(e => e.CreatedAt);

            entity.Property(e => e.ExecutionTime)
                  .HasConversion(
                      v => v.Ticks,
                      v => new TimeSpan(v));
        });

        // Configure Proxy entity
        modelBuilder.Entity<ProxyEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Host, e.Port }).IsUnique();
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Health);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsBanned);
            entity.HasIndex(e => e.Country);
        });

        // Configure Setting entity
        modelBuilder.Entity<SettingEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.Category);
        });

        // Configure base entity properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return new SqliteTransaction(transaction);
    }

    public async Task<int> ExecuteSqlAsync(string sql, object[]? parameters = null, CancellationToken cancellationToken = default)
    {
        return await Database.ExecuteSqlRawAsync(sql, parameters ?? Array.Empty<object>(), cancellationToken);
    }

    public async Task<bool> BackupAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating database backup to {BackupPath}", backupPath);

            // Ensure backup directory exists
            var backupDir = Path.GetDirectoryName(backupPath);
            if (!string.IsNullOrEmpty(backupDir) && !Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            // For SQLite, we can simply copy the database file
            var connectionString = Database.GetConnectionString();
            var dbPath = ExtractDbPathFromConnectionString(connectionString);

            if (File.Exists(dbPath))
            {
                await using var sourceStream = new FileStream(dbPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                await using var destStream = new FileStream(backupPath, FileMode.Create, FileAccess.Write);
                await sourceStream.CopyToAsync(destStream, cancellationToken);

                _logger.LogInformation("Database backup completed successfully");
                return true;
            }

            _logger.LogWarning("Database file not found at {DbPath}", dbPath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database backup");
            return false;
        }
    }

    public async Task<bool> RestoreAsync(string backupPath, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Restoring database from {BackupPath}", backupPath);

            if (!File.Exists(backupPath))
            {
                _logger.LogError("Backup file not found at {BackupPath}", backupPath);
                return false;
            }

            var connectionString = Database.GetConnectionString();
            var dbPath = ExtractDbPathFromConnectionString(connectionString);

            // Close all connections
            await Database.CloseConnectionAsync();

            // Replace the database file
            await using var sourceStream = new FileStream(backupPath, FileMode.Open, FileAccess.Read);
            await using var destStream = new FileStream(dbPath, FileMode.Create, FileAccess.Write);
            await sourceStream.CopyToAsync(destStream, cancellationToken);

            _logger.LogInformation("Database restore completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore database from backup");
            return false;
        }
    }

    public async Task OptimizeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Optimizing database");

            // Only run optimization for actual database providers (not InMemory)
            if (_options.Provider == DatabaseProvider.SQLite)
            {
                // SQLite optimization commands
                await Database.ExecuteSqlRawAsync("VACUUM;", cancellationToken);
                await Database.ExecuteSqlRawAsync("ANALYZE;", cancellationToken);
                await Database.ExecuteSqlRawAsync("PRAGMA optimize;", cancellationToken);
            }
            else if (_options.Provider == DatabaseProvider.InMemory)
            {
                // InMemory database doesn't support these operations, just log
                _logger.LogInformation("Skipping optimization for InMemory database");
            }

            _logger.LogInformation("Database optimization completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to optimize database");
            throw;
        }
    }

    private static string ExtractDbPathFromConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "openbullet.db";

        // Extract Data Source from connection string
        var parts = connectionString.Split(';');
        var dataSourcePart = parts.FirstOrDefault(p => p.Trim().StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));
        
        if (dataSourcePart != null)
        {
            return dataSourcePart.Split('=')[1].Trim();
        }

        return "openbullet.db";
    }

    public override void Dispose()
    {
        base.Dispose();
        _logger.LogDebug("SqliteContext disposed");
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        _logger.LogDebug("SqliteContext disposed asynchronously");
    }
}

/// <summary>
/// SQLite repository implementation
/// </summary>
public class SqliteRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly SqliteContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger _logger;

    public SqliteRepository(SqliteContext context, ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _logger = logger;
    }

    public async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        // Use FirstOrDefaultAsync instead of FindAsync to respect global query filters (soft delete)
        return await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id.ToString(), cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        // Sync JSON fields for JobResultEntity
        if (entity is JobResultEntity jobResult)
            jobResult.SyncJsonFields();

        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        await _dbSet.AddRangeAsync(entityList, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entityList;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.UpdatedAt = DateTime.UtcNow;
        }

        // Sync JSON fields for JobResultEntity
        if (entity is JobResultEntity jobResult)
            jobResult.SyncJsonFields();

        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is BaseEntity baseEntity)
        {
            // Soft delete
            baseEntity.IsDeleted = true;
            baseEntity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        else
        {
            // Hard delete
            _dbSet.Remove(entity);
        }

        var changes = await _context.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }

    public async Task<bool> DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        return await DeleteAsync(entity, cancellationToken);
    }

    public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        var count = 0;

        foreach (var entity in entities)
        {
            if (await DeleteAsync(entity, cancellationToken))
                count++;
        }

        return count;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);

        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<TEntity>> GetOrderedAsync<TKey>(Expression<Func<TEntity, TKey>> orderBy, bool descending = false, CancellationToken cancellationToken = default)
    {
        var query = descending 
            ? _dbSet.OrderByDescending(orderBy) 
            : _dbSet.OrderBy(orderBy);

        return await query.ToListAsync(cancellationToken);
    }
}

/// <summary>
/// SQLite transaction wrapper
/// </summary>
public class SqliteTransaction : IDbTransaction
{
    private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _transaction;
    private bool _disposed = false;

    public SqliteTransaction(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction.Dispose();
            _disposed = true;
        }
    }
}
