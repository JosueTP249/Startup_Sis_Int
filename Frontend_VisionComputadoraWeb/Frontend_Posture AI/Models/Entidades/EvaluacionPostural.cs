using System.ComponentModel.DataAnnotations;

namespace Frontedn_Posture_AI.Models.Entidades
{
    /// <summary>
    /// Historial de ejercicios: cada registro es una evaluación hecha por la IA
    /// sobre un paciente. Las posturas de riesgo quedan con Estado = "Peligro".
    /// </summary>
    public class EvaluacionPostural
    {
        public long Id { get; set; }

        // Se mantiene el nombre UsuarioId de la guía del Rol 4 (apunta a Pacientes)
        public int UsuarioId { get; set; }
        public Paciente? Paciente { get; set; }

        // Null cuando la IA no reconoce el ejercicio (confianza menor al umbral)
        public int? EjercicioId { get; set; }
        public Ejercicio? Ejercicio { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        // Texto devuelto al usuario, ej. "Press de hombro (Arriba)"
        [Required, MaxLength(120)]
        public string ClaseDetectada { get; set; } = string.Empty;

        // "Bien" o "Peligro"
        [Required, MaxLength(20)]
        public string Estado { get; set; } = string.Empty;

        // Probabilidad máxima de la salida del modelo (0 a 1)
        public float Confianza { get; set; }
    }
}
