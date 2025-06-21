using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EruMobil.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationBelIsActive",
                table: "Devices",
                newName: "NotificationsIsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationsIsActive",
                table: "Devices",
                newName: "NotificationBelIsActive");
        }
    }
}
