using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travely.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblWishList_tblHotels_hotel_id",
                table: "tblWishList");

            migrationBuilder.DropTable(
                name: "tblHotelAmenities");

            migrationBuilder.DropIndex(
                name: "UQ__tblUsers__AB6E616422F1317D",
                table: "tblUsers");

            migrationBuilder.DropIndex(
                name: "UQ__tblHotel__72E12F1BED7C40EC",
                table: "tblHotels");

            migrationBuilder.DropIndex(
                name: "UQ__tblBooki__BADA455927423170",
                table: "tblBookings");

            migrationBuilder.DropIndex(
                name: "UQ__lkpAmeni__72E12F1B6FB36A1B",
                table: "lkpAmenities");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tblUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "active",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldDefaultValue: "active");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "tblUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "customer",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "customer");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "tblUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "fullname",
                table: "tblUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "tblUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "hotel_id",
                table: "tblUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tblUserHotelBooking",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "active",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "active");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "tblRoomImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "tblPayments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "pending");

            migrationBuilder.AlterColumn<string>(
                name: "payment_method",
                table: "tblPayments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "tblHotels",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "tblHotelImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tblBookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "pending");

            migrationBuilder.AlterColumn<string>(
                name: "booking_reference",
                table: "tblBookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "lkpAmenities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "LkpAmenityTblHotel",
                columns: table => new
                {
                    AmenitiesAmenityId = table.Column<int>(type: "int", nullable: false),
                    HotelsHotelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LkpAmenityTblHotel", x => new { x.AmenitiesAmenityId, x.HotelsHotelId });
                    table.ForeignKey(
                        name: "FK_LkpAmenityTblHotel_lkpAmenities_AmenitiesAmenityId",
                        column: x => x.AmenitiesAmenityId,
                        principalTable: "lkpAmenities",
                        principalColumn: "amenity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LkpAmenityTblHotel_tblHotels_HotelsHotelId",
                        column: x => x.HotelsHotelId,
                        principalTable: "tblHotels",
                        principalColumn: "hotel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblUsers_hotel_id",
                table: "tblUsers",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tblUsers__AB6E616422F1317D",
                table: "tblUsers",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__tblHotel__72E12F1BED7C40EC",
                table: "tblHotels",
                column: "name",
                unique: true,
                filter: "[name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__tblBooki__BADA455927423170",
                table: "tblBookings",
                column: "booking_reference",
                unique: true,
                filter: "[booking_reference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__lkpAmeni__72E12F1B6FB36A1B",
                table: "lkpAmenities",
                column: "name",
                unique: true,
                filter: "[name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LkpAmenityTblHotel_HotelsHotelId",
                table: "LkpAmenityTblHotel",
                column: "HotelsHotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblUsers_tblHotels_hotel_id",
                table: "tblUsers",
                column: "hotel_id",
                principalTable: "tblHotels",
                principalColumn: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishList_Hotel",
                table: "tblWishList",
                column: "hotel_id",
                principalTable: "tblHotels",
                principalColumn: "hotel_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblUsers_tblHotels_hotel_id",
                table: "tblUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WishList_Hotel",
                table: "tblWishList");

            migrationBuilder.DropTable(
                name: "LkpAmenityTblHotel");

            migrationBuilder.DropIndex(
                name: "IX_tblUsers_hotel_id",
                table: "tblUsers");

            migrationBuilder.DropIndex(
                name: "UQ__tblUsers__AB6E616422F1317D",
                table: "tblUsers");

            migrationBuilder.DropIndex(
                name: "UQ__tblHotel__72E12F1BED7C40EC",
                table: "tblHotels");

            migrationBuilder.DropIndex(
                name: "UQ__tblBooki__BADA455927423170",
                table: "tblBookings");

            migrationBuilder.DropIndex(
                name: "UQ__lkpAmeni__72E12F1B6FB36A1B",
                table: "lkpAmenities");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "tblUsers");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tblUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "active",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true,
                oldDefaultValue: "active");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "tblUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "customer",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "customer");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "tblUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "fullname",
                table: "tblUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "tblUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tblUserHotelBooking",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "active",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "active");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "tblRoomImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "tblPayments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "pending");

            migrationBuilder.AlterColumn<string>(
                name: "payment_method",
                table: "tblPayments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "tblHotels",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "tblHotelImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "tblBookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "pending");

            migrationBuilder.AlterColumn<string>(
                name: "booking_reference",
                table: "tblBookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "lkpAmenities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "tblHotelAmenities",
                columns: table => new
                {
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    amenity_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblHotel__8B6EFA74CCBE0F70", x => new { x.hotel_id, x.amenity_id });
                    table.ForeignKey(
                        name: "FK_HotelAmenity_Amenity",
                        column: x => x.amenity_id,
                        principalTable: "lkpAmenities",
                        principalColumn: "amenity_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelAmenity_Hotel",
                        column: x => x.hotel_id,
                        principalTable: "tblHotels",
                        principalColumn: "hotel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__tblUsers__AB6E616422F1317D",
                table: "tblUsers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__tblHotel__72E12F1BED7C40EC",
                table: "tblHotels",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__tblBooki__BADA455927423170",
                table: "tblBookings",
                column: "booking_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__lkpAmeni__72E12F1B6FB36A1B",
                table: "lkpAmenities",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblHotelAmenities_amenity_id",
                table: "tblHotelAmenities",
                column: "amenity_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblWishList_tblHotels_hotel_id",
                table: "tblWishList",
                column: "hotel_id",
                principalTable: "tblHotels",
                principalColumn: "hotel_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
