using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travely.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lkpAmenities",
                columns: table => new
                {
                    amenity_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lkpAmeni__E908452D58205242", x => x.amenity_id);
                });

            migrationBuilder.CreateTable(
                name: "tblHotels",
                columns: table => new
                {
                    hotel_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    stars = table.Column<byte>(type: "tinyint", nullable: true),
                    contact_info = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    location = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    check_in_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    check_out_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    cancellation_policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fees = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    commission = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblHotel__45FE7E2639928AE5", x => x.hotel_id);
                });

            migrationBuilder.CreateTable(
                name: "tblUsers",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    age = table.Column<byte>(type: "tinyint", nullable: true),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "customer"),
                    imagepath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "active"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblUsers__B9BE370F4D5024FC", x => x.user_id);
                });

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

            migrationBuilder.CreateTable(
                name: "tblHotelImages",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblHotel__DC9AC95586FE2E59", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_HotelImage_Hotel",
                        column: x => x.hotel_id,
                        principalTable: "tblHotels",
                        principalColumn: "hotel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblRooms",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    room_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    room_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    beds_count = table.Column<byte>(type: "tinyint", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    max_guests = table.Column<byte>(type: "tinyint", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    breakfast_included = table.Column<bool>(type: "bit", nullable: false),
                    pets_allowed = table.Column<bool>(type: "bit", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblRooms__19675A8A9EDAE80E", x => x.room_id);
                    table.ForeignKey(
                        name: "FK_Room_Hotel",
                        column: x => x.hotel_id,
                        principalTable: "tblHotels",
                        principalColumn: "hotel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblUserHotelBooking",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    booking_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblUserH__5DE3A5B18D70EFC5", x => x.booking_id);
                    table.ForeignKey(
                        name: "FK_UHB_Hotel",
                        column: x => x.hotel_id,
                        principalTable: "tblHotels",
                        principalColumn: "hotel_id");
                    table.ForeignKey(
                        name: "FK_UHB_User",
                        column: x => x.user_id,
                        principalTable: "tblUsers",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tblWishList",
                columns: table => new
                {
                    wishlist_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    hotel_id = table.Column<int>(type: "int", nullable: false),
                    added_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblWishL__6151514E8CC42492", x => x.wishlist_id);
                    table.ForeignKey(
                        name: "FK_WishList_User",
                        column: x => x.user_id,
                        principalTable: "tblUsers",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblBookings",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    check_in = table.Column<DateOnly>(type: "date", nullable: true),
                    check_out = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "pending"),
                    total_price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    booking_reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    adults = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    children = table.Column<byte>(type: "tinyint", nullable: true, defaultValue: (byte)0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblBooki__5DE3A5B18C8ACDC1", x => x.booking_id);
                    table.ForeignKey(
                        name: "FK_Booking_Room",
                        column: x => x.room_id,
                        principalTable: "tblRooms",
                        principalColumn: "room_id");
                    table.ForeignKey(
                        name: "FK_Booking_User",
                        column: x => x.user_id,
                        principalTable: "tblUsers",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tblRoomImages",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblRoomI__DC9AC955108AD689", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_RoomImage_Room",
                        column: x => x.room_id,
                        principalTable: "tblRooms",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblReviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<byte>(type: "tinyint", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    review_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    helpful_count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblRevie__60883D90A84AA0C2", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_Review_Booking",
                        column: x => x.booking_id,
                        principalTable: "tblUserHotelBooking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblPayments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "pending"),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tblPayme__ED1FC9EA50A456C9", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_Payment_Booking",
                        column: x => x.booking_id,
                        principalTable: "tblBookings",
                        principalColumn: "booking_id");
                    table.ForeignKey(
                        name: "FK_Payment_User",
                        column: x => x.user_id,
                        principalTable: "tblUsers",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__lkpAmeni__72E12F1B6FB36A1B",
                table: "lkpAmenities",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblBookings_room_id",
                table: "tblBookings",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_tblBookings_user_id",
                table: "tblBookings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tblBooki__BADA455927423170",
                table: "tblBookings",
                column: "booking_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblHotelAmenities_amenity_id",
                table: "tblHotelAmenities",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "IX_tblHotelImages_hotel_id",
                table: "tblHotelImages",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tblHotel__72E12F1BED7C40EC",
                table: "tblHotels",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPayments_booking_id",
                table: "tblPayments",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_tblPayments_user_id",
                table: "tblPayments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tblRevie__5DE3A5B0889ABF13",
                table: "tblReviews",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblRoomImages_room_id",
                table: "tblRoomImages",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_tblRooms_hotel_id",
                table: "tblRooms",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserHotelBooking_hotel_id",
                table: "tblUserHotelBooking",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_tblUserHotelBooking_user_id",
                table: "tblUserHotelBooking",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ__tblUsers__AB6E616422F1317D",
                table: "tblUsers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblWishList_user_id",
                table: "tblWishList",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblHotelAmenities");

            migrationBuilder.DropTable(
                name: "tblHotelImages");

            migrationBuilder.DropTable(
                name: "tblPayments");

            migrationBuilder.DropTable(
                name: "tblReviews");

            migrationBuilder.DropTable(
                name: "tblRoomImages");

            migrationBuilder.DropTable(
                name: "tblWishList");

            migrationBuilder.DropTable(
                name: "lkpAmenities");

            migrationBuilder.DropTable(
                name: "tblBookings");

            migrationBuilder.DropTable(
                name: "tblUserHotelBooking");

            migrationBuilder.DropTable(
                name: "tblRooms");

            migrationBuilder.DropTable(
                name: "tblUsers");

            migrationBuilder.DropTable(
                name: "tblHotels");
        }
    }
}
