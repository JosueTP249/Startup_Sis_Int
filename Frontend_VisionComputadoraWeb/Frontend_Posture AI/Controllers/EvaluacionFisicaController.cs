using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Frontedn_Posture_AI.Models;

namespace Frontedn_Posture_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionFisicaController : ControllerBase
    {
        private readonly InferenceSession _session;

        // Inyección de dependencias configurada en Program.cs
        public EvaluacionFisicaController(InferenceSession session)
        {
            _session = session;
        }

        [HttpPost("Clasificar")]
        public IActionResult Clasificar([FromBody] PostureRequest payload)
        {
            if (payload?.Keypoints == null || payload.Keypoints.Count == 0)
            {
                return BadRequest(new { estado = "error", mensaje = "Faltan coordenadas." });
            }
            try
            {
                //Aplanar los datos: MediaPipe 33 puntos. 
                float[] inputData = new float[33 * 3];
                int index = 0;

                foreach (var kp in payload.Keypoints)
                {
                    inputData[index++] = kp.X;
                    inputData[index++] = kp.Y;
                    inputData[index++] = kp.Z;
                }

                //Tensor para ONNX con la forma exacta  Netron: [1, 33, 3]
                var tensor = new DenseTensor<float>(inputData, new[] { 1, 33, 3 });

                //Usar el nombre Netron: "landmarks"
                var inputs = new List<NamedOnnxValue>
    {
        NamedOnnxValue.CreateFromTensor("landmarks", tensor)
    };

                //Ejecutar Inferencia
                using var results = _session.Run(inputs);
                var outputTensor = results.First().AsTensor<float>();

                //Calcular el ArgMax (La clase ganadora entre las 13 posibles)
                float maxProb = -1;
                int maxIndex = -1;
                for (int i = 0; i < outputTensor.Length; i++)
                {
                    if (outputTensor.GetValue(i) > maxProb)
                    {
                        maxProb = outputTensor.GetValue(i);
                        maxIndex = i;
                    }
                }

                // 6. Respuesta de Negocio
                string estado = "Bien"; // Por defecto
                string mensaje = "";

                switch (maxIndex)
                {
                    case 0: mensaje = "Puente de glúteo (Abajo)"; break;
                    case 1: mensaje = "Puente de glúteo (Arriba)"; break;
                    case 2: mensaje = "Elevación de rodillas (Abajo)"; break;
                    case 3: mensaje = "Elevación de rodillas (Arriba)"; break;
                    case 4: mensaje = "Elevación lateral de pierna (Abajo)"; break;
                    case 5: mensaje = "Elevación lateral de pierna (Arriba)"; break;
                    case 6: mensaje = "Elevación lateral de brazos (Abajo)"; break;
                    case 7: mensaje = "Elevación lateral de brazos (Arriba)"; break;
                    case 8: mensaje = "Elevación de piernas (Abajo)"; break;
                    case 9: mensaje = "Elevación de piernas (Arriba)"; break;
                    case 10: mensaje = "Flexión de hombro (Arriba)"; break;
                    case 11: mensaje = "Press de hombro (Abajo)"; break;
                    case 12: mensaje = "Press de hombro (Arriba)"; break;
                    default:
                        mensaje = $"Postura desconocida ({maxIndex})";
                        estado = "Peligro";
                        break;
                }

                
                return Ok(new { estado = estado, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                // Registramos el error internamente pero no rompemos la app del cliente
                Console.WriteLine($"Error de inferencia: {ex.Message}");
                return StatusCode(500, new { estado = "error", mensaje = "Fallo en el análisis biométrico." });
            }
        }
    }
}