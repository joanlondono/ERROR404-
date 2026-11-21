using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevWork.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUserIdAPreguntaV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Preguntas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Preguntas");
        }
    }
}
