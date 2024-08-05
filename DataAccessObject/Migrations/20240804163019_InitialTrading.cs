using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuIntegrator24DAO.Migrations
{
    /// <inheritdoc />
    public partial class InitialTrading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "LowestPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "HighestPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<ulong>(
                name: "FinalBidVolume",
                table: "Tradings",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalBidPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<ulong>(
                name: "FinalAskVolume",
                table: "Tradings",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalAskPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosingPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "AveragePrice",
                table: "Tradings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarketType",
                table: "Tradings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AveragePrice",
                table: "Tradings");

            migrationBuilder.DropColumn(
                name: "MarketType",
                table: "Tradings");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpeningPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "LowestPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "HighestPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "FinalBidVolume",
                table: "Tradings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalBidPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<ulong>(
                name: "FinalAskVolume",
                table: "Tradings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalAskPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ClosingPrice",
                table: "Tradings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
