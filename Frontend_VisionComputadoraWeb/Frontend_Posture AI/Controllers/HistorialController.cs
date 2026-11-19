using Frontedn_Posture_AI.Data;
using Frontedn_Posture_AI.Models.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Frontedn_Posture_AI.Controllers
{
    /// <summary>
    /// Consultas sobre la base de datos para el modelo SaaS B2B: las clínicas
    /// pueden ver el historial de sus pacientes y métricas agregadas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly EstadoBaseDatos _estadoBd;

        public HistorialController(AppDbContext db, EstadoBaseDatos estadoBd)
        {
            _db = db;
            _estadoBd = estadoBd;
        }

        private IActionResult SinBaseDatos() =>
            StatusCode(503, new { mensaje = "La base de datos no está configurada." });

        // GET api/Historial/clinicas
        [HttpGet("clinicas")]
        public async Task<IActionResult> Clinicas()
        {
            if (!_estadoBd.Habilitada) return SinBaseDatos();

            var clinicas = await _db.Clinicas
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Plan,
                    c.Ciudad,
                    c.Activa,
                    Pacientes = c.Pacientes.Count
                })
                .ToListAsync();

            return Ok(clinicas);
        }

        // GET api/Historial/clinica/1/pacientes
        [HttpGet("clinica/{clinicaId:int}/pacientes")]
        public async Task<IActionResult> PacientesDeClinica(int clinicaId)
        {
            if (!_estadoBd.Habilitada) return SinBaseDatos();

            var pacientes = await _db.Pacientes
                .AsNoTracking()
                .Where(p => p.ClinicaId == clinicaId)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Empresa,
                    p.FechaRegistro,
                    Evaluaciones = p.Evaluaciones.Count,
                    Alertas = p.Evaluaciones.Count(e => e.Estado == "Peligro")
                })
                .ToListAsync();

            return Ok(pacientes);
        }

        // POST api/Historial/pacientes   { "clinicaId": 1, "nombre": "...", "empresa": "..." }
        [HttpPost("pacientes")]
        public async Task<IActionResult> RegistrarPaciente([FromBody] NuevoPacienteRequest request)
        {
            if (!_estadoBd.Habilitada) return SinBaseDatos();
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest(new { mensaje = "El nombre es obligatorio." });
            }
            if (!await _db.Clinicas.AnyAsync(c => c.Id == request.ClinicaId))
            {
                return NotFound(new { mensaje = "La clínica no existe." });
            }

            var paciente = new Paciente
            {
                ClinicaId = request.ClinicaId,
                Nombre = request.Nombre.Trim(),
                Empresa = request.Empresa?.Trim(),
                FechaRegistro = DateTime.UtcNow
            };
            _db.Pacientes.Add(paciente);
            await _db.SaveChangesAsync();

            return Created($"/api/Historial/paciente/{paciente.Id}",
                new { paciente.Id, paciente.ClinicaId, paciente.Nombre, paciente.Empresa });
        }

        // GET api/Historial/paciente/1?limite=50&soloRiesgo=true
        [HttpGet("paciente/{pacienteId:int}")]
        public async Task<IActionResult> HistorialPaciente(int pacienteId, int limite = 50, bool soloRiesgo = false)
        {
            if (!_estadoBd.Habilitada) return SinBaseDatos();

            var paciente = await _db.Pacientes
                .AsNoTracking()
                .Include(p => p.Clinica)
                .FirstOrDefaultAsync(p => p.Id == pacienteId);
            if (paciente == null)
            {
                return NotFound(new { mensaje = "El paciente no existe." });
            }

            var consulta = _db.EvaluacionesPosturales
                .AsNoTracking()
                .Where(e => e.UsuarioId == pacienteId);
            if (soloRiesgo)
            {
                consulta = consulta.Where(e => e.Estado == "Peligro");
            }

            var evaluaciones = await consulta
                .OrderByDescending(e => e.FechaHora)
                .Take(Math.Clamp(limite, 1, 500))
                .Select(e => new
                {
                    e.Id,
                    e.FechaHora,
                    e.ClaseDetectada,
                    e.Estado,
                    Confianza = Math.Round(e.Confianza * 100, 1)
                })
                .ToListAsync();

            return Ok(new
            {
                paciente = new { paciente.Id, paciente.Nombre, paciente.Empresa, Clinica = paciente.Clinica?.Nombre },
                total = evaluaciones.Count,
                evaluaciones
            });
        }

        // GET api/Historial/clinica/1/metricas
        [HttpGet("clinica/{clinicaId:int}/metricas")]
        public async Task<IActionResult> MetricasClinica(int clinicaId)
        {
            if (!_estadoBd.Habilitada) return SinBaseDatos();

            var clinica = await _db.Clinicas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clinicaId);
            if (clinica == null)
            {
                return NotFound(new { mensaje = "La clínica no existe." });
            }

            var evaluaciones = _db.EvaluacionesPosturales
                .AsNoTracking()
                .Where(e => e.Paciente!.ClinicaId == clinicaId);

            var total = await evaluaciones.CountAsync();
            var alertas = await evaluaciones.CountAsync(e => e.Estado == "Peligro");
            var pacientesActivos = await evaluaciones.Select(e => e.UsuarioId).Distinct().CountAsync();
            var ultima = await evaluaciones.MaxAsync(e => (DateTime?)e.FechaHora);

            var porEjercicio = await evaluaciones
                .Where(e => e.EjercicioId != null)
                .GroupBy(e => new { e.Ejercicio!.Nombre, e.Ejercicio.Fase })
                .Select(g => new { Ejercicio = g.Key.Nombre, g.Key.Fase, Repeticiones = g.Count() })
                .OrderByDescending(x => x.Repeticiones)
                .ToListAsync();

            return Ok(new
            {
                clinica = new { clinica.Id, clinica.Nombre, clinica.Plan },
                totalEvaluaciones = total,
                alertasRiesgo = alertas,
                porcentajeRiesgo = total == 0 ? 0 : Math.Round(alertas * 100.0 / total, 1),
                pacientesActivos,
                ultimaEvaluacion = ultima,
                porEjercicio
            });
        }
    }

    public class NuevoPacienteRequest
    {
        public int ClinicaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Empresa { get; set; }
    }
}
