using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FutureLife.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class V2_NewSimulationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimulationResults_Profiles_ProfileId",
                table: "SimulationResults");

            migrationBuilder.RenameColumn(
                name: "ResultJson",
                table: "SimulationResults",
                newName: "YearlySnapshotsJson");

            migrationBuilder.RenameColumn(
                name: "ProjectionYears",
                table: "SimulationResults",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "SimulationResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "BurnoutRisk",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CareerGrowthIndex",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CareerStagnationRisk",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EnergyDepletionRisk",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EnergyScore10Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EnergyScore1Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EnergyScore5Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FinancialCollapseRisk",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HealthScore10Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HealthScore1Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HealthScore5Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "InputJson",
                table: "SimulationResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "IsolationRisk",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LifeStrategyScore",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MonthlySavings",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "MonthlySnapshotsJson",
                table: "SimulationResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SimulationResults",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "NetWorth10Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OverallRiskIndex",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PromotionProbability",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SalaryMultiplier",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Savings10Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Savings1Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Savings5Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SocialBalanceScore",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StudyHours10Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StudyHours1Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StudyHours5Y",
                table: "SimulationResults",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationResults_UserId",
                table: "SimulationResults",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationResults_Profiles_ProfileId",
                table: "SimulationResults",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationResults_Users_UserId",
                table: "SimulationResults",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimulationResults_Profiles_ProfileId",
                table: "SimulationResults");

            migrationBuilder.DropForeignKey(
                name: "FK_SimulationResults_Users_UserId",
                table: "SimulationResults");

            migrationBuilder.DropIndex(
                name: "IX_SimulationResults_UserId",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BurnoutRisk",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "CareerGrowthIndex",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "CareerStagnationRisk",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "EnergyDepletionRisk",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "EnergyScore10Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "EnergyScore1Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "EnergyScore5Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "FinancialCollapseRisk",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "HealthScore10Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "HealthScore1Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "HealthScore5Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "InputJson",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "IsolationRisk",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "LifeStrategyScore",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "MonthlySavings",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "MonthlySnapshotsJson",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "NetWorth10Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "OverallRiskIndex",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "PromotionProbability",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "SalaryMultiplier",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "Savings10Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "Savings1Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "Savings5Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "SocialBalanceScore",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "StudyHours10Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "StudyHours1Y",
                table: "SimulationResults");

            migrationBuilder.DropColumn(
                name: "StudyHours5Y",
                table: "SimulationResults");

            migrationBuilder.RenameColumn(
                name: "YearlySnapshotsJson",
                table: "SimulationResults",
                newName: "ResultJson");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SimulationResults",
                newName: "ProjectionYears");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "SimulationResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SimulationResults_Profiles_ProfileId",
                table: "SimulationResults",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
