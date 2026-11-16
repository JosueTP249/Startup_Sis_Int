using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Text.Json.Nodes;

namespace Frontedn_Posture_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionFisicaController : ControllerBase
    {
        private readonly InferenceSession _session;

        // El constructor carga el modelo ONNX en memoria cuando la API arranca
        public EvaluacionFisicaController()
        {
            // Ajusta la ruta a donde guardaron el modelo exportado desde Python
            _session = new InferenceSession("MLModels/postura_model.onnx");
        }

        [HttpPost("Clasificar")]
        public IActionResult Clasificar([FromBody] JsonObject payload)
        {
            try
            {
                // 1. Extraer los datos numéricos (x, y, z, visibilidad) del JSON enviado por MediaPipe
                var keypoints = payload["keypoints"]?.AsArray();
                if (keypoints == null || keypoints.Count == 0)
                {
                    return BadRequest("No se detectaron landmarks del cuerpo.");
                }

                // 2. Aplanar los datos para el Perceptrón Multicapa (MLP)
                // MediaPipe detecta 33 puntos, cada uno con x, y, z, visibility.
                // Ajusta este tamaño según con cuántas variables entrenaron el modelo en Colab
                float[] inputData = new float[33 * 4];
                int index = 0;

                foreach (var kp in keypoints)
                {
                    inputData[index++] = (float)kp["x"];
                    inputData[index++] = (float)kp["y"];
                    inputData[index++] = (float)kp["z"];
                    inputData[index++] = (float)kp["visibility"];
                }

                // 3. Crear el tensor para ONNX
                var tensor = new DenseTensor<float>(inputData, new[] { 1, inputData.Length });
                var inputs = new List<NamedOnnxValue>
                {
                    // "dense_input" debe coincidir con el nombre de la capa de entrada de tu modelo Keras
                    NamedOnnxValue.CreateFromTensor("dense_input", tensor)
                };

                // 4. Ejecutar la inferencia (¡la magia de la IA ocurre aquí!)
                using var results = _session.Run(inputs);

                // 5. Obtener los resultados (probabilidades softmax)
                var outputTensor = results.First().AsTensor<float>();

                // 6. Encontrar la clase con mayor probabilidad
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

                // 7. Mapear el índice a tu lógica de negocio y devolver respuesta al frontend
                if (maxIndex == 0) // Asumiendo que 0 es postura incorrecta/peligro
                {
                    return Ok(new { estado = "Peligro", mensaje = "¡Corrige tu postura!" });
                }
                else
                {
                    return Ok(new { estado = "Bien", mensaje = "Postura correcta. Mantén así." });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = "error", mensaje = "Error en el servidor de IA." });
            }
        }
    }
}