using System;


namespace Mre.Servicios.SharePoint.Api.Models.Response.Tramite
{
    public class Beneficiario
    {
        public string CarnetConadis { get; set; }

        public string CiudadNacimiento { get; set; }

        public string CodigoMDG { get; set; }

        public string Correo { get; set; }

        public Domicilio Domicilio { get; set; }

        public Guid DomicilioId { get; set; }

        public int Edad { get; set; }

        public string EstadoCivil { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string Foto { get; set; }

        public string Genero { get; set; }

        public string Ocupacion { get; set; }

        public string NacionalidadId { get; set; }
        public string Nacionalidad { get; set; }

        public string Nombres { get; set; }

        public string PaisNacimiento { get; set; }

        public Pasaporte Pasaporte { get; set; }

        public Guid PasaporteId { get; set; }

        public int PorcentajeDiscapacidad { get; set; }

        public bool PoseeDiscapacidad { get; set; }

        public string PrimerApellido { get; set; }

        public string SegundoApellido { get; set; }

        public Visa Visa { get; set; }

        public Guid VisaId { get; set; }
    }
}