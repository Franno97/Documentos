using System.ComponentModel.DataAnnotations;

namespace Mre.Servicios.SharePoint.Api.Models
{
    public class SolicitudObtenerArchivoPorUrl
    {
        [Required]
        public string UrlArchivo { get; set; }
    }
}
