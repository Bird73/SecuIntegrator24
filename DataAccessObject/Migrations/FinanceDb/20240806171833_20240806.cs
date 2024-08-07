using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecuIntegrator24DAO.Migrations.FinanceDb
{
    /// <inheritdoc />
    public partial class _20240806 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlyRevenues",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Month = table.Column<int>(type: "INTEGER", nullable: false),
                    MarketType = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentMonthRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    PreviousMonthRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastYearMonthRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    MonthOverMonthChangePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    YearOverYearChangePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrentMonthCumulativeRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastYearCumulativeRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    CumulativeChangePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    Comments = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyRevenues", x => new { x.Code, x.Year, x.Month });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyRevenues");
        }
    }
}
