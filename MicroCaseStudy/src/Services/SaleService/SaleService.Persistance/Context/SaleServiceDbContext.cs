using System.Text.Json;
using Core.CrossCuttingConcerns.Serilog;
using Core.WebAPI.Appsettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SaleService.Domain.Entities;
using Serilog.Context;

namespace SaleService.Persistance.Context;

public class SaleServiceDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;
    private readonly IUserSession<int> _userSession;
    private readonly string _httpMethod;
    private readonly string _path;
    private readonly string _queryParams;
    private readonly WebApiConfiguration _webApiConfiguration;

    public SaleServiceDbContext(DbContextOptions<SaleServiceDbContext> options, IConfiguration configuration,
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

    public virtual DbSet<Parameter> Parameters { get; set; }

    public virtual DbSet<ParameterGroup> ParameterGroups { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("SaleService"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region SeedData
            #region ParameterGroup
                modelBuilder.Entity<ParameterGroup>()
                    .HasData(
                        new ParameterGroup()
                        {
                            Id = 1,
                            ParameterGroupValue = "Satış Durumu",
                            RecordGuid = Guid.NewGuid(),
                            IsActive = true,
                            IsDeleted = false,
                        }
                    );
            
            #endregion
            
            #region Parameter
                modelBuilder.Entity<Parameter>()
                    .HasData(
                        new Parameter()
                        {
                            Id = 1,
                            ParameterValue = "Yeni",
                            ParameterGroupId =  1, 
                            RecordGuid = Guid.NewGuid(),
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new Parameter()
                        {
                            Id = 2,
                            ParameterValue = "İletişimde",
                            ParameterGroupId =  1, 
                            RecordGuid = Guid.NewGuid(),
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new Parameter()
                        {
                            Id = 3,
                            ParameterValue = "Anlaşma",
                            ParameterGroupId =  1, 
                            RecordGuid = Guid.NewGuid(),
                            IsActive = true,
                            IsDeleted = false,
                        },
                        new Parameter()
                        {
                            Id = 4,
                            ParameterValue = "Kapandı",
                            ParameterGroupId =  1, 
                            RecordGuid = Guid.NewGuid(),
                            IsActive = true,
                            IsDeleted = false,
                        }
                    );
            
            #endregion
            
        #endregion
        
        #region GlobalFilter

        modelBuilder.Entity<Parameter>()
            .HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<ParameterGroup>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<Sale>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);
        modelBuilder.Entity<SaleDetail>().HasQueryFilter(e => e.IsActive == true && e.IsDeleted == false);

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

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parameters_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"ParameterSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ParameterGroupId).HasColumnName("parameter_group_id");
            entity.Property(e => e.ParameterValue)
                .HasMaxLength(30)
                .HasColumnName("parameter_value");
            entity.Property(e => e.ParameterValueDescription)
                .HasMaxLength(50)
                .HasColumnName("parameter_value_description");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.ParameterGroup).WithMany(p => p.Parameters)
                .HasForeignKey(d => d.ParameterGroupId)
                .HasConstraintName("parameters_parametergroups_parameter_group_fk");
        });

        modelBuilder.Entity<ParameterGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parametergroups_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"ParameterGroupSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.ParameterGroupValue)
                .HasMaxLength(25)
                .HasColumnName("parameter_group_value");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sale_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"SaleSeq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(50)
                .HasColumnName("customer_email");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(50)
                .HasColumnName("customer_phone");
            entity.Property(e => e.CustomerSurname)
                .HasMaxLength(50)
                .HasColumnName("customer_surname");
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
            entity.Property(e => e.SaleName)
                .HasMaxLength(50)
                .HasColumnName("sale_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("saledetail_pk");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"SaleDetailSeq\"'::regclass)")
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
            entity.Property(e => e.Note)
                .HasMaxLength(50)
                .HasColumnName("note");
            entity.Property(e => e.RecordGuid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("record_guid");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.SaleStatusParameterId).HasColumnName("sale_status_parameter_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("saledetails_sales_sale_id_fk");

            entity.HasOne(d => d.SaleStatusParameter).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.SaleStatusParameterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("saledetails_parameters_sale_parameter_id_fk");
        });

        #endregion

        modelBuilder.HasSequence("ParameterGroupSeq").StartsAt(4);
        modelBuilder.HasSequence("ParameterSeq").StartsAt(5);
        modelBuilder.HasSequence("SaleDetailSeq");
        modelBuilder.HasSequence("SaleSeq");

        base.OnModelCreating(modelBuilder);
    }
}