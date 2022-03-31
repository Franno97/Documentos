using System.Collections.Generic;

namespace Mre.Servicios.SharePoint.Api.Models
{
    public class ResultadoObtenerArchivoPorCodigo
    {
        public string Error { get; set; }
        public List<InformacionArchivo> Archivos { get; set; }

    }
    public class InformacionArchivo
    {
        public string Ruta { get; set; }
        public string ArchivoBase64 { get; set; }
        public string Biblioteca { get; set; }
        public string TipoDocumento { get; set; }
        public string Sufijo { get; set; }

    }
}