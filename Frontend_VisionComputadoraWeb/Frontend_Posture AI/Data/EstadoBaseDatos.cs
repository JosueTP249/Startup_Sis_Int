namespace Frontedn_Posture_AI.Data
{
    /// <summary>
    /// Indica si hay una cadena de conexión configurada. Permite que la IA siga
    /// funcionando aunque la base de datos no esté disponible (por ejemplo, en
    /// local sin la variable de entorno ConnectionStrings__DefaultConnection).
    /// </summary>
    public class EstadoBaseDatos
    {
        public bool Habilitada { get; }

        public EstadoBaseDatos(bool habilitada)
        {
            Habilitada = habilitada;
        }
    }
}
