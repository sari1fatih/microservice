using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerService.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "CustomerNoteSeq",
                startValue: 3L);

            migrationBuilder.CreateSequence(
                name: "CustomerSeq",
                startValue: 3L);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"CustomerSeq\"'::regclass)"),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    surname = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    company = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("customer_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"CustomerNoteSeq\"'::regclass)"),
                    customerid = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("customernote_pk", x => x.id);
                    table.ForeignKey(
                        name: "FK_CustomerNotes_Customers_customerid",
                        column: x => x.customerid,
                        principalTable: "Customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "id", "company", "created_at", "created_by", "deleted_at", "deleted_by", "email", "is_active", "name", "phone", "record_guid", "surname", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, "Nasa", new DateTime(2025, 2, 11, 20, 54, 9, 459, DateTimeKind.Utc).AddTicks(6590), 1, null, null, "ayse@gmail.com", true, "Ayşe", "12345134", new Guid("09fdb930-76c9-423a-881b-e8d213cf60b7"), "Fatma", null, null },
                    { 2, "Tesla", new DateTime(2025, 2, 11, 20, 54, 9, 459, DateTimeKind.Utc).AddTicks(6600), 1, null, null, "hakkı@gmail.com", true, "Hakkı", "12123345134", new Guid("bf4027c1-b4bb-4a7d-9b2f-30203a57963c"), "Hakyemez", null, null }
                });

            migrationBuilder.InsertData(
                table: "CustomerNotes",
                columns: new[] { "id", "created_at", "created_by", "customerid", "deleted_at", "deleted_by", "is_active", "note", "record_guid", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 11, 20, 54, 9, 459, DateTimeKind.Utc).AddTicks(6680), 1, 1, null, null, true, "Önemli Müşteri", new Guid("5d10d6b3-cf4f-43e5-9ebc-d40fb5db6255"), null, null },
                    { 2, new DateTime(2025, 2, 11, 20, 54, 9, 459, DateTimeKind.Utc).AddTicks(6690), 1, 2, null, null, true, "Çok daha önemli Müşteri", new Guid("58af7073-8edc-47b1-96b2-8f2ea1f5b0c0"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNotes_customerid",
                table: "CustomerNotes",
                column: "customerid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerNotes");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropSequence(
                name: "CustomerNoteSeq");

            migrationBuilder.DropSequence(
                name: "CustomerSeq");
        }
    }
}
