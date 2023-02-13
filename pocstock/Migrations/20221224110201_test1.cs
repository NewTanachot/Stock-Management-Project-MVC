using System;
using Microsoft.EntityFrameworkCore.Migrations;
using pocstock.Data;

#nullable disable

namespace pocstock.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryStatus",
                columns: table => new
                {
                    HistoryStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HistoryStatusName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryStatus", x => x.HistoryStatusId);
                });

            migrationBuilder.CreateTable(
                name: "JobDetail",
                columns: table => new
                {
                    JobId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    JobDescription = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    JobWage = table.Column<int>(type: "INTEGER", nullable: false),
                    JobStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    JobLocation = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TaxId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDetail", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "JobHistoryLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobNo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobHistoryLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobStatus",
                columns: table => new
                {
                    JobStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobStatusName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatus", x => x.JobStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SellingPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    CostPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    StockTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockHistoryLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    JobNo = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockHistoryLog", x => x.Id);
                });

            // Add Initial Data in Migration File
            migrationBuilder.Sql(InitialMasterData.HistoryStatus_SQLite);
            migrationBuilder.Sql(InitialMasterData.JobStatus_SQLite);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryStatus");

            migrationBuilder.DropTable(
                name: "JobDetail");

            migrationBuilder.DropTable(
                name: "JobHistoryLog");

            migrationBuilder.DropTable(
                name: "JobProduct");

            migrationBuilder.DropTable(
                name: "JobStatus");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "StockHistoryLog");
        }
    }
}
