using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SaleService.Domain.Entities;

public partial class MicrosaleContext : DbContext
{
    public MicrosaleContext()
    {
    }

    public MicrosaleContext(DbContextOptions<MicrosaleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Parameter> Parameters { get; set; }

    public virtual DbSet<ParameterGroup> ParameterGroups { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID=postgres;Password=1234;Server=127.0.0.1;Port=5432;Database=microsale;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
        modelBuilder.HasSequence("ParameterGroupSeq");
        modelBuilder.HasSequence("ParameterSeq");
        modelBuilder.HasSequence("SaleDetailSeq");
        modelBuilder.HasSequence("SaleSeq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
