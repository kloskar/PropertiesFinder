using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class SetDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbPropertyAddresses",
                columns: table => new
                {
                    PropertyAddressID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<int>(nullable: false),
                    District = table.Column<string>(nullable: true),
                    StreetName = table.Column<string>(nullable: true),
                    DetailedAddress = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPropertyAddresses", x => x.PropertyAddressID);
                });

            migrationBuilder.CreateTable(
                name: "DbPropertyDetails",
                columns: table => new
                {
                    PropertyDetailsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<decimal>(nullable: false),
                    NumberOfRooms = table.Column<int>(nullable: false),
                    FloorNumber = table.Column<int>(nullable: true),
                    YearOfConstruction = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPropertyDetails", x => x.PropertyDetailsID);
                });

            migrationBuilder.CreateTable(
                name: "DbPropertyFeatures",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GardenArea = table.Column<decimal>(nullable: true),
                    Balconies = table.Column<int>(nullable: true),
                    BasementArea = table.Column<decimal>(nullable: true),
                    OutdoorParkingPlaces = table.Column<int>(nullable: true),
                    IndoorParkingPlaces = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPropertyFeatures", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DbPropertyPrices",
                columns: table => new
                {
                    PropertyPriceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalGrossPrice = table.Column<decimal>(nullable: false),
                    PricePerMeter = table.Column<decimal>(nullable: false),
                    ResidentalRent = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbPropertyPrices", x => x.PropertyPriceID);
                });

            migrationBuilder.CreateTable(
                name: "DbSellerContacts",
                columns: table => new
                {
                    SellerContactID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbSellerContacts", x => x.SellerContactID);
                });

            migrationBuilder.CreateTable(
                name: "DbOfferDetails",
                columns: table => new
                {
                    OfferDetailsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(nullable: true),
                    CreationDateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateDateTime = table.Column<DateTime>(nullable: true),
                    OfferKind = table.Column<int>(nullable: false),
                    SellerContactID = table.Column<int>(nullable: true),
                    IsStillValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbOfferDetails", x => x.OfferDetailsID);
                    table.ForeignKey(
                        name: "FK_DbOfferDetails_DbSellerContacts_SellerContactID",
                        column: x => x.SellerContactID,
                        principalTable: "DbSellerContacts",
                        principalColumn: "SellerContactID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DbEntries",
                columns: table => new
                {
                    EntryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferDetailsID = table.Column<int>(nullable: true),
                    PropertyPriceID = table.Column<int>(nullable: true),
                    PropertyDetailsID = table.Column<int>(nullable: true),
                    PropertyAddressID = table.Column<int>(nullable: true),
                    PropertyFeaturesID = table.Column<int>(nullable: true),
                    RawDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbEntries", x => x.EntryID);
                    table.ForeignKey(
                        name: "FK_DbEntries_DbOfferDetails_OfferDetailsID",
                        column: x => x.OfferDetailsID,
                        principalTable: "DbOfferDetails",
                        principalColumn: "OfferDetailsID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbEntries_DbPropertyAddresses_PropertyAddressID",
                        column: x => x.PropertyAddressID,
                        principalTable: "DbPropertyAddresses",
                        principalColumn: "PropertyAddressID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbEntries_DbPropertyDetails_PropertyDetailsID",
                        column: x => x.PropertyDetailsID,
                        principalTable: "DbPropertyDetails",
                        principalColumn: "PropertyDetailsID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbEntries_DbPropertyFeatures_PropertyFeaturesID",
                        column: x => x.PropertyFeaturesID,
                        principalTable: "DbPropertyFeatures",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DbEntries_DbPropertyPrices_PropertyPriceID",
                        column: x => x.PropertyPriceID,
                        principalTable: "DbPropertyPrices",
                        principalColumn: "PropertyPriceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbEntries_OfferDetailsID",
                table: "DbEntries",
                column: "OfferDetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_DbEntries_PropertyAddressID",
                table: "DbEntries",
                column: "PropertyAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_DbEntries_PropertyDetailsID",
                table: "DbEntries",
                column: "PropertyDetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_DbEntries_PropertyFeaturesID",
                table: "DbEntries",
                column: "PropertyFeaturesID");

            migrationBuilder.CreateIndex(
                name: "IX_DbEntries_PropertyPriceID",
                table: "DbEntries",
                column: "PropertyPriceID");

            migrationBuilder.CreateIndex(
                name: "IX_DbOfferDetails_SellerContactID",
                table: "DbOfferDetails",
                column: "SellerContactID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbEntries");

            migrationBuilder.DropTable(
                name: "DbOfferDetails");

            migrationBuilder.DropTable(
                name: "DbPropertyAddresses");

            migrationBuilder.DropTable(
                name: "DbPropertyDetails");

            migrationBuilder.DropTable(
                name: "DbPropertyFeatures");

            migrationBuilder.DropTable(
                name: "DbPropertyPrices");

            migrationBuilder.DropTable(
                name: "DbSellerContacts");
        }
    }
}
