using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuIntegrator24DAO.Migrations
{
    /// <inheritdoc />
    public partial class _20240803 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tradings",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    TradingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TradeVolume = table.Column<decimal>(type: "TEXT", nullable: false),
                    Transaction = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TradeValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    OpeningPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    HighestPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    LowestPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ClosingPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChangeIndicator = table.Column<char>(type: "TEXT", nullable: true),
                    Change = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChangeValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    FinalBidPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    FinalBidVolume = table.Column<ulong>(type: "INTEGER", nullable: false),
                    FinalAskPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    FinalAskVolume = table.Column<ulong>(type: "INTEGER", nullable: false),
                    PERatio = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tradings", x => new { x.Code, x.TradingDate });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tradings");
        }
    }
}
