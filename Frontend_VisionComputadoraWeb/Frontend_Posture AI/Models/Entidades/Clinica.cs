using System.ComponentModel.DataAnnotations;

namespace Frontedn_Posture_AI.Models.Entidades
{
    /// <summary>
    /// Cliente B2B del modelo SaaS: clínica de fisioterapia, centro de
    /// rehabilitación o empresa que contrata Posture AI para sus pacientes.
    /// </summary>
    public class Clinica
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        // Plan de suscripción SaaS: Basico, Profesional o Empresarial
        [Required, MaxLength(30)]
        public string Plan { get; set; } = "Basico";

        [MaxLength(100)]
        public string? Ciudad { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.UtcNow;

        public bool Activa { get; set; } = true;

        public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
    }
}
