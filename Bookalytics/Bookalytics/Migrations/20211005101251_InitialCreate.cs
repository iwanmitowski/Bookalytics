using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookalytics.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WordsCount = table.Column<int>(type: "int", nullable: false),
                    ShortestWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongestWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostCommonWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MostCommonWordCount = table.Column<int>(type: "int", nullable: false),
                    LeastCommonWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeastCommonWordCount = table.Column<int>(type: "int", nullable: false),
                    AverageWordLength = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
