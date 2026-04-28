using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedCore.Migrations
{
    /// <inheritdoc />
    public partial class FixChatHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "ChatSessions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "ChatMessages",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ChatSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChatSessions");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ChatSessions",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ChatMessages",
                newName: "LastModified");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "ChatSessions",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "ChatMessages",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
