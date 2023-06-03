using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Admin.Migrations
{
    public partial class v7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "arquivos");

            migrationBuilder.CreateTable(
                name: "arquivo",
                schema: "arquivos",
                columns: table => new
                {
                    ArquivoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Dado = table.Column<byte[]>(type: "bytea", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deletado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arquivo", x => x.ArquivoId);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                schema: "arquivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Deletado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArquivoUsuario",
                schema: "arquivos",
                columns: table => new
                {
                    ArquivosArquivoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArquivoUsuario", x => new { x.ArquivosArquivoId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_ArquivoUsuario_arquivo_ArquivosArquivoId",
                        column: x => x.ArquivosArquivoId,
                        principalSchema: "arquivos",
                        principalTable: "arquivo",
                        principalColumn: "ArquivoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArquivoUsuario_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "arquivos",
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArquivoUsuario_UsuarioId",
                schema: "arquivos",
                table: "ArquivoUsuario",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArquivoUsuario",
                schema: "arquivos");

            migrationBuilder.DropTable(
                name: "arquivo",
                schema: "arquivos");

            migrationBuilder.DropTable(
                name: "usuario",
                schema: "arquivos");
        }
    }
}
