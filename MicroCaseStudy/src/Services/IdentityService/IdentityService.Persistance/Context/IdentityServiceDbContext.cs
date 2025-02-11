using System.Text.Json;
using Core.CrossCuttingConcerns.Serilog;
using Core.Security.Hashing;
using Core.WebAPI.Appsettings;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace IdentityService.Persistance.Context;

public class IdentityServiceDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;
    private readonly IUserSession<int> _userSession;
    private readonly string _httpMethod;
    private readonly string _path;
    private readonly string _queryParams;
    private readonly WebApiConfiguration _webApiConfiguration;

    public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options, IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        LoggerServiceBase loggerServiceBase,
        IUserSession<int> userSession,
        IOptions<WebApiConfiguration> webApiConfiguration)
        : base(options)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _loggerServiceBase = loggerServiceBase;
        _userSession = userSession;

        _httpMethod = _httpContextAccessor?.HttpContext?.Request?.Method ?? string.Empty;
        _path = _httpContextAccessor?.HttpContext?.Request?.Path ?? string.Empty;
        _queryParams = _httpContextAccessor?.HttpContext?.Request != null &&
                       _httpContextAccessor.HttpContext.Request.QueryString.HasValue
            ? _httpContextAccessor.HttpContext.Request.QueryString.Value
            : "No Query Parameters";
        _webApiConfiguration = webApiConfiguration.Value;
    }

    private void Log()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified || x.State == EntityState.Deleted ||
                        x.State == EntityState.Added);
        Dictionary<EntityState, List<string>> dictionary = new Dictionary<EntityState, List<string>>();

        foreach (var entry in modifiedEntries)
        {
            var entityName = entry.Entity.GetType().Name;
            var state = entry.State;
            var oldValues = entry.OriginalValues;
            var currentValues = entry.CurrentValues;

            // Eski ve yeni değerleri loglamak, yalnızca değerler farklıysa

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.Name == "RecordGuid")
                    continue;

                var oldValue = oldValues[property.Metadata.Name]?.ToString();
                var newValue = currentValues[property.Metadata.Name]?.ToString();

                if (newValue == "00000000-0000-0000-0000-000000000000")
                    continue;
                // Sadece Insert işlemi ise, sadece yeni değerleri logla
                if (state == EntityState.Added)
                {
                    if (
                        property.Metadata.Name == "Id" ||
                        property.Metadata.Name == "Token" ||
                        property.Metadata.Name == "Jti" ||
                        property.Metadata.Name == "ReplacedByToken" ||
                        property.Metadata.Name == "ReasonRevoked"
                    )
                        continue;

                    if (!dictionary.ContainsKey(EntityState.Added))
                    {
                        dictionary[EntityState.Added] = new List<string>();
                    }

                    if (!string.IsNullOrEmpty(newValue?.Trim()))
                    {
                        dictionary[EntityState.Added]
                            .Add($"Entity: {entityName}, Property: {property.Metadata.Name}, New Value: {newValue}");
                    }
                }
                else if (oldValue != newValue)
                {
                    if (
                        property.Metadata.Name == "Id" ||
                        property.Metadata.Name == "Token" ||
                        property.Metadata.Name == "Jti" ||
                        property.Metadata.Name == "ReplacedByToken" ||
                        property.Metadata.Name == "ReasonRevoked"
                    )
                        continue;

                    if (!dictionary.ContainsKey(state))
                    {
                        dictionary[state] = new List<string>();
                    }

                    dictionary[state]
                        .Add(
                            $"Entity: {entityName}, Property: {property.Metadata.Name}, Old Value: {oldValue}, New Value: {newValue}");
                }
            }
        }

        if (dictionary.Count != 0)
        {
            using (LogContext.PushProperty("http_method", _httpMethod))
            {
                LogContext.PushProperty("path", _path);
                LogContext.PushProperty("query_params", _queryParams);
                LogContext.PushProperty("body", _userSession.Body);
                LogContext.PushProperty("user_id", _userSession.UserId.ToString());
                _userSession.DataLog = dictionary;
                _loggerServiceBase.Info(JsonSerializer.Serialize(dictionary));
            }
        }
    }

    public override int SaveChanges()
    {
        if (!_webApiConfiguration.ExcludedPaths.Any(x => x == _userSession.Path))
            Log();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if (!_webApiConfiguration.ExcludedPaths.Any(x => x == _userSession.Path))
            Log();

        return base.SaveChangesAsync(cancellationToken);
    }

    #region Info Table

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("IdentityService"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region SeedData

            #region User

            HashingHelper.CreatePasswordHash(
                "test",
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );
            modelBuilder.Entity<User>()
                .HasData(
                    new User()
                    {
                        Id = 1,
                        Username = "sari1fatih",
                        Email = "fatihsari1992@gmail.com",
                        Name = "Fatih",
                        Surname = "Sarı",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        RecordGuid = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "gorkan1tahir",
                        Email = "tahirgorkan@gmail.com",
                        Name = "Tahir",
                        Surname = "Görkan",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        RecordGuid = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    },
                    new User()
                    {
                        Id = 3,
                        Username = "adıguzel1utkan",
                        Email = "utkan@gmail.com",
                        Name = "Utkan",
                        Surname = "Adıgüzel",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        RecordGuid = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    },
                    new User()
                    {
                        Id = 4,
                        Username = "acar1kutay",
                        Email = "kutay@gmail.com",
                        Name = "Kutay",
                        Surname = "Acar",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        RecordGuid = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    },
                    new User()
                    {
                        Id = 5,
                        Username = "behlul1enes",
                        Email = "enes@gmail.com",
                        Name = "Enes",
                        Surname = "Behlül",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        RecordGuid = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                    }
                );

            #endregion
            
            #region Role
                modelBuilder.Entity<Role>()
                    .HasData(
                        new Role()
                        {
                            Id = 1,
                            RoleValue = "Admin",
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new Role()
                        {
                            Id = 2,
                            RoleValue = "User",
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        }
                    );
            #endregion
            
            #region UserRole
            
                modelBuilder.Entity<UserRole>()
                    .HasData(
                        new UserRole()
                        {
                            Id = 1,
                            RoleId = 1,
                            UserId = 1,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 2,
                            RoleId = 2,
                            UserId = 1,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 3,
                            RoleId = 1,
                            UserId = 2,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 4,
                            RoleId = 2,
                            UserId = 2,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 5,
                            RoleId = 1,
                            UserId = 3,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 6,
                            RoleId = 2,
                            UserId =3,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 7,
                            RoleId = 1,
                            UserId = 4,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new UserRole()
                        {
                            Id = 8,
                            RoleId = 2,
                            UserId = 4,
                            CreatedBy = 1,
                            RecordGuid = Guid.NewGuid(),
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                        }
                    );
            
            
            #endregion
        #endregion

        #region GlobalFilter

        modelBuilder.Entity<Role>()
            .HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<User>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<UserRole>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);

        #endregion

        #region Creating Table

        modelBuilder.Entity<Log>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("logs");

            entity.Property(e => e.Body)
                .HasColumnType("jsonb")
                .HasColumnName("body");
            entity.Property(e => e.Exception).HasColumnName("exception");
            entity.Property(e => e.HttpMethod)
                .HasMaxLength(10)
                .HasColumnName("http_method");
            entity.Property(e => e.Level)
                .HasMaxLength(50)
                .HasColumnName("level");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.MessageTemplate).HasColumnName("message_template");
            entity.Property(e => e.Path)
                .HasMaxLength(50)
                .HasColumnName("path");
            entity.Property(e => e.Properties)
                .HasColumnType("jsonb")
                .HasColumnName("properties");
            entity.Property(e => e.QueryParams)
                .HasMaxLength(100)
                .HasColumnName("query_params");
            entity.Property(e => e.TimeStamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_stamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"RefreshTokenSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedByIp)
                .HasMaxLength(50)
                .HasColumnName("created_by_ip");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.ExpiresDate).HasColumnName("expires_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Jti)
                .HasMaxLength(50)
                .HasColumnName("jti");
            entity.Property(e => e.ReasonRevoked)
                .HasMaxLength(90)
                .HasColumnName("reason_revoked");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.ReplacedByJti).HasColumnName("replaced_by_jti");
            entity.Property(e => e.RevokedByIp)
                .HasMaxLength(50)
                .HasColumnName("revoked_by_ip");
            entity.Property(e => e.RevokedDate).HasColumnName("revoked_date");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RefreshTokenCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refreshtokens_users_created_by_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.RefreshTokenDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("refreshtokens_users_deleted_by_fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RefreshTokenUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("refreshtokens_users_updated_by_fk");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokenUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refreshtokens_users_user_id_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"RoleSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.RoleValue)
                .HasMaxLength(50)
                .HasColumnName("role_value");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("roles_users_created_by_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.RoleDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("roles_users_deleted_by_fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.RoleUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("roles_users_updated_by_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"UserSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt).HasColumnName("password_salt");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.Surname)
                .HasMaxLength(25)
                .HasColumnName("surname");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.InverseDeletedByNavigation)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("users_users_deleted_by_fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("users_users_updated_by_fk");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_roles_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"UserRoleSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserRoleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("userroles_users_created_by_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.UserRoleDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("userroles_users_deleted_by_fk");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userroles_roles_role_id_fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UserRoleUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("userroles_users_updated_by_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoleUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userroles_users_user_id_fk");
        });

        #endregion

        modelBuilder.HasSequence("RefreshTokenSeq");
        modelBuilder.HasSequence("RoleSeq").StartsAt(3);
        modelBuilder.HasSequence("UserRoleSeq").StartsAt(9);
        modelBuilder.HasSequence("UserSeq").StartsAt(6);

        base.OnModelCreating(modelBuilder);
    }
}