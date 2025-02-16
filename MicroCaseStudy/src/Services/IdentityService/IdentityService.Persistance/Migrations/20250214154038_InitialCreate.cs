using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IdentityService.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "RefreshTokenSeq");

            migrationBuilder.CreateSequence(
                name: "RoleSeq",
                startValue: 3L);

            migrationBuilder.CreateSequence(
                name: "UserRoleSeq",
                startValue: 9L);

            migrationBuilder.CreateSequence(
                name: "UserSeq",
                startValue: 6L);

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
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"UserSeq\"'::regclass)"),
                    username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    surname = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: true),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: true),
                    password_salt = table.Column<byte[]>(type: "bytea", nullable: true),
                    email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("user_pk", x => x.id);
                    table.ForeignKey(
                        name: "users_users_deleted_by_fk",
                        column: x => x.deleted_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "users_users_updated_by_fk",
                        column: x => x.updated_by,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"RefreshTokenSeq\"'::regclass)"),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    expires_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    revoked_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_by_ip = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    replaced_by_jti = table.Column<string>(type: "text", nullable: true),
                    reason_revoked = table.Column<string>(type: "character varying(90)", maxLength: 90, nullable: true),
                    jti = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("refresh_tokens_pk", x => x.id);
                    table.ForeignKey(
                        name: "refreshtokens_users_created_by_fk",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "refreshtokens_users_deleted_by_fk",
                        column: x => x.deleted_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "refreshtokens_users_updated_by_fk",
                        column: x => x.updated_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "refreshtokens_users_user_id_fk",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"RoleSeq\"'::regclass)"),
                    role_value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("roles_pk", x => x.id);
                    table.ForeignKey(
                        name: "roles_users_created_by_fk",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "roles_users_deleted_by_fk",
                        column: x => x.deleted_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "roles_users_updated_by_fk",
                        column: x => x.updated_by,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"UserRoleSeq\"'::regclass)"),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("user_roles_pk", x => x.id);
                    table.ForeignKey(
                        name: "userroles_roles_role_id_fk",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "userroles_users_created_by_fk",
                        column: x => x.created_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "userroles_users_deleted_by_fk",
                        column: x => x.deleted_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "userroles_users_updated_by_fk",
                        column: x => x.updated_by,
                        principalTable: "Users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "userroles_users_user_id_fk",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id", "created_at", "deleted_at", "deleted_by", "email", "is_active", "name", "password_hash", "password_salt", "record_guid", "surname", "updated_at", "updated_by", "username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4680), null, null, "fatihsari1992@gmail.com", true, "Fatih", new byte[] { 61, 113, 107, 215, 227, 184, 85, 59, 184, 93, 163, 54, 78, 100, 185, 117, 81, 159, 98, 22, 18, 157, 84, 138, 13, 212, 237, 146, 163, 126, 169, 134, 17, 245, 163, 194, 70, 219, 3, 6, 252, 147, 192, 253, 188, 102, 161, 123, 78, 191, 3, 178, 180, 251, 116, 213, 207, 120, 134, 92, 240, 136, 127, 53 }, new byte[] { 115, 137, 47, 244, 22, 169, 144, 102, 210, 133, 224, 78, 130, 204, 197, 220, 132, 131, 126, 1, 72, 71, 1, 176, 115, 190, 141, 200, 32, 245, 97, 45, 160, 1, 10, 31, 41, 152, 164, 193, 190, 20, 2, 248, 97, 69, 188, 105, 173, 4, 124, 171, 245, 255, 116, 78, 106, 165, 245, 252, 68, 8, 137, 164, 213, 86, 210, 8, 107, 88, 63, 41, 10, 36, 5, 97, 79, 68, 87, 86, 203, 0, 129, 132, 162, 44, 141, 106, 136, 179, 164, 20, 78, 204, 59, 1, 219, 184, 93, 53, 241, 57, 5, 63, 118, 188, 238, 199, 17, 160, 250, 176, 35, 108, 145, 172, 122, 245, 92, 97, 82, 29, 156, 164, 143, 65, 151, 141 }, new Guid("6483bf25-3dba-4c47-b5d2-baf98e1f43e5"), "Sarı", null, null, "sari1fatih" },
                    { 2, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4690), null, null, "tahirgorkan@gmail.com", true, "Tahir", new byte[] { 61, 113, 107, 215, 227, 184, 85, 59, 184, 93, 163, 54, 78, 100, 185, 117, 81, 159, 98, 22, 18, 157, 84, 138, 13, 212, 237, 146, 163, 126, 169, 134, 17, 245, 163, 194, 70, 219, 3, 6, 252, 147, 192, 253, 188, 102, 161, 123, 78, 191, 3, 178, 180, 251, 116, 213, 207, 120, 134, 92, 240, 136, 127, 53 }, new byte[] { 115, 137, 47, 244, 22, 169, 144, 102, 210, 133, 224, 78, 130, 204, 197, 220, 132, 131, 126, 1, 72, 71, 1, 176, 115, 190, 141, 200, 32, 245, 97, 45, 160, 1, 10, 31, 41, 152, 164, 193, 190, 20, 2, 248, 97, 69, 188, 105, 173, 4, 124, 171, 245, 255, 116, 78, 106, 165, 245, 252, 68, 8, 137, 164, 213, 86, 210, 8, 107, 88, 63, 41, 10, 36, 5, 97, 79, 68, 87, 86, 203, 0, 129, 132, 162, 44, 141, 106, 136, 179, 164, 20, 78, 204, 59, 1, 219, 184, 93, 53, 241, 57, 5, 63, 118, 188, 238, 199, 17, 160, 250, 176, 35, 108, 145, 172, 122, 245, 92, 97, 82, 29, 156, 164, 143, 65, 151, 141 }, new Guid("c929ffad-6122-4f2c-bdf0-f9f09aefd1a5"), "Görkan", null, null, "gorkan1tahir" },
                    { 3, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4700), null, null, "utkan@gmail.com", true, "Utkan", new byte[] { 61, 113, 107, 215, 227, 184, 85, 59, 184, 93, 163, 54, 78, 100, 185, 117, 81, 159, 98, 22, 18, 157, 84, 138, 13, 212, 237, 146, 163, 126, 169, 134, 17, 245, 163, 194, 70, 219, 3, 6, 252, 147, 192, 253, 188, 102, 161, 123, 78, 191, 3, 178, 180, 251, 116, 213, 207, 120, 134, 92, 240, 136, 127, 53 }, new byte[] { 115, 137, 47, 244, 22, 169, 144, 102, 210, 133, 224, 78, 130, 204, 197, 220, 132, 131, 126, 1, 72, 71, 1, 176, 115, 190, 141, 200, 32, 245, 97, 45, 160, 1, 10, 31, 41, 152, 164, 193, 190, 20, 2, 248, 97, 69, 188, 105, 173, 4, 124, 171, 245, 255, 116, 78, 106, 165, 245, 252, 68, 8, 137, 164, 213, 86, 210, 8, 107, 88, 63, 41, 10, 36, 5, 97, 79, 68, 87, 86, 203, 0, 129, 132, 162, 44, 141, 106, 136, 179, 164, 20, 78, 204, 59, 1, 219, 184, 93, 53, 241, 57, 5, 63, 118, 188, 238, 199, 17, 160, 250, 176, 35, 108, 145, 172, 122, 245, 92, 97, 82, 29, 156, 164, 143, 65, 151, 141 }, new Guid("02be94e8-0b99-4ddc-a5ac-7fa25f3919fb"), "Adıgüzel", null, null, "adıguzel1utkan" },
                    { 4, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4700), null, null, "kutay@gmail.com", true, "Kutay", new byte[] { 61, 113, 107, 215, 227, 184, 85, 59, 184, 93, 163, 54, 78, 100, 185, 117, 81, 159, 98, 22, 18, 157, 84, 138, 13, 212, 237, 146, 163, 126, 169, 134, 17, 245, 163, 194, 70, 219, 3, 6, 252, 147, 192, 253, 188, 102, 161, 123, 78, 191, 3, 178, 180, 251, 116, 213, 207, 120, 134, 92, 240, 136, 127, 53 }, new byte[] { 115, 137, 47, 244, 22, 169, 144, 102, 210, 133, 224, 78, 130, 204, 197, 220, 132, 131, 126, 1, 72, 71, 1, 176, 115, 190, 141, 200, 32, 245, 97, 45, 160, 1, 10, 31, 41, 152, 164, 193, 190, 20, 2, 248, 97, 69, 188, 105, 173, 4, 124, 171, 245, 255, 116, 78, 106, 165, 245, 252, 68, 8, 137, 164, 213, 86, 210, 8, 107, 88, 63, 41, 10, 36, 5, 97, 79, 68, 87, 86, 203, 0, 129, 132, 162, 44, 141, 106, 136, 179, 164, 20, 78, 204, 59, 1, 219, 184, 93, 53, 241, 57, 5, 63, 118, 188, 238, 199, 17, 160, 250, 176, 35, 108, 145, 172, 122, 245, 92, 97, 82, 29, 156, 164, 143, 65, 151, 141 }, new Guid("39908266-05e0-4850-83ba-5b0dc5c8defe"), "Acar", null, null, "acar1kutay" },
                    { 5, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4710), null, null, "enes@gmail.com", true, "Enes", new byte[] { 61, 113, 107, 215, 227, 184, 85, 59, 184, 93, 163, 54, 78, 100, 185, 117, 81, 159, 98, 22, 18, 157, 84, 138, 13, 212, 237, 146, 163, 126, 169, 134, 17, 245, 163, 194, 70, 219, 3, 6, 252, 147, 192, 253, 188, 102, 161, 123, 78, 191, 3, 178, 180, 251, 116, 213, 207, 120, 134, 92, 240, 136, 127, 53 }, new byte[] { 115, 137, 47, 244, 22, 169, 144, 102, 210, 133, 224, 78, 130, 204, 197, 220, 132, 131, 126, 1, 72, 71, 1, 176, 115, 190, 141, 200, 32, 245, 97, 45, 160, 1, 10, 31, 41, 152, 164, 193, 190, 20, 2, 248, 97, 69, 188, 105, 173, 4, 124, 171, 245, 255, 116, 78, 106, 165, 245, 252, 68, 8, 137, 164, 213, 86, 210, 8, 107, 88, 63, 41, 10, 36, 5, 97, 79, 68, 87, 86, 203, 0, 129, 132, 162, 44, 141, 106, 136, 179, 164, 20, 78, 204, 59, 1, 219, 184, 93, 53, 241, 57, 5, 63, 118, 188, 238, 199, 17, 160, 250, 176, 35, 108, 145, 172, 122, 245, 92, 97, 82, 29, 156, 164, 143, 65, 151, 141 }, new Guid("e7f8a186-f416-459f-b91a-f2ce41dd105e"), "Behlül", null, null, "behlul1enes" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "id", "created_at", "created_by", "deleted_at", "deleted_by", "is_active", "record_guid", "role_value", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4780), 1, null, null, true, new Guid("67a60545-6fdc-4e46-ab93-307b7a810ab3"), "Admin", null, null },
                    { 2, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4790), 1, null, null, true, new Guid("e4e17ac3-95d1-448a-8ab4-c9b7f062d4ef"), "User", null, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "id", "created_at", "created_by", "deleted_at", "deleted_by", "is_active", "record_guid", "role_id", "updated_at", "updated_by", "user_id" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4810), 1, null, null, true, new Guid("f7867ea0-022a-4d36-b414-48038093c945"), 1, null, null, 1 },
                    { 2, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4820), 1, null, null, true, new Guid("a295ed80-d16f-4d8e-b7ee-5b4eb5809f31"), 2, null, null, 1 },
                    { 3, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4820), 1, null, null, true, new Guid("b4053beb-89f9-4ac9-bf22-1c688f8b0f14"), 1, null, null, 2 },
                    { 4, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4830), 1, null, null, true, new Guid("1fd77b14-7ba5-4733-9c7e-b9596cdbe0e1"), 2, null, null, 2 },
                    { 5, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4830), 1, null, null, true, new Guid("27283539-afc9-464a-bfb5-85fc4e9cbd71"), 1, null, null, 3 },
                    { 6, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4840), 1, null, null, true, new Guid("f5cdc333-b458-45cc-9236-142f6771447f"), 2, null, null, 3 },
                    { 7, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4840), 1, null, null, true, new Guid("9f12fd16-80dd-4e62-a15f-cc6f13bddf9e"), 1, null, null, 4 },
                    { 8, new DateTime(2025, 2, 14, 15, 40, 38, 579, DateTimeKind.Utc).AddTicks(4840), 1, null, null, true, new Guid("54954d51-172f-473b-ad3e-8c06e2692e7e"), 2, null, null, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_created_by",
                table: "RefreshTokens",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_deleted_by",
                table: "RefreshTokens",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_updated_by",
                table: "RefreshTokens",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_user_id",
                table: "RefreshTokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_created_by",
                table: "Roles",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_deleted_by",
                table: "Roles",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_updated_by",
                table: "Roles",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_created_by",
                table: "UserRoles",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_deleted_by",
                table: "UserRoles",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_role_id",
                table: "UserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_updated_by",
                table: "UserRoles",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_user_id",
                table: "UserRoles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_deleted_by",
                table: "Users",
                column: "deleted_by");

            migrationBuilder.CreateIndex(
                name: "IX_Users_updated_by",
                table: "Users",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropSequence(
                name: "RefreshTokenSeq");

            migrationBuilder.DropSequence(
                name: "RoleSeq");

            migrationBuilder.DropSequence(
                name: "UserRoleSeq");

            migrationBuilder.DropSequence(
                name: "UserSeq");
        }
    }
}
