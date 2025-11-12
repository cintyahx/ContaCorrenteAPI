using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Miotto.BankMore.Conta.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContaCorrente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Numero = table.Column<int>(type: "int", maxLength: 10, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cpf = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Saldo = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaCorrente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movimento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false, defaultValue: new DateOnly(2025, 11, 12)),
                    Tipo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Valor = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    ContaCorrenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimento_ContaCorrente_ContaCorrenteId",
                        column: x => x.ContaCorrenteId,
                        principalTable: "ContaCorrente",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimento_ContaCorrenteId",
                table: "Movimento",
                column: "ContaCorrenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimento");

            migrationBuilder.DropTable(
                name: "ContaCorrente");
        }
    }
}
