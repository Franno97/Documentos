using System;
using System.Collections.Generic;


namespace Mre.Servicios.SharePoint.Api.Models.Response.Tramite
{
    public class Tramite
    {
        /// <summary>
        /// Numero del tramite se compone de la fecha en formato yyyyMMdd-contador
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Fecha del tramite
        /// </summary>
        public string Fecha { get; set; }

        /// <summary>
        /// Nombre d la actividad
        /// </summary>
        public Guid ActividadId { get; set; }

        /// <summary>
        /// Datos del Beneficiario
        /// </summary>
        public Beneficiario Beneficiario { get; set; }

        /// <summary>
        /// Id del Beneficiario
        /// </summary>
        public Guid BeneficiarioId { get; set; }

        /// <summary>
        /// Calidad migratoria
        /// </summary>
        public Guid CalidadMigratoriaId { get; set; }

        /// <summary>
        /// Los documentos adjuntos
        /// </summary>
        public List<Documento> Documentos { get; set; }

        /// <summary>
        /// Datos del grupo que pertenece
        /// </summary>
        public Guid TipoConvenioId { get; set; }

        /// <summary>
        /// Datos del solciitante
        /// </summary>
        public Solicitante Solicitante { get; set; }

        /// <summary>
        /// Id del solicitante
        /// </summary>
        public Guid SolicitanteId { get; set; }

        /// <summary>
        /// Tipo de visas
        /// </summary>
        public Guid TipoVisaId { get; set; }

        /// <summary>
        /// Unidad Administrativa entro de Emision de Visas
        /// </summary>
        public Guid UnidadAdministrativaIdCEV { get; set; }

        /// <summary>
        /// Unidad Administrativa de la Zonal
        /// </summary>
        public Guid UnidadAdministrativaIdZonal { get; set; }

        /// <summary>
        /// Los documentos adjuntos
        /// </summary>
        public List<SoporteGestion> SoporteGestiones { get; set; }

        /// <summary>
        /// Los documentos adjuntos
        /// </summary>
        public List<Movimiento> Movimientos { get; set; }

        /// <summary>
        /// Servicio Id
        /// </summary>
        public Guid ServicioId { get; set; }

        /// <summary>
        /// Codigo de Pais
        /// </summary>
        public string CodigoPais { get; set; }

        /// <summary>
        /// Persona Id
        /// </summary>
        public Guid PersonaId { get; set; }
    }
}