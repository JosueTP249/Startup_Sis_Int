//Mapear lo que se envia de MediaPipe desde el JS

namespace Frontedn_Posture_AI.Models
{
    public class PostureRequest
    {
        public List<Landmark>? Keypoints { get; set; }

        // Paciente evaluado. Si el frontend no lo envía se usa el paciente demo.
        public int? PacienteId { get; set; }
    }

    public class Landmark
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Visibility { get; set; }
    }
}