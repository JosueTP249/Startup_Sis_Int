using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Frontedn_Posture_AI.Data;
using Frontedn_Posture_AI.Models;
using Frontedn_Posture_AI.Models.Entidades;

namespace Frontedn_Posture_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionFisicaController : ControllerBase
    {
        private readonly InferenceSession _session;
        private readonly AppDbContext _db;
        private readonly EstadoBaseDatos _estadoBd;
        private readonly ControlRegistros _control;
        private readonly ILogger<EvaluacionFisicaController> _logger;

        // Inyección de dependencias configurada en Program.cs
        public EvaluacionFisicaController(
            InferenceSession session,
            AppDbContext db,
            EstadoBaseDatos estadoBd,
            ControlRegistros control,
            ILogger<EvaluacionFisicaController> logger)
        {
            _session = session;
            _db = db;
            _estadoBd = estadoBd;
            _control = control;
            _logger = logger;
        }

        [HttpPost("Clasificar")]
        public async Task<IActionResult> Clasificar([FromBody] PostureRequest payload)
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

                //La confianza de la IA
                if (maxProb < 0.90f)
                {
                    //Es menor al 90%, forzamos un índice que no existe
                    //default y devuelva "Peligro"
                    maxIndex = -1;
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

                // 7. Persistencia en la base de datos (historial de ejercicios)
                await GuardarEvaluacionAsync(payload.PacienteId ?? AppDbContext.PacienteDemoId,
                                             maxIndex, mensaje, estado, maxProb);

                return Ok(new { estado = estado, mensaje = mensaje });
            }
            catch (Exception ex)
            {
                // Registramos el error internamente pero no rompemos la app del cliente
                Console.WriteLine($"Error de inferencia: {ex.Message}");
                return StatusCode(500, new { estado = "error", mensaje = "Fallo en el análisis biométrico." });
            }
        }

        /// <summary>
        /// Guarda la evaluación en EvaluacionesPosturales. Las posturas de riesgo
        /// siempre se registran (limitadas a una cada 5 s por paciente) y los
        /// ejercicios reconocidos solo cuando cambia la clase. Si la base de datos
        /// falla, la respuesta de la IA al usuario no se interrumpe.
        /// </summary>
        private async Task GuardarEvaluacionAsync(int pacienteId, int claseIndex, string mensaje, string estado, float confianza)
        {
            if (!_estadoBd.Habilitada || !_control.DebeGuardar(pacienteId, estado, claseIndex))
            {
                return;
            }

            try
            {
                _db.EvaluacionesPosturales.Add(new EvaluacionPostural
                {
                    UsuarioId = pacienteId,
                    EjercicioId = claseIndex >= 0 ? claseIndex : null,
                    FechaHora = DateTime.UtcNow,
                    ClaseDetectada = mensaje,
                    Estado = estado,
                    Confianza = confianza
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo guardar la evaluación del paciente {PacienteId}", pacienteId);
            }
        }
    }
}
