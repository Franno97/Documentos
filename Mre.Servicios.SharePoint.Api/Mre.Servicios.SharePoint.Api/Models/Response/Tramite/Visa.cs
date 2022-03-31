using System;


namespace Mre.Servicios.SharePoint.Api.Models.Response.Tramite
{
    public class Visa
    {
        public DateTime FechaEmision { get; set; }

        public DateTime FechaExpiracion { get; set; }

        public string Numero { get; set; }

        public bool PoseeVisa { get; set; }

        public string Tipo { get; set; }

        public bool ConfirmacionVisa { get; set; }
    }
}