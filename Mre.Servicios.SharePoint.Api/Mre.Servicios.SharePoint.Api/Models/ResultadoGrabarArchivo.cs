namespace Mre.Servicios.SharePoint.Api.Models
{
    /// <summary>
    /// Modelo para el resultado de grabar archivo
    /// </summary>
    public class ResultadoGrabarArchivo
    {
        /// <summary>
        /// Ruta del documento
        /// </summary>
        public string Ruta { get; set; }

        /// <summary>
        /// Ruta del archivo en el Sharepoint
        /// </summary>
        public string RutaSp { get; set; }

        /// <summary>
        /// Lista del Sharepoint
        /// </summary>
        public string ListaSp { get; set; }

        /// <summary>
        /// Sitio del Sharepoint
        /// </summary>
        public string SitioSp { get; set; }

        /// <summary>
        /// Carpeta del Sharepoint
        /// </summary>
        public string CarpetaSp { get; set; }

        /// <summary>
        /// Nombre del fichero
        /// </summary>
        public string NombreFichero { get; set; }
    }
}