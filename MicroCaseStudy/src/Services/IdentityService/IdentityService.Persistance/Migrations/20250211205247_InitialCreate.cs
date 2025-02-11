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
                    { 1, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3760), null, null, "fatihsari1992@gmail.com", true, "Fatih", new byte[] { 160, 9, 197, 45, 23, 86, 86, 89, 15, 161, 224, 78, 212, 241, 197, 250, 220, 194, 250, 42, 68, 235, 220, 19, 20, 252, 134, 149, 75, 29, 89, 122, 97, 251, 142, 81, 163, 2, 179, 190, 9, 99, 176, 127, 52, 143, 147, 247, 94, 212, 140, 248, 225, 54, 118, 79, 56, 211, 124, 174, 136, 171, 168, 74 }, new byte[] { 203, 77, 216, 58, 135, 11, 120, 152, 31, 125, 113, 45, 238, 245, 33, 28, 19, 12, 108, 10, 37, 144, 30, 208, 218, 167, 12, 206, 150, 110, 164, 210, 93, 186, 174, 218, 120, 34, 14, 22, 5, 122, 234, 241, 47, 39, 30, 109, 230, 10, 225, 49, 64, 200, 217, 100, 56, 205, 77, 21, 99, 163, 165, 159, 248, 93, 61, 144, 189, 76, 90, 162, 165, 123, 12, 219, 134, 80, 174, 184, 98, 249, 98, 182, 113, 235, 103, 94, 231, 235, 177, 27, 95, 244, 159, 127, 22, 43, 104, 172, 127, 187, 159, 96, 94, 4, 246, 136, 219, 32, 41, 250, 207, 28, 117, 175, 174, 185, 103, 177, 39, 103, 63, 151, 195, 126, 191, 61 }, new Guid("061fe893-77ef-41cd-a41d-1e74efd63f01"), "Sarı", null, null, "sari1fatih" },
                    { 2, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3760), null, null, "tahirgorkan@gmail.com", true, "Tahir", new byte[] { 160, 9, 197, 45, 23, 86, 86, 89, 15, 161, 224, 78, 212, 241, 197, 250, 220, 194, 250, 42, 68, 235, 220, 19, 20, 252, 134, 149, 75, 29, 89, 122, 97, 251, 142, 81, 163, 2, 179, 190, 9, 99, 176, 127, 52, 143, 147, 247, 94, 212, 140, 248, 225, 54, 118, 79, 56, 211, 124, 174, 136, 171, 168, 74 }, new byte[] { 203, 77, 216, 58, 135, 11, 120, 152, 31, 125, 113, 45, 238, 245, 33, 28, 19, 12, 108, 10, 37, 144, 30, 208, 218, 167, 12, 206, 150, 110, 164, 210, 93, 186, 174, 218, 120, 34, 14, 22, 5, 122, 234, 241, 47, 39, 30, 109, 230, 10, 225, 49, 64, 200, 217, 100, 56, 205, 77, 21, 99, 163, 165, 159, 248, 93, 61, 144, 189, 76, 90, 162, 165, 123, 12, 219, 134, 80, 174, 184, 98, 249, 98, 182, 113, 235, 103, 94, 231, 235, 177, 27, 95, 244, 159, 127, 22, 43, 104, 172, 127, 187, 159, 96, 94, 4, 246, 136, 219, 32, 41, 250, 207, 28, 117, 175, 174, 185, 103, 177, 39, 103, 63, 151, 195, 126, 191, 61 }, new Guid("2fb4b76c-e2c7-4428-8a85-eb04471485c6"), "Görkan", null, null, "gorkan1tahir" },
                    { 3, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3770), null, null, "utkan@gmail.com", true, "Utkan", new byte[] { 160, 9, 197, 45, 23, 86, 86, 89, 15, 161, 224, 78, 212, 241, 197, 250, 220, 194, 250, 42, 68, 235, 220, 19, 20, 252, 134, 149, 75, 29, 89, 122, 97, 251, 142, 81, 163, 2, 179, 190, 9, 99, 176, 127, 52, 143, 147, 247, 94, 212, 140, 248, 225, 54, 118, 79, 56, 211, 124, 174, 136, 171, 168, 74 }, new byte[] { 203, 77, 216, 58, 135, 11, 120, 152, 31, 125, 113, 45, 238, 245, 33, 28, 19, 12, 108, 10, 37, 144, 30, 208, 218, 167, 12, 206, 150, 110, 164, 210, 93, 186, 174, 218, 120, 34, 14, 22, 5, 122, 234, 241, 47, 39, 30, 109, 230, 10, 225, 49, 64, 200, 217, 100, 56, 205, 77, 21, 99, 163, 165, 159, 248, 93, 61, 144, 189, 76, 90, 162, 165, 123, 12, 219, 134, 80, 174, 184, 98, 249, 98, 182, 113, 235, 103, 94, 231, 235, 177, 27, 95, 244, 159, 127, 22, 43, 104, 172, 127, 187, 159, 96, 94, 4, 246, 136, 219, 32, 41, 250, 207, 28, 117, 175, 174, 185, 103, 177, 39, 103, 63, 151, 195, 126, 191, 61 }, new Guid("2e9365e0-842b-4eda-a60e-bc5c4b1af1a7"), "Adıgüzel", null, null, "adıguzel1utkan" },
                    { 4, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3770), null, null, "kutay@gmail.com", true, "Kutay", new byte[] { 160, 9, 197, 45, 23, 86, 86, 89, 15, 161, 224, 78, 212, 241, 197, 250, 220, 194, 250, 42, 68, 235, 220, 19, 20, 252, 134, 149, 75, 29, 89, 122, 97, 251, 142, 81, 163, 2, 179, 190, 9, 99, 176, 127, 52, 143, 147, 247, 94, 212, 140, 248, 225, 54, 118, 79, 56, 211, 124, 174, 136, 171, 168, 74 }, new byte[] { 203, 77, 216, 58, 135, 11, 120, 152, 31, 125, 113, 45, 238, 245, 33, 28, 19, 12, 108, 10, 37, 144, 30, 208, 218, 167, 12, 206, 150, 110, 164, 210, 93, 186, 174, 218, 120, 34, 14, 22, 5, 122, 234, 241, 47, 39, 30, 109, 230, 10, 225, 49, 64, 200, 217, 100, 56, 205, 77, 21, 99, 163, 165, 159, 248, 93, 61, 144, 189, 76, 90, 162, 165, 123, 12, 219, 134, 80, 174, 184, 98, 249, 98, 182, 113, 235, 103, 94, 231, 235, 177, 27, 95, 244, 159, 127, 22, 43, 104, 172, 127, 187, 159, 96, 94, 4, 246, 136, 219, 32, 41, 250, 207, 28, 117, 175, 174, 185, 103, 177, 39, 103, 63, 151, 195, 126, 191, 61 }, new Guid("38037f26-5c42-44b3-a867-d1b2dd956f3b"), "Acar", null, null, "acar1kutay" },
                    { 5, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3780), null, null, "enes@gmail.com", true, "Enes", new byte[] { 160, 9, 197, 45, 23, 86, 86, 89, 15, 161, 224, 78, 212, 241, 197, 250, 220, 194, 250, 42, 68, 235, 220, 19, 20, 252, 134, 149, 75, 29, 89, 122, 97, 251, 142, 81, 163, 2, 179, 190, 9, 99, 176, 127, 52, 143, 147, 247, 94, 212, 140, 248, 225, 54, 118, 79, 56, 211, 124, 174, 136, 171, 168, 74 }, new byte[] { 203, 77, 216, 58, 135, 11, 120, 152, 31, 125, 113, 45, 238, 245, 33, 28, 19, 12, 108, 10, 37, 144, 30, 208, 218, 167, 12, 206, 150, 110, 164, 210, 93, 186, 174, 218, 120, 34, 14, 22, 5, 122, 234, 241, 47, 39, 30, 109, 230, 10, 225, 49, 64, 200, 217, 100, 56, 205, 77, 21, 99, 163, 165, 159, 248, 93, 61, 144, 189, 76, 90, 162, 165, 123, 12, 219, 134, 80, 174, 184, 98, 249, 98, 182, 113, 235, 103, 94, 231, 235, 177, 27, 95, 244, 159, 127, 22, 43, 104, 172, 127, 187, 159, 96, 94, 4, 246, 136, 219, 32, 41, 250, 207, 28, 117, 175, 174, 185, 103, 177, 39, 103, 63, 151, 195, 126, 191, 61 }, new Guid("3735c4e4-0a92-482b-a71f-f419df0111f4"), "Behlül", null, null, "behlul1enes" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "id", "created_at", "created_by", "deleted_at", "deleted_by", "is_active", "record_guid", "role_value", "updated_at", "updated_by" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3870), 1, null, null, true, new Guid("cbd3a9d4-7f6e-4ef0-b827-7bd4c3a6141d"), "Admin", null, null },
                    { 2, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3870), 1, null, null, true, new Guid("09f32505-d253-481a-abbe-017f5b12b671"), "User", null, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "id", "created_at", "created_by", "deleted_at", "deleted_by", "is_active", "record_guid", "role_id", "updated_at", "updated_by", "user_id" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3890), 1, null, null, true, new Guid("a8ae56b4-2164-4cdc-b963-39c8229c999d"), 1, null, null, 1 },
                    { 2, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3900), 1, null, null, true, new Guid("fedc061d-b85f-43c5-947f-a635c91c3533"), 2, null, null, 1 },
                    { 3, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3900), 1, null, null, true, new Guid("86fbcc4f-0c09-463a-9396-4260f8376754"), 1, null, null, 2 },
                    { 4, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3900), 1, null, null, true, new Guid("c1a59233-f12b-40c0-8757-a6612a68a14d"), 2, null, null, 2 },
                    { 5, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3910), 1, null, null, true, new Guid("d4bc6296-2f7e-4014-8a01-bca17b5d377c"), 1, null, null, 3 },
                    { 6, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3910), 1, null, null, true, new Guid("9700b8ec-16a8-4f6f-9b58-ab47184cd4b7"), 2, null, null, 3 },
                    { 7, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3920), 1, null, null, true, new Guid("6cd8567a-6359-4a9d-aebb-99a621c2bf52"), 1, null, null, 4 },
                    { 8, new DateTime(2025, 2, 11, 20, 52, 47, 691, DateTimeKind.Utc).AddTicks(3920), 1, null, null, true, new Guid("98651926-4656-4daf-99b3-89b2de62ecb0"), 2, null, null, 4 }
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
