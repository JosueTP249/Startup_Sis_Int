using System.ComponentModel.DataAnnotations;

namespace Frontedn_Posture_AI.Models.Entidades
{
    /// <summary>
    /// Catálogo de las 13 clases que reconoce el modelo MLP (postura_model.onnx).
    /// El Id coincide con el índice de salida del modelo (0 a 12).
    /// </summary>
    public class Ejercicio
    {
        // No autoincremental: es el índice de la clase en la salida del modelo
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        // "Arriba" o "Abajo"
        [Required, MaxLength(20)]
        public string Fase { get; set; } = string.Empty;

        public ICollection<EvaluacionPostural> Evaluaciones { get; set; } = new List<EvaluacionPostural>();
    }
}
