using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "__efmigrationshistory",
                columns: table => new
                {
                    MigrationId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductVersion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___efmigrationshistory", x => x.MigrationId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetroles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetroles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetusers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetusers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "car",
                columns: table => new
                {
                    Plate = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Info = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<short>(type: "smallint", nullable: true, defaultValueSql: "'0'"),
                    Key = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_car", x => x.Plate);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "car_brand",
                columns: table => new
                {
                    Brand = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_car_brand", x => x.Brand);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cashback",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cashback", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dailyexpense",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expense = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dailyexpense", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Stock = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false),
                    Import = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Export = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: true),
                    Barcode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Normalize_Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Info = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<short>(type: "smallint", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory", x => x.Name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_caroption",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Option = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Car = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_caroption", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_option",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Option = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_option", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inventory_productgroup",
                columns: table => new
                {
                    Key = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GroupName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_productgroup", x => x.Key);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetroleclaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetroleclaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aspnetroleclaims_aspnetroles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "aspnetroles",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetuserclaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetuserclaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aspnetuserclaims_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetuserlogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetuserlogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_aspnetuserlogins_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetuserroles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetuserroles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_aspnetuserroles_aspnetroles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "aspnetroles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_aspnetuserroles_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "aspnetusertokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetusertokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_aspnetusertokens_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cart",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Plate = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Creator = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Company = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_cart_car_Plate",
                        column: x => x.Plate,
                        principalTable: "car",
                        principalColumn: "Plate");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "history",
                columns: table => new
                {
                    CashoutDate = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Plate = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Bank = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Dollar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Baht = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Riel = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Company = table.Column<sbyte>(type: "tinyint", nullable: true),
                    User = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<short>(type: "smallint", nullable: true, defaultValueSql: "'0'"),
                    Info = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    historycol = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_history", x => x.CashoutDate);
                    table.ForeignKey(
                        name: "FK_history_car_Plate",
                        column: x => x.Plate,
                        principalTable: "car",
                        principalColumn: "Plate");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "connector",
                columns: table => new
                {
                    GeneratedKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    Product = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Qty = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PriceOverwrite = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connector", x => x.GeneratedKey);
                    table.ForeignKey(
                        name: "FK_connector_cart_CartId",
                        column: x => x.CartId,
                        principalTable: "cart",
                        principalColumn: "CartID");
                    table.ForeignKey(
                        name: "FK_connector_inventory_Product",
                        column: x => x.Product,
                        principalTable: "inventory",
                        principalColumn: "Name");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "historyconnector",
                columns: table => new
                {
                    ID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Product = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Qty = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Export = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CartID = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historyconnector", x => x.ID);
                    table.ForeignKey(
                        name: "FK_historyconnector_history_CartID",
                        column: x => x.CartID,
                        principalTable: "history",
                        principalColumn: "CashoutDate");
                    table.ForeignKey(
                        name: "FK_historyconnector_inventory_Product",
                        column: x => x.Product,
                        principalTable: "inventory",
                        principalColumn: "Name");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetroleclaims_RoleId",
                table: "aspnetroleclaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserclaims_UserId",
                table: "aspnetuserclaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserlogins_UserId",
                table: "aspnetuserlogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserroles_RoleId",
                table: "aspnetuserroles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_Plate",
                table: "cart",
                column: "Plate");

            migrationBuilder.CreateIndex(
                name: "IX_connector_CartId",
                table: "connector",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_connector_Product",
                table: "connector",
                column: "Product");

            migrationBuilder.CreateIndex(
                name: "IX_history_Plate",
                table: "history",
                column: "Plate");

            migrationBuilder.CreateIndex(
                name: "IX_historyconnector_CartID",
                table: "historyconnector",
                column: "CartID");

            migrationBuilder.CreateIndex(
                name: "IX_historyconnector_Product",
                table: "historyconnector",
                column: "Product");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__efmigrationshistory");

            migrationBuilder.DropTable(
                name: "aspnetroleclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserlogins");

            migrationBuilder.DropTable(
                name: "aspnetuserroles");

            migrationBuilder.DropTable(
                name: "aspnetusertokens");

            migrationBuilder.DropTable(
                name: "car_brand");

            migrationBuilder.DropTable(
                name: "cashback");

            migrationBuilder.DropTable(
                name: "connector");

            migrationBuilder.DropTable(
                name: "dailyexpense");

            migrationBuilder.DropTable(
                name: "historyconnector");

            migrationBuilder.DropTable(
                name: "inventory_caroption");

            migrationBuilder.DropTable(
                name: "inventory_option");

            migrationBuilder.DropTable(
                name: "inventory_productgroup");

            migrationBuilder.DropTable(
                name: "aspnetroles");

            migrationBuilder.DropTable(
                name: "aspnetusers");

            migrationBuilder.DropTable(
                name: "cart");

            migrationBuilder.DropTable(
                name: "history");

            migrationBuilder.DropTable(
                name: "inventory");

            migrationBuilder.DropTable(
                name: "car");
        }
    }
}
