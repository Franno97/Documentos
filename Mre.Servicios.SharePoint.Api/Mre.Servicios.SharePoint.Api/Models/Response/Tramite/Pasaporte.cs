using System;


namespace Mre.Servicios.SharePoint.Api.Models.Response.Tramite
{
    public class Pasaporte
    {
        public string CiudadEmision { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaExpiracion { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string Nombres { get; set; }

        public string Numero { get; set; }

        public string PaisEmision { get; set; }
    }
}