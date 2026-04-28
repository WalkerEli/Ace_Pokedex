using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamAceProject.Migrations
{
    /// <inheritdoc />
    public partial class AddPokemonTypesToTeamMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PokemonTypes",
                table: "TeamMembers",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PokemonTypes",
                table: "TeamMembers");
        }
    }
}
