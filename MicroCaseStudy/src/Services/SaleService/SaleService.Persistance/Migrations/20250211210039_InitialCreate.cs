using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SaleService.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ParameterGroupSeq",
                startValue: 4L);

            migrationBuilder.CreateSequence(
                name: "ParameterSeq",
                startValue: 5L);

            migrationBuilder.CreateSequence(
                name: "SaleDetailSeq");

            migrationBuilder.CreateSequence(
                name: "SaleSeq");

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    message = table.Column<string>(type: "text", nullable: true),
                    message_template = table.Column<string>(type: "text", nullable: true),
                    level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    time_stamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    exception = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "jsonb", nullable: true),
                    http_method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    path = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    query_params = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    body = table.Column<string>(type: "jsonb", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ParameterGroups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ParameterGroupSeq\"'::regclass)"),
                    parameter_group_value = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    record_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parametergroups_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SaleSeq\"'::regclass)"),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    customer_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    sale_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    record_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sale_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ParameterSeq\"'::regclass)"),
                    parameter_group_id = table.Column<int>(type: "integer", nullable: true),
                    parameter_value = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    parameter_value_description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    record_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("parameters_pk", x => x.id);
                    table.ForeignKey(
                        name: "parameters_parametergroups_parameter_group_fk",
                        column: x => x.parameter_group_id,
                        principalTable: "ParameterGroups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SaleDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"SaleDetailSeq\"'::regclass)"),
                    sale_id = table.Column<int>(type: "integer", nullable: false),
                    sale_status_parameter_id = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<int>(type: "integer", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    record_guid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("saledetail_pk", x => x.id);
                    table.ForeignKey(
                        name: "saledetails_parameters_sale_parameter_id_fk",
                        column: x => x.sale_status_parameter_id,
                        principalTable: "Parameters",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "saledetails_sales_sale_id_fk",
                        column: x => x.sale_id,
                        principalTable: "Sales",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "ParameterGroups",
                columns: new[] { "id", "deleted_at", "deleted_by", "is_active", "parameter_group_value", "record_guid", "updated_at", "updated_by" },
                values: new object[] { 1, null, null, true, "Satış Durumu", new Guid("beeaed67-60a7-41ba-878e-027736ad41c1"), null, null });

            migrationBuilder.InsertData(
                table: "Parameters",
                columns: new[] { "id", "deleted_at", "deleted_by", "is_active", "parameter_group_id", "parameter_value", "parameter_value_description", "record_guid", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, null, null, true, 1, "Yeni", null, new Guid("3843f7f6-a338-4f6b-84be-e8df3fe893c2"), null, null },
                    { 2, null, null, true, 1, "İletişimde", null, new Guid("25e9dfaa-dd46-411d-ad68-9d2faaaa6dcd"), null, null },
                    { 3, null, null, true, 1, "Anlaşma", null, new Guid("d4db096f-1ee5-4a17-9c44-9e7e515857a2"), null, null },
                    { 4, null, null, true, 1, "Kapandı", null, new Guid("b4684e3e-7a0b-4aa6-a5ce-2c7225a1f840"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parameters_parameter_group_id",
                table: "Parameters",
                column: "parameter_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_sale_id",
                table: "SaleDetails",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_sale_status_parameter_id",
                table: "SaleDetails",
                column: "sale_status_parameter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "SaleDetails");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "ParameterGroups");

            migrationBuilder.DropSequence(
                name: "ParameterGroupSeq");

            migrationBuilder.DropSequence(
                name: "ParameterSeq");

            migrationBuilder.DropSequence(
                name: "SaleDetailSeq");

            migrationBuilder.DropSequence(
                name: "SaleSeq");
        }
    }
}
