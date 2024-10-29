using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateApi.Migrations
{
    /// <inheritdoc />
    public partial class _20241029Oliver1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Table1Log",
                columns: table => new
                {
                    Table1LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExcuteTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Table1Id = table.Column<int>(type: "int", nullable: false),
                    Column1 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table1Log", x => x.Table1LogId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Table1Log");
        }
    }
}
