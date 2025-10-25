using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travely.Migrations
{
    /// <inheritdoc />
    public partial class AddWishListHotelRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tblWishList_hotel_id",
                table: "tblWishList",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblWishList_tblHotels_hotel_id",
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
                name: "FK_tblWishList_tblHotels_hotel_id",
                table: "tblWishList");

            migrationBuilder.DropIndex(
                name: "IX_tblWishList_hotel_id",
                table: "tblWishList");
        }
    }
}
