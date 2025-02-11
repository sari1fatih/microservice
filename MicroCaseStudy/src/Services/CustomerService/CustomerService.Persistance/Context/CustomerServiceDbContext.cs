using System.Text.Json;
using Core.CrossCuttingConcerns.Serilog;
using Core.WebAPI.Appsettings;
using CustomerService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog.Context;

namespace CustomerService.Persistance.Context;

public class CustomerServiceDbContext:DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;
    private readonly IUserSession<int> _userSession;
    private readonly string _httpMethod;
    private readonly string _path;
    private readonly string _queryParams;
    private readonly WebApiConfiguration _webApiConfiguration;
    
    public CustomerServiceDbContext(DbContextOptions<CustomerServiceDbContext> options,  IConfiguration configuration,
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

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerNote> CustomerNotes { get; set; }


    #endregion
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("CustomerService"));
    
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        #region SeedData
            #region Customer
                 modelBuilder.Entity<Customer>()
                    .HasData(
                        new Customer()
                        {
                            Id = 1, 
                            Name = "Ayşe",
                            Surname = "Fatma", 
                            Email = "ayse@gmail.com", 
                            Phone = "12345134", 
                            Company = "Nasa", 
                            RecordGuid = Guid.NewGuid(),
                            CreatedBy = 1,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        } ,
                        new Customer()
                        {
                            Id = 2, 
                            Name = "Hakkı",
                            Surname = "Hakyemez", 
                            Email = "hakkı@gmail.com", 
                            Phone = "12123345134", 
                            Company = "Tesla", 
                            RecordGuid = Guid.NewGuid(),
                            CreatedBy = 1,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        } 
                    );
            #endregion 
            
            #region Customer
            modelBuilder.Entity<CustomerNote>()
                .HasData(
                    new CustomerNote()
                    {
                        Id = 1, 
                        Customerid = 1,
                        Note = "Önemli Müşteri",
                        RecordGuid = Guid.NewGuid(),
                        CreatedBy = 1,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    } ,
                    new CustomerNote()
                    {
                        Id = 2, 
                        Customerid = 2,
                        Note = "Çok daha önemli Müşteri",
                        RecordGuid = Guid.NewGuid(),
                        CreatedBy = 1,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    } 
                );
            #endregion 
        #endregion 
        
        #region GlobalFilter

        modelBuilder.Entity<Customer>()
            .HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<CustomerNote>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);

        #endregion
        
        #region Creating Table
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"CustomerSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Company)
                .HasMaxLength(50)
                .HasColumnName("company");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
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
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.Surname)
                .HasMaxLength(25)
                .HasColumnName("surname");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<CustomerNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customernote_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"CustomerNoteSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });
        #endregion

        modelBuilder.HasSequence("CustomerNoteSeq").StartsAt(3);
        modelBuilder.HasSequence("CustomerSeq").StartsAt(3);

        base.OnModelCreating(modelBuilder);
    }

    
}