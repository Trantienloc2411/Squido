using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class v3_1_Update_Order_RatingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompleteTime",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Users",
                newName: "WardName");

            migrationBuilder.RenameColumn(
                name: "ShippedDate",
                table: "Orders",
                newName: "ConfirmDate");

            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "Orders",
                newName: "CompleteDate");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeAddress",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HomeAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "WardName",
                table: "Users",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "ConfirmDate",
                table: "Orders",
                newName: "ShippedDate");

            migrationBuilder.RenameColumn(
                name: "CompleteDate",
                table: "Orders",
                newName: "DeliveryDate");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompleteTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }
    }
}
