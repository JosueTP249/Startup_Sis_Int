using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Frontedn_Posture_AI.Migrations
{
    /// <inheritdoc />
    public partial class InicialPostureAI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Plan = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Ciudad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ejercicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fase = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejercicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Empresa = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pacientes_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesPosturales",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EjercicioId = table.Column<int>(type: "integer", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClaseDetectada = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Confianza = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesPosturales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionesPosturales_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EvaluacionesPosturales_Pacientes_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clinicas",
                columns: new[] { "Id", "Activa", "Ciudad", "FechaAlta", "Nombre", "Plan" },
                values: new object[] { 1, true, "Demo", new DateTime(2026, 11, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Clínica Demo Posture AI", "Profesional" });

            migrationBuilder.InsertData(
                table: "Ejercicios",
                columns: new[] { "Id", "Fase", "Nombre" },
                values: new object[,]
                {
                    { 0, "Abajo", "Puente de glúteo" },
                    { 1, "Arriba", "Puente de glúteo" },
                    { 2, "Abajo", "Elevación de rodillas" },
                    { 3, "Arriba", "Elevación de rodillas" },
                    { 4, "Abajo", "Elevación lateral de pierna" },
                    { 5, "Arriba", "Elevación lateral de pierna" },
                    { 6, "Abajo", "Elevación lateral de brazos" },
                    { 7, "Arriba", "Elevación lateral de brazos" },
                    { 8, "Abajo", "Elevación de piernas" },
                    { 9, "Arriba", "Elevación de piernas" },
                    { 10, "Arriba", "Flexión de hombro" },
                    { 11, "Abajo", "Press de hombro" },
                    { 12, "Arriba", "Press de hombro" }
                });

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "Id", "ClinicaId", "Empresa", "FechaRegistro", "Nombre" },
                values: new object[] { 1, 1, "Jurado Pitch", new DateTime(2026, 11, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Paciente Demo" });

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_Nombre",
                table: "Clinicas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesPosturales_EjercicioId",
                table: "EvaluacionesPosturales",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesPosturales_Estado",
                table: "EvaluacionesPosturales",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesPosturales_UsuarioId_FechaHora",
                table: "EvaluacionesPosturales",
                columns: new[] { "UsuarioId", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_ClinicaId",
                table: "Pacientes",
                column: "ClinicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluacionesPosturales");

            migrationBuilder.DropTable(
                name: "Ejercicios");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Clinicas");
        }
    }
}
