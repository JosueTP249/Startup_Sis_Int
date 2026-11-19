using System.Collections.Concurrent;

namespace Frontedn_Posture_AI.Data
{
    /// <summary>
    /// El frontend envía ~3 fotogramas por segundo. Guardar todos llenaría la
    /// base de datos en minutos, así que esta clase decide qué evaluaciones
    /// vale la pena persistir:
    ///  - Postura de riesgo ("Peligro"): como máximo una cada 5 segundos por paciente.
    ///  - Ejercicio reconocido ("Bien"): solo cuando cambia la clase detectada
    ///    (así el historial registra cada repetición Arriba/Abajo, no cada fotograma).
    /// Se registra como Singleton en Program.cs.
    /// </summary>
    public class ControlRegistros
    {
        private static readonly TimeSpan IntervaloPeligro = TimeSpan.FromSeconds(5);

        private readonly ConcurrentDictionary<int, DateTime> _ultimoPeligro = new();
        private readonly ConcurrentDictionary<int, int> _ultimaClase = new();

        public bool DebeGuardar(int pacienteId, string estado, int claseIndex)
        {
            var ahora = DateTime.UtcNow;

            if (estado == "Peligro")
            {
                var guardar = !_ultimoPeligro.TryGetValue(pacienteId, out var ultimo)
                              || ahora - ultimo >= IntervaloPeligro;
                if (guardar)
                {
                    _ultimoPeligro[pacienteId] = ahora;
                    _ultimaClase[pacienteId] = -1;
                }
                return guardar;
            }

            var cambio = !_ultimaClase.TryGetValue(pacienteId, out var anterior) || anterior != claseIndex;
            if (cambio)
            {
                _ultimaClase[pacienteId] = claseIndex;
            }
            return cambio;
        }
    }
}
