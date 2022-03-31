using System;


namespace Mre.Servicios.SharePoint.Api.Models.Response.Tramite
{
    public class Movimiento
    {
        public Guid TramiteId { get; set; }


        /// <summary>
        /// Numero de Orden como ingresa el proceso
        /// </summary>
        public int Orden { get; set; }

        /// <summary>
        /// Usuario que va a gestionar el tramite
        /// Ejemplo puede ser Consultor, Funcionario, Perito, otros
        /// </summary>
        public Guid UsuarioId { get; set; }

        /// <summary>
        /// El estado del flujo de proceso
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// El nombre estado del flujo de proceso
        /// </summary>
        public string NombreEstado { get; set; }

        /// <summary>
        /// Vigente se considera al ultimo como True y los anteres al orden como False
        /// </summary>
        public bool Vigente { get; set; }

        /// <summary>
        /// Nombre del rol del usuario en ese momento
        /// </summary>
        public string NombreRol { get; set; }

        /// <summary>
        /// Unidad Administrativa de la Zonal
        /// </summary>
        public Guid UnidadAdministrativaId { get; set; }

        /// <summary>
        /// observacion del tramite
        /// </summary>
        public string ObservacionDatosPersonales { get; set; }

        /// <summary>
        /// Observaciones de beneficiarios
        /// </summary>
        public string ObservacionDomicilios { get; set; }

        /// <summary>
        /// Observaciones de soportes de gestion
        /// </summary>
        public string ObservacionSoportesGestion { get; set; }

        /// <summary>
        /// Las observaciones al tab de movimientos migratorios
        /// </summary>
        public string ObservacionMovimientoMigratorio { get; set; }

        /// <summary>
        /// La observacion del tab de multas
        /// </summary>
        public string ObservacionMultas { get; set; }

        /// <summary>
        /// Este campo solo aplica para los movimientos de ciudadano
        /// </summary>
        public int DiasTranscurridos { get; set; }

        /// <summary>
        /// Formato de fecha desde para la cita ejemplo 2022-10-10 10:00:00
        /// </summary>
        public string FechaCitaDesde { get; set; }

        /// <summary>
        /// Formato de fecha desde para la cita ejemplo 2022-10-10 10:30:00 
        /// </summary>
        public string FechaCitaHasta { get; set; }
    }
}