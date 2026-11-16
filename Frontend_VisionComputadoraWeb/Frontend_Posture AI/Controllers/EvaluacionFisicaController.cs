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
                //Aplanar el arreglo de clases a un arreglo unidimensional
                //MediaPipe envía 33 puntos con 4 valores (X,Y,Z,Visibility) = 132 entradas
                float[] inputData = new float[payload.Keypoints.Count * 4];
                int index = 0;

                foreach (var kp in payload.Keypoints)
                {
                    inputData[index++] = kp.X;
                    inputData[index++] = kp.Y;
                    inputData[index++] = kp.Z;
                    inputData[index++] = kp.Visibility;
                }

                //Crear Tensor para ONNX
                var tensor = new DenseTensor<float>(inputData, new[] { 1, inputData.Length });

                //"dense_input" es el nombre típico, pero depende de cómo lo exportó el Científico de Datos.
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("dense_input", tensor)
                };

                //Ejecutar Inferencia
                using var results = _session.Run(inputs);
                var outputTensor = results.First().AsTensor<float>();

                //Calcular el ArgMax (La clase ganadora)
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

                //Respuesta de Negocio
                return maxIndex switch
                {
                    0 => Ok(new { estado = "Peligro", mensaje = "Espalda encorvada. \n Observacio[Peligro de lesión]" }),
                    1 => Ok(new { estado = "Bien", mensaje = "Postura correcta. \n Observacio[Sigue asi]" }),
                    _ => Ok(new { estado = "Neutro", mensaje = "Analizando movimiento..." })
                };
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