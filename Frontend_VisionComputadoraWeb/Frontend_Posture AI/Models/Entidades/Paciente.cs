using System.ComponentModel.DataAnnotations;

namespace Frontedn_Posture_AI.Models.Entidades
{
    /// <summary>
    /// Usuario final evaluado por la cámara. Pertenece a una clínica (cliente B2B).
    /// Equivale a la tabla "Usuarios" (Id, Nombre, Empresa) de la guía del Rol 4.
    /// </summary>
    public class Paciente
    {
        public int Id { get; set; }

        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // Empresa o institución de procedencia (útil para el enfoque ergonómico corporativo)
        [MaxLength(150)]
        public string? Empresa { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<EvaluacionPostural> Evaluaciones { get; set; } = new List<EvaluacionPostural>();
    }
}
