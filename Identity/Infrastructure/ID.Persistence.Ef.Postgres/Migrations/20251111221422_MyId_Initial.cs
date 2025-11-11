using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ID.Persistence.Ef.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class MyId_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MyId");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "avatar",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    src_type = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    b64 = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_avatar", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feature_flags",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_flags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content_json = table.Column<string>(type: "text", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    renewal_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    device_limit = table.Column<int>(type: "integer", nullable: false),
                    trial_months = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plan_features",
                schema: "MyId",
                columns: table => new
                {
                    subscription_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_flag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_plan_features", x => new { x.subscription_plan_id, x.feature_flag_id });
                    table.ForeignKey(
                        name: "fk_subscription_plan_features_feature_flags_feature_flag_id",
                        column: x => x.feature_flag_id,
                        principalSchema: "MyId",
                        principalTable: "feature_flags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subscription_plan_features_subscription_plans_subscription_",
                        column: x => x.subscription_plan_id,
                        principalSchema: "MyId",
                        principalTable: "subscription_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "MyId",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "MyId",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address_line1 = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: true),
                    address_line2 = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: true),
                    address_line3 = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: true),
                    address_line4 = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: true),
                    address_line5 = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: true),
                    address_area_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    address_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    avatar_id = table.Column<Guid>(type: "uuid", nullable: true),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_position = table.Column<int>(type: "integer", nullable: false),
                    tkn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    tkn_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    two_factor_provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    two_factor_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_users_avatar_avatar_id",
                        column: x => x.avatar_id,
                        principalSchema: "MyId",
                        principalTable: "avatar",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "MyId",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "o_auth_info",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    issuer = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    email_verified = table.Column<bool>(type: "boolean", nullable: true),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    app_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_o_auth_info", x => x.id);
                    table.ForeignKey(
                        name: "fk_o_auth_info_users_app_user_id",
                        column: x => x.app_user_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payload = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    expires_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    team_type = table.Column<int>(type: "integer", nullable: false),
                    min_position = table.Column<int>(type: "integer", nullable: false),
                    max_position = table.Column<int>(type: "integer", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: true),
                    leader_id = table.Column<Guid>(type: "uuid", nullable: true),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_asp_net_users_leader_id",
                        column: x => x.leader_id,
                        principalSchema: "MyId",
                        principalTable: "AspNetUsers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "team_subscription",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    discount = table.Column<double>(type: "double precision", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    trial_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    trial_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_payment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_paymen_amount = table.Column<double>(type: "double precision", nullable: false),
                    device_limit = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    renewal_type = table.Column<int>(type: "integer", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_subscription", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_subscription_subscription_plans_subscription_plan_id",
                        column: x => x.subscription_plan_id,
                        principalSchema: "MyId",
                        principalTable: "subscription_plans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_team_subscription_teams_team_id",
                        column: x => x.team_id,
                        principalSchema: "MyId",
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_device",
                schema: "MyId",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    unique_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    administrator_username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    administrator_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_device", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_device_team_subscription_subscription_id",
                        column: x => x.subscription_id,
                        principalSchema: "MyId",
                        principalTable: "team_subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                schema: "MyId",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "MyId",
                table: "AspNetRoles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                schema: "MyId",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                schema: "MyId",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                schema: "MyId",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "MyId",
                table: "AspNetUsers",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_avatar_id",
                schema: "MyId",
                table: "AspNetUsers",
                column: "avatar_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_team_id",
                schema: "MyId",
                table: "AspNetUsers",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "MyId",
                table: "AspNetUsers",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_feature_flags_name",
                schema: "MyId",
                table: "feature_flags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_o_auth_info_app_user_id",
                schema: "MyId",
                table: "o_auth_info",
                column: "app_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                schema: "MyId",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_features_feature_flag_id",
                schema: "MyId",
                table: "subscription_plan_features",
                column: "feature_flag_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_device_subscription_id",
                schema: "MyId",
                table: "team_device",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_subscription_subscription_plan_id",
                schema: "MyId",
                table: "team_subscription",
                column: "subscription_plan_id");

            migrationBuilder.CreateIndex(
                name: "ix_team_subscription_team_id_subscription_plan_id",
                schema: "MyId",
                table: "team_subscription",
                columns: new[] { "team_id", "subscription_plan_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_teams_leader_id",
                schema: "MyId",
                table: "teams",
                column: "leader_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_claims_asp_net_users_user_id",
                schema: "MyId",
                table: "AspNetUserClaims",
                column: "user_id",
                principalSchema: "MyId",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_logins_asp_net_users_user_id",
                schema: "MyId",
                table: "AspNetUserLogins",
                column: "user_id",
                principalSchema: "MyId",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_user_roles_asp_net_users_user_id",
                schema: "MyId",
                table: "AspNetUserRoles",
                column: "user_id",
                principalSchema: "MyId",
                principalTable: "AspNetUsers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_teams_team_id",
                schema: "MyId",
                table: "AspNetUsers",
                column: "team_id",
                principalSchema: "MyId",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_asp_net_users_leader_id",
                schema: "MyId",
                table: "teams");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "o_auth_info",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "subscription_plan_features",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "team_device",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "feature_flags",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "team_subscription",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "subscription_plans",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "avatar",
                schema: "MyId");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "MyId");
        }
    }
}
