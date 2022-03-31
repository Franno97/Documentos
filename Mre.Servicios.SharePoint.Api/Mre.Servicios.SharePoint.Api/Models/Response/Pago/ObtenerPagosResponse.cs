using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mre.Servicios.SharePoint.Api.Models.Response.Pago
{
  public class ObtenerPagosResponse
  {
    public int httpStatusCode { get; set; }
    public Pago result { get; set; }
    public class Pago
    {
      public string idTramite { get; set; }
      public int formaPago { get; set; }
      public string solicitante { get; set; }
      public string documentoIdentificacion { get; set; }
      public string servicioId { get; set; }
      public string observacion { get; set; }
      public string estado { get; set; }
      public string banco { get; set; }
      public string numeroCuenta { get; set; }
      public string tipoCuenta { get; set; }
      public List<PagoDetalle> listaPagoDetalle { get; set; }
      public string id { get; set; }
    }
    public class PagoDetalle
    {
      public string idTramite { get; set; }
      public string idPago { get; set; }
      public int orden { get; set; }
      public string descripcion { get; set; }
      public double valorArancel { get; set; }
      public double porcentajeDescuento { get; set; }
      public double valorDescuento { get; set; }
      public double valorTotal { get; set; }
      public string estado { get; set; }
      public string ordenPago { get; set; }
      public string numeroTransaccion { get; set; }
      public bool estaFacturado { get; set; }
      public string claveAcceso { get; set; }
      public string comprobantePago { get; set; }
      public string partidaArancelariaId { get; set; }
      public string servicioId { get; set; }
      public string servicio { get; set; }
      public string partidaArancelaria { get; set; }
      public string numeroPartida { get; set; }
      public string jerarquiaArancelariaId { get; set; }
      public string jerarquiaArancelaria { get; set; }
      public Guid arancelId { get; set; }
      public string arancel { get; set; }
      public string convenioId { get; set; }
      public string tipoServicio { get; set; }
      public string tipoExoneracionId { get; set; }
      public string tipoExoneracion { get; set; }
      public string facturarEn { get; set; }
      public DateTime lastModified { get; set; }
      public string lastModifierId { get; set; }
      public DateTime created { get; set; }
      public string creatorId { get; set; }
      public Guid id { get; set; }
      public bool isDeleted { get; set; }
    }
  }
}