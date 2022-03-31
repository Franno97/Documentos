using Microsoft.SharePoint.Client;
using Mre.Servicios.SharePoint.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;
using Mre.Servicios.SharePoint.Api.Models.Response.Pago;

namespace Mre.Servicios.SharePoint.Api.Controllers
{
  [RoutePrefix("api/sharepoint")]
  public class SharePointController : ApiController
  {
    public SharePointController()
    {
    }

    #region Métodos públicos del controlador

    /// <summary>
    /// Metodo de login que la conexion de sharepoint se enlaza
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("login")]
    public string Login()
    {
      string SiteURL = ConfigurationManager.AppSettings["server"].ToString();
      string Environmentvalue = ConfigurationManager.AppSettings["environment"].ToString();
      string username = ConfigurationManager.AppSettings["userName"].ToString();
      string domain = ConfigurationManager.AppSettings["domain"].ToString();
      string password = ConfigurationManager.AppSettings["password"].ToString();
      return AutenticacionteUsuarioBasica(new Uri(SiteURL), Environmentvalue, username, password, domain);
    }

    /// <summary>
    /// Metodo de grabar un documento mediante 2 parametros de entrada por form data
    /// codigoMDG es el codigo de entrada
    /// 1 archivo  de entrada no importa la extensión
    /// </summary>
    /// <returns>true o false</returns>
    [HttpPost]
    [Route("grabarDocumento")]
    public Resultado GrabarDocumento()
    {
      Resultado resultado;
      try
      {
        var nombre = Convert.ToString(System.Web.HttpContext.Current.Request.Form["nombre"]);
        var codigoMdg = Convert.ToString(System.Web.HttpContext.Current.Request.Form["codigoMDG"]);
        var file = System.Web.HttpContext.Current.Request.Files[0]; //archivo

        if (file == null)
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "Archivo es obligatorio", Ruta = string.Empty };
        }

        string extension = Path.GetExtension(file.FileName);

        if (nombre.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el nombre de archivo esta vacio", Ruta = string.Empty };
        }

        if (codigoMdg.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el codigo MDG esta vacio", Ruta = string.Empty };
        }

        if (extension.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el archivo no esta cargado ", Ruta = string.Empty };
        }

        ClientContext clientContext = AutenticacionteUsuario();

        var resultadoGrabar = GrabarArchivo(clientContext, "Documentos", ReadToEnd(file.InputStream), codigoMdg,
            nombre, extension);

        resultado = new Resultado
        {
          Estado = "OK",
          Mensaje = "Archivo almacenado",
          Ruta = resultadoGrabar.Ruta
        };
      }
      catch (Exception ex)
      {
        resultado = new Resultado
        {
          Estado = "ERROR",
          Mensaje = ex.Message,
          Ruta = string.Empty
        };
      }

      return resultado;
    }

    [Route("grabarSoporteGestion")]
    [HttpPost]
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public Resultado GrabarSoporteGestion()
    {
      Resultado resultado;
      try
      {
        var codigoMdg = Convert.ToString(System.Web.HttpContext.Current.Request.Form["codigoMDG"]);
        var file = System.Web.HttpContext.Current.Request.Files[0];

        if (file == null)
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "Archivo es obligatorio", Ruta = string.Empty };
        }

        string extension = Path.GetExtension(file.FileName);
        string nombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);

        if (nombreArchivo.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el nombre del archivo esta vacio", Ruta = string.Empty };
        }

        if (codigoMdg.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el codigo MDG esta vacio", Ruta = string.Empty };
        }

        if (extension.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el archivo no esta cargado", Ruta = string.Empty };
        }

        var clientContext = AutenticacionteUsuario();

        var resultadoGrabar = GrabarArchivo(clientContext, "SoporteGestiones", ReadToEnd(file.InputStream),
            codigoMdg, nombreArchivo, extension);

        resultado = new Resultado
        {
          Estado = "OK",
          Mensaje = "Archivo almacenado",
          Ruta = resultadoGrabar.Ruta,
          RutaSp = resultadoGrabar.RutaSp,
          ListaSp = resultadoGrabar.ListaSp,
          SitioSp = resultadoGrabar.SitioSp,
          CarpetaSp = resultadoGrabar.CarpetaSp,
          NombreFichero = resultadoGrabar.NombreFichero
        };
      }
      catch (Exception ex)
      {
        resultado = new Resultado
        {
          Estado = "ERROR",
          Mensaje = ex.Message,
          Ruta = string.Empty
        };
      }

      return resultado;
    }

    [Route("grabarBiblioteca")]
    [HttpPost]
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public Resultado GrabarBiblioteca()
    {
      //cedulacion
      //factura
      //Solicitud Visa
      //Concesion Visa
      Resultado resultado;
      try
      {
        var codigoMdg = Convert.ToString(System.Web.HttpContext.Current.Request.Form["codigoMDG"]);
        var biblioteca = Convert.ToString(System.Web.HttpContext.Current.Request.Form["biblioteca"]);
        var file = System.Web.HttpContext.Current.Request.Files["archivo"];

        if (file == null)
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "Archivo es obligatorio", Ruta = string.Empty };
        }

        string extension = Path.GetExtension(file.FileName);
        string nombreArchivo = Path.GetFileNameWithoutExtension(file.FileName);

        if (nombreArchivo.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el nombre del archivo esta vacio", Ruta = string.Empty };
        }
        if (codigoMdg.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el codigo MDG esta vacio", Ruta = string.Empty };
        }

        if (biblioteca.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "biblioteca esta vacio", Ruta = string.Empty };
        }

        if (extension.Equals(string.Empty))
        {
          return new Resultado
          { Estado = "ERROR", Mensaje = "el archivo no esta cargado", Ruta = string.Empty };
        }

        var clientContext = AutenticacionteUsuario();

        var resultadoGrabar = GrabarArchivo(clientContext, biblioteca, ReadToEnd(file.InputStream),
            codigoMdg, nombreArchivo, extension);

        resultado = new Resultado
        {
          Estado = "OK",
          Mensaje = "Archivo almacenado",
          Ruta = resultadoGrabar.Ruta,
          RutaSp = resultadoGrabar.RutaSp,
          ListaSp = resultadoGrabar.ListaSp,
          SitioSp = resultadoGrabar.SitioSp,
          CarpetaSp = resultadoGrabar.CarpetaSp,
          NombreFichero = resultadoGrabar.NombreFichero
        };
      }
      catch (Exception ex)
      {
        resultado = new Resultado
        {
          Estado = "ERROR",
          Mensaje = ex.Message,
          Ruta = string.Empty
        };
      }

      return resultado;
    }

    /// <summary>
    /// Obtiene el archivo en base 64 a partir de la URL
    /// </summary>
    /// <param name="solicitud"></param>
    /// <returns></returns>
    [Route("obtenerArchivoBase64PorUrl")]
    [HttpPost]
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public ResultadoObtenerArchivo ObtenerArchivoBase64PorUrl(SolicitudObtenerArchivoPorUrl solicitud)
    {
      try
      {
        var usuario = ConfigurationManager.AppSettings["userName"];
        var contrasena = ConfigurationManager.AppSettings["password"];
        using (WebClient client = new WebClient())
        {
          client.Credentials = new NetworkCredential(usuario, contrasena);
          var archivo = client.DownloadData(solicitud.UrlArchivo);

          var base64String = Convert.ToBase64String(archivo);
          var contentType = ObtenerMimeArchivoUrl(solicitud.UrlArchivo);

          var resultado = $"data:{contentType};base64," + base64String;

          return new ResultadoObtenerArchivo
          {
            ArchivoBase64 = resultado
          };
        }
      }
      catch
      {
        return null;
      }
    }

    [Route("obtenerArchivosCiudadanoPorCodigo")]
    [HttpPost]
    [EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public ResultadoObtenerArchivoPorCodigo ObtenerArchivosCiudadanoPorCodigo(string CodigoRequest)
    {
      ResultadoObtenerArchivoPorCodigo archivos = new ResultadoObtenerArchivoPorCodigo();
      archivos.Archivos = new List<InformacionArchivo>();

      try
      {
        var usuario = ConfigurationManager.AppSettings["userName"];
        var contrasena = ConfigurationManager.AppSettings["password"];
        using (WebClient client = new WebClient())
        {
          client.Credentials = new NetworkCredential(usuario, contrasena);

          var url = "http://172.31.3.27/";
          var bibliotecas = new List<(string, string)>
                    {
                        (Models.Const.Tipos.CEDU,"Cedula"),
                        (Models.Const.Tipos.APEN,"AntecedentesPenales"),
                        (Models.Const.Tipos.COND,"CarnetDiscapacidad"),
                        (Models.Const.Tipos.PASP,"Pasaporte"),
                        (Models.Const.Tipos.RCON,"RegistroConsular"),
                        ("PAGO","PagoComprobante"),
                        (Models.Const.Tipos.PNAC,"PartidaNacimiento"),
                        ("PMAT","PartidaMatrimonio"),
                        (Models.Const.Tipos.FOTO,Models.Const.Tipos.FOTO)
                    };
          var extensiones = new List<string>
                    {
                        ("pdf"),
                        ("jpg"),
                        ("png"),
                        ("jpeg")
                    };

          foreach (var item in bibliotecas)
          {
            foreach (var item1 in extensiones)
            {
              var biblioteca = item.Item2;
              var codigo = CodigoRequest;
              var sufijo = item.Item1;
              var extension = item1;
              try
              {
                var ruta = $"{url}{biblioteca}/{codigo}_{sufijo}.{extension}";
                var archivo = client.DownloadData(ruta);
                var base64 = Convert.ToBase64String(archivo);
                var resultado = new InformacionArchivo
                {
                  Ruta = ruta,
                  ArchivoBase64 = base64,
                  Biblioteca = biblioteca,
                  TipoDocumento = extension,
                  Sufijo = sufijo
                };
                archivos.Archivos.Add(resultado);
              }
              catch (Exception ex)
              {
              }



            }
          }
        }
      }
      catch (Exception ex)
      {
        archivos.Error = ex.Message;
      }
      return archivos;
    }
    [HttpPost]
    [Route("GrabarDocumentoZipAsync")]
    public Resultado GrabarDocumentoZipAsync()
    {
      Resultado resultado;
      try
      {
        var tramiteId = Convert.ToString(System.Web.HttpContext.Current.Request.Form["tramiteId"]);
        if (tramiteId == null)
        {
          return resultado = new Resultado
          {
            Estado = "ERROR",
            Mensaje = "falta ingresar el tramiteId"
          };
        }
        var lstArchivo = new List<Archivo>();
        lstArchivo = ObtenerArhivos(System.Web.HttpContext.Current);
        if (lstArchivo.Count == 0)
        {
          return resultado = new Resultado
          {
            Estado = "ERROR",
            Mensaje = "Los archivos cargados no tiene el formato definido en su nombre"
          };
        }
        if (!ValidarTipoNombre(lstArchivo))
        {
          return resultado = new Resultado
          {
            Estado = "ERROR",
            Mensaje = "Los archivos cargados no tiene el tipo definido en su nombre"
          };
        }


        var tramite = new Models.Response.Tramite.TramiteResponse();
        var pago = new ObtenerPagosResponse.Pago();
        var pagoDetalle = new List<ObtenerPagosResponse.PagoDetalle>();

        #region Consultar Tramite

        //string placesJson = string.Empty;
        var client = new HttpClient();
        var uri = new Uri(ConfigurationManager.AppSettings["serverTramites"]) + "api/Tramite/ConsultarTramitePorId";
        Models.Request.Tramite tramiteRequest = new Models.Request.Tramite { Id = Guid.Parse(tramiteId) };
        var data = JsonConvert.SerializeObject(tramiteRequest);
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        var response = client.PostAsync(uri, content).Result;
        if (response.StatusCode == HttpStatusCode.OK)
        {
          tramite = JsonConvert.DeserializeObject<Models.Response.Tramite.TramiteResponse>(response.Content.ReadAsStringAsync().Result);
        }
        else
        {
          return resultado = new Resultado
          {
            Estado = "ERROR",
            Mensaje = "No existe tramite"
          };
        }
        #endregion

        #region Obtener Pago

        HttpClient Client = new HttpClient();

        var Uri = new Uri(ConfigurationManager.AppSettings["serverPago"]) + "api/Pago/ObtenerPago?idTramite=" + tramiteId + "&valoresMayoraCero=false&facturarEn=0";
        var Response = Client.PostAsync(Uri, null).Result;
        if (Response.StatusCode == HttpStatusCode.OK)
        {
          var PlacesJson = Response.Content.ReadAsStringAsync().Result;
          var pagoObtenerPagoResponse = JsonConvert.DeserializeObject<ObtenerPagosResponse>(PlacesJson);
          pago = pagoObtenerPagoResponse.result;
          pagoDetalle = pagoObtenerPagoResponse.result.listaPagoDetalle;
        }
        
        #endregion

        //Vamos a consultar el tramite con sus datos

        ////inicio xml
        XmlDocument doc = new XmlDocument();
        doc.LoadXml("<Datos_Documentos>" +
                    "  <Id_ciudadano>" + tramite.result.Beneficiario.CodigoMDG + "</Id_ciudadano>" +
                    "  <Id_tramite>" + tramiteId + "</Id_tramite>" +
                    "</Datos_Documentos>");

        XmlNode root = doc.DocumentElement;


        #region PASAPORTE

        XmlNode nodoPasp = doc.CreateNode("element", Models.Const.Tipos.PASP, "");
        XmlNode elementNumeroPasaporte = doc.CreateNode("element", "Num_Pasaporte", "");
        XmlNode elementNombre = doc.CreateNode("element", "nombre", "");
        XmlNode elementFechaCaducidad = doc.CreateNode("element", "fec_caducidad_PASP", "");
        XmlNode elementGuidpasp = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.PASP)) != null)
        {
          elementNumeroPasaporte.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          elementNombre.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                    tramite.result.Beneficiario.PrimerApellido + " " +
                                    tramite.result.Beneficiario.SegundoApellido;
          elementFechaCaducidad.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          elementGuidpasp.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.PASP)).Id
              .ToString();
        }
        else
        {
          elementNumeroPasaporte.InnerText = " ";
          elementNombre.InnerText = " ";
          elementFechaCaducidad.InnerText = " ";
          elementGuidpasp.InnerText = Guid.Empty.ToString();
        }

        nodoPasp.AppendChild(elementNumeroPasaporte);
        root.AppendChild(nodoPasp);

        nodoPasp.AppendChild(elementNombre);
        root.AppendChild(nodoPasp);

        nodoPasp.AppendChild(elementFechaCaducidad);
        root.AppendChild(nodoPasp);

        nodoPasp.AppendChild(elementGuidpasp);
        root.AppendChild(nodoPasp);

        #endregion PASAPORTE

        #region CEDULA

        XmlNode nodoCedu = doc.CreateNode("element", Models.Const.Tipos.CEDU, "");
        XmlNode elementNumeroCedula = doc.CreateNode("element", "Num_Cedula", "");
        XmlNode elementNombreCedula = doc.CreateNode("element", "Nombre", "");
        XmlNode elementFechaCaducidadCedu = doc.CreateNode("element", "fec_caducidad_CEDU", "");
        XmlNode elementGuidcedu = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.CEDU)) != null)
        {
          elementNumeroCedula.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          elementNombreCedula.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                          tramite.result.Beneficiario.PrimerApellido + " " +
                                          tramite.result.Beneficiario.SegundoApellido;
          elementFechaCaducidadCedu.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          elementGuidcedu.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.CEDU)).Id
              .ToString();
        }
        else
        {
          elementNumeroCedula.InnerText = " ";
          elementNombreCedula.InnerText = " ";
          elementFechaCaducidadCedu.InnerText = " ";
          elementGuidcedu.InnerText = Guid.Empty.ToString();
        }

        nodoCedu.AppendChild(elementNumeroCedula);
        root.AppendChild(nodoCedu);

        nodoCedu.AppendChild(elementNombreCedula);
        root.AppendChild(nodoCedu);

        nodoCedu.AppendChild(elementFechaCaducidadCedu);
        root.AppendChild(nodoCedu);

        nodoCedu.AppendChild(elementGuidcedu);
        root.AppendChild(nodoCedu);

        #endregion CEDULA

        #region RECONOCIMIENTO FACIAL

        XmlNode nodoRcon = doc.CreateNode("element", Models.Const.Tipos.RCON, "");
        XmlNode eleRconNumeroPasaporte = doc.CreateNode("element", "Num_Pasaporte", "");
        XmlNode eleRconNombre = doc.CreateNode("element", "Nombre", "");
        XmlNode eleRconFechaCaducidad = doc.CreateNode("element", "fec_caducidad_RCON", "");
        XmlNode eleRconguid = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.RCON)) != null)
        {
          eleRconNumeroPasaporte.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          eleRconNombre.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                    tramite.result.Beneficiario.PrimerApellido + " " +
                                    tramite.result.Beneficiario.SegundoApellido;
          eleRconFechaCaducidad.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          eleRconguid.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.RCON)).Id
              .ToString();
        }
        else
        {
          eleRconNumeroPasaporte.InnerText = " ";
          eleRconNombre.InnerText = " ";
          eleRconFechaCaducidad.InnerText = " ";
          eleRconguid.InnerText = Guid.Empty.ToString();
        }

        nodoRcon.AppendChild(eleRconNumeroPasaporte);
        root.AppendChild(nodoRcon);

        nodoRcon.AppendChild(eleRconNombre);
        root.AppendChild(nodoRcon);

        nodoRcon.AppendChild(eleRconFechaCaducidad);
        root.AppendChild(nodoRcon);

        nodoRcon.AppendChild(eleRconguid);
        root.AppendChild(nodoRcon);

        #endregion RECONOCIMIENTO FACIAL

        #region ANTECEDENTES PENALES

        XmlNode nodoApen = doc.CreateNode("element", Models.Const.Tipos.APEN, "");
        XmlNode eleApenFechaCaducidad = doc.CreateNode("element", "fec_caducidad_RCON", "");
        XmlNode eleApenApostillado = doc.CreateNode("element", "num_reg_apostillado", "");
        XmlNode eleApenNombre = doc.CreateNode("element", "nombre", "");
        XmlNode eleApenLugar = doc.CreateNode("element", "lugar", "");
        XmlNode eleApenguid = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.APEN)) != null)
        {
          eleApenFechaCaducidad.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          eleApenApostillado.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                         tramite.result.Beneficiario.PrimerApellido + " " +
                                         tramite.result.Beneficiario.SegundoApellido;
          eleApenNombre.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          eleApenLugar.InnerText = tramite.result.Beneficiario.Pasaporte.CiudadEmision;
          eleApenguid.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.APEN)).Id
              .ToString();
        }
        else
        {
          eleApenFechaCaducidad.InnerText = " ";
          eleApenApostillado.InnerText = " ";
          eleApenNombre.InnerText = " ";
          eleApenLugar.InnerText = " ";
          eleApenguid.InnerText = Guid.Empty.ToString();
        }

        nodoApen.AppendChild(eleApenFechaCaducidad);
        root.AppendChild(nodoApen);

        nodoApen.AppendChild(eleApenApostillado);
        root.AppendChild(nodoApen);

        nodoApen.AppendChild(eleApenNombre);
        root.AppendChild(nodoApen);

        nodoApen.AppendChild(eleApenLugar);
        root.AppendChild(nodoApen);

        nodoApen.AppendChild(eleApenguid);
        root.AppendChild(nodoApen);

        #endregion ANTECEDENTES PENALES

        #region PAGO 1

        XmlNode nodoPago1 = doc.CreateNode("element", Models.Const.Tipos.PAGO1, "");
        XmlNode elemTipoPago1 = doc.CreateNode("element", "tipo_PAGO", "");
        XmlNode elemPagoLugar1 = doc.CreateNode("element", "lugar_PAGO", "");
        XmlNode elemPagoFechaEmision1 = doc.CreateNode("element", "fec_emision_PAGO", "");
        XmlNode elemPagoNumTransaccion1 = doc.CreateNode("element", "num_transaccion", "");
        XmlNode elemPagoNumCuenta1 = doc.CreateNode("element", "num_cuenta", "");
        XmlNode elemPagoMonto1 = doc.CreateNode("element", "monto", "");
        XmlNode elemPagoguid1 = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.PAGO1)) != null)
        {
          if (pagoDetalle.Count == 0)
          {
            return resultado = new Resultado
            {
              Estado = "ERROR",
              Mensaje = "No existe datos de pago para el tramite"
            };
          }
          elemTipoPago1.InnerText = "Visa";
          elemPagoLugar1.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          elemPagoFechaEmision1.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                           tramite.result.Beneficiario.PrimerApellido + " " +
                                           tramite.result.Beneficiario.SegundoApellido;
          elemPagoNumTransaccion1.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          elemPagoNumCuenta1.InnerText = pago.numeroCuenta;
          elemPagoMonto1.InnerText = pagoDetalle[0].valorArancel.ToString();
          elemPagoguid1.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.PAGO1)).Id
              .ToString();
        }
        else
        {
          elemTipoPago1.InnerText = " ";
          elemPagoLugar1.InnerText = " ";
          elemPagoFechaEmision1.InnerText = " ";
          elemPagoNumTransaccion1.InnerText = " ";
          elemPagoNumCuenta1.InnerText = " ";
          elemPagoMonto1.InnerText = " ";
          elemPagoguid1.InnerText = Guid.Empty.ToString();
        }

        nodoPago1.AppendChild(elemTipoPago1);
        root.AppendChild(nodoPago1);

        nodoPago1.AppendChild(elemPagoLugar1);
        root.AppendChild(nodoPago1);

        nodoPago1.AppendChild(elemPagoFechaEmision1);
        root.AppendChild(nodoPago1);

        nodoPago1.AppendChild(elemPagoNumTransaccion1);
        root.AppendChild(nodoPago1);

        nodoPago1.AppendChild(elemPagoNumCuenta1);
        root.AppendChild(nodoPago1);

        nodoPago1.AppendChild(elemPagoMonto1);
        root.AppendChild(nodoPago1);

        nodoPago1.AppendChild(elemPagoguid1);
        root.AppendChild(nodoPago1);

        #endregion PAGO 1

        #region PAGO 2

        XmlNode nodoPago2 = doc.CreateNode("element", Models.Const.Tipos.PAGO2, "");
        XmlNode elemTipoPago2 = doc.CreateNode("element", "tipo_PAGO", "");
        XmlNode elemPagoLugar2 = doc.CreateNode("element", "lugar_PAGO", "");
        XmlNode elemPagoFechaEmision2 = doc.CreateNode("element", "fec_emision_PAGO", "");
        XmlNode elemPagoNumTransaccion2 = doc.CreateNode("element", "num_transaccion", "");
        XmlNode elemPagoNumCuenta2 = doc.CreateNode("element", "num_cuenta", "");
        XmlNode elemPagoMonto2 = doc.CreateNode("element", "monto", "");
        XmlNode elemPagoguid2 = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.PAGO2)) != null)
        {
          if (pagoDetalle.Count == 0)
          {
            return resultado = new Resultado
            {
              Estado = "ERROR",
              Mensaje = "No existe datos de pago para el tramite"
            };
          }
          elemTipoPago2.InnerText = "Cedula";
          elemPagoLugar2.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          elemPagoFechaEmision2.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                           tramite.result.Beneficiario.PrimerApellido + " " +
                                           tramite.result.Beneficiario.SegundoApellido;
          elemPagoNumTransaccion2.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          elemPagoNumCuenta2.InnerText = pago.numeroCuenta;
          elemPagoMonto2.InnerText = pagoDetalle[1].valorArancel.ToString();
          elemPagoguid2.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.PAGO2)).Id
              .ToString();
        }
        else
        {
          elemTipoPago2.InnerText = " ";
          elemPagoLugar2.InnerText = " ";
          elemPagoFechaEmision2.InnerText = " ";
          elemPagoNumTransaccion2.InnerText = " ";
          elemPagoNumCuenta2.InnerText = " ";
          elemPagoMonto2.InnerText = " ";
          elemPagoguid2.InnerText = Guid.Empty.ToString();
        }

        nodoPago2.AppendChild(elemTipoPago2);
        root.AppendChild(nodoPago2);

        nodoPago2.AppendChild(elemPagoLugar2);
        root.AppendChild(nodoPago2);

        nodoPago2.AppendChild(elemPagoFechaEmision2);
        root.AppendChild(nodoPago2);

        nodoPago2.AppendChild(elemPagoNumTransaccion2);
        root.AppendChild(nodoPago2);

        nodoPago2.AppendChild(elemPagoNumCuenta2);
        root.AppendChild(nodoPago2);

        nodoPago2.AppendChild(elemPagoMonto2);
        root.AppendChild(nodoPago2);

        nodoPago2.AppendChild(elemPagoguid2);
        root.AppendChild(nodoPago2);

        #endregion PAGO 2

        #region PAGO 3

        XmlNode nodoPago3 = doc.CreateNode("element", Models.Const.Tipos.PAGO3, "");
        XmlNode elemTipoPago3 = doc.CreateNode("element", "tipo_PAGO", "");
        XmlNode elemPagoLugar3 = doc.CreateNode("element", "lugar_PAGO", "");
        XmlNode elemPagoFechaEmision3 = doc.CreateNode("element", "fec_emision_PAGO", "");
        XmlNode elemPagoNumTransaccion3 = doc.CreateNode("element", "num_transaccion", "");
        XmlNode elemPagoNumCuenta3 = doc.CreateNode("element", "num_cuenta", "");
        XmlNode elemPagoMonto3 = doc.CreateNode("element", "monto", "");
        XmlNode elemPagoguid3 = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.PAGO3)) != null)
        {
          elemTipoPago3.InnerText = "Adicional1";
          elemPagoLugar3.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          elemPagoFechaEmision3.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                           tramite.result.Beneficiario.PrimerApellido + " " +
                                           tramite.result.Beneficiario.SegundoApellido;
          elemPagoNumTransaccion3.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          elemPagoNumCuenta3.InnerText = " ";
          elemPagoMonto3.InnerText = " ";
          elemPagoguid3.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.PAGO3)).Id
              .ToString();
        }
        else
        {
          elemTipoPago3.InnerText = " ";
          elemPagoLugar3.InnerText = " ";
          elemPagoFechaEmision3.InnerText = " ";
          elemPagoNumTransaccion3.InnerText = " ";
          elemPagoNumCuenta3.InnerText = " ";
          elemPagoMonto3.InnerText = " ";
          elemPagoguid3.InnerText = Guid.Empty.ToString();
        }

        nodoPago3.AppendChild(elemTipoPago3);
        root.AppendChild(nodoPago3);

        nodoPago3.AppendChild(elemPagoLugar3);
        root.AppendChild(nodoPago3);

        nodoPago3.AppendChild(elemPagoFechaEmision3);
        root.AppendChild(nodoPago3);

        nodoPago3.AppendChild(elemPagoNumTransaccion3);
        root.AppendChild(nodoPago3);

        nodoPago3.AppendChild(elemPagoNumCuenta3);
        root.AppendChild(nodoPago3);

        nodoPago3.AppendChild(elemPagoMonto3);
        root.AppendChild(nodoPago3);

        nodoPago3.AppendChild(elemPagoguid3);
        root.AppendChild(nodoPago3);

        #endregion PAGO 3

        #region PAGO 4

        XmlNode nodoPago4 = doc.CreateNode("element", Models.Const.Tipos.PAGO4, "");
        XmlNode elemTipoPago4 = doc.CreateNode("element", "tipo_PAGO", "");
        XmlNode elemPagoLugar4 = doc.CreateNode("element", "lugar_PAGO", "");
        XmlNode elemPagoFechaEmision4 = doc.CreateNode("element", "fec_emision_PAGO", "");
        XmlNode elemPagoNumTransaccion4 = doc.CreateNode("element", "num_transaccion", "");
        XmlNode elemPagoNumCuenta4 = doc.CreateNode("element", "num_cuenta", "");
        XmlNode elemPagoMonto4 = doc.CreateNode("element", "monto", "");
        XmlNode elemPagoguid4 = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.PAGO4)) != null)
        {
          elemTipoPago4.InnerText = "Adicional2";
          elemPagoLugar4.InnerText = tramite.result.Beneficiario.Pasaporte.Numero;
          elemPagoFechaEmision4.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                           tramite.result.Beneficiario.PrimerApellido + " " +
                                           tramite.result.Beneficiario.SegundoApellido;
          elemPagoNumTransaccion4.InnerText =
              tramite.result.Beneficiario.Pasaporte.FechaExpiracion.ToString("dd/MM/yyyy");
          elemPagoNumCuenta4.InnerText = " ";
          elemPagoMonto4.InnerText = " ";
          elemPagoguid4.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.PAGO4)).Id
              .ToString();
        }
        else
        {
          elemTipoPago4.InnerText = " ";
          elemPagoLugar4.InnerText = " ";
          elemPagoFechaEmision4.InnerText = " ";
          elemPagoNumTransaccion4.InnerText = " ";
          elemPagoNumCuenta4.InnerText = " ";
          elemPagoMonto4.InnerText = " ";
          elemPagoguid4.InnerText = Guid.Empty.ToString();
        }

        nodoPago4.AppendChild(elemTipoPago4);
        root.AppendChild(nodoPago4);

        nodoPago4.AppendChild(elemPagoLugar4);
        root.AppendChild(nodoPago4);

        nodoPago4.AppendChild(elemPagoFechaEmision4);
        root.AppendChild(nodoPago4);

        nodoPago4.AppendChild(elemPagoNumTransaccion4);
        root.AppendChild(nodoPago4);

        nodoPago4.AppendChild(elemPagoNumCuenta4);
        root.AppendChild(nodoPago4);

        nodoPago4.AppendChild(elemPagoMonto4);
        root.AppendChild(nodoPago4);

        nodoPago4.AppendChild(elemPagoguid4);
        root.AppendChild(nodoPago4);

        #endregion PAGO 4

        #region PARTIDA DE NACIMIENTO

        XmlNode nodoPnac = doc.CreateNode("element", Models.Const.Tipos.PNAC, "");
        XmlNode elePnacNombre = doc.CreateNode("element", "nombre", "");
        XmlNode elePnacFechaNacimiento = doc.CreateNode("element", "fec_nacimiento", "");
        XmlNode elePnacPais = doc.CreateNode("element", "pais", "");
        XmlNode elePnacNombrePadre = doc.CreateNode("element", "nombre_Padre", "");
        XmlNode elePnacNombreMadre = doc.CreateNode("element", "nombre_Madre", "");
        XmlNode elePnacguid = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.PNAC)) != null)
        {
          elePnacNombre.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                    tramite.result.Beneficiario.PrimerApellido + " " +
                                    tramite.result.Beneficiario.SegundoApellido;
          elePnacFechaNacimiento.InnerText =
              tramite.result.Beneficiario.FechaNacimiento.ToString("dd/MM/yyyy");
          elePnacPais.InnerText = tramite.result.Beneficiario.Domicilio.Pais;
          elePnacNombrePadre.InnerText = " "; //pendiente
          elePnacNombreMadre.InnerText = " "; //pendiente
          elePnacguid.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.PNAC)).Id
              .ToString();
        }
        else
        {
          elePnacNombre.InnerText = " ";
          elePnacFechaNacimiento.InnerText = " ";
          elePnacPais.InnerText = " ";
          elePnacNombrePadre.InnerText = " ";
          elePnacNombreMadre.InnerText = " ";
          elePnacguid.InnerText = Guid.Empty.ToString();
        }

        nodoPnac.AppendChild(elePnacNombre);
        root.AppendChild(nodoPnac);

        nodoPnac.AppendChild(elePnacFechaNacimiento);
        root.AppendChild(nodoPnac);

        nodoPnac.AppendChild(elePnacPais);
        root.AppendChild(nodoPnac);

        nodoPnac.AppendChild(elePnacNombrePadre);
        root.AppendChild(nodoPnac);

        nodoPnac.AppendChild(elePnacNombreMadre);
        root.AppendChild(nodoPnac);

        nodoPnac.AppendChild(elePnacguid);
        root.AppendChild(nodoPnac);

        #endregion PARTIDA DE NACIMIENTO

        #region CONADIS

        XmlNode nodoCond = doc.CreateNode("element", Models.Const.Tipos.COND, "");
        XmlNode eleCondNombre = doc.CreateNode("element", "nombre", "");
        XmlNode eleCondFechaCaducidad = doc.CreateNode("element", "fec_caducidad_COND", "");
        XmlNode eleCondFechaEmision = doc.CreateNode("element", "fec_emision_COND", "");
        XmlNode eleCondNumeroCarnet = doc.CreateNode("element", "num_carnet", "");
        XmlNode eleCondguid = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.COND)) != null)
        {
          eleCondNombre.InnerText = tramite.result.Beneficiario.Nombres + " " +
                                    tramite.result.Beneficiario.PrimerApellido + " " +
                                    tramite.result.Beneficiario.SegundoApellido;
          eleCondFechaCaducidad.InnerText = " ";
          eleCondFechaEmision.InnerText = " ";
          eleCondNumeroCarnet.InnerText = tramite.result.Beneficiario.CarnetConadis; //pendiente
          eleCondguid.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.COND)).Id
              .ToString();
        }
        else
        {
          eleCondNombre.InnerText = " ";
          eleCondFechaCaducidad.InnerText = " ";
          eleCondFechaEmision.InnerText = " ";
          eleCondNumeroCarnet.InnerText = " ";
          eleCondguid.InnerText = Guid.Empty.ToString();
        }

        nodoCond.AppendChild(eleCondNombre);
        root.AppendChild(nodoCond);

        nodoCond.AppendChild(eleCondFechaCaducidad);
        root.AppendChild(nodoCond);

        nodoCond.AppendChild(eleCondFechaEmision);
        root.AppendChild(nodoCond);

        nodoCond.AppendChild(eleCondNumeroCarnet);
        root.AppendChild(nodoCond);

        nodoCond.AppendChild(eleCondguid);
        root.AppendChild(nodoCond);

        #endregion CONADIS

        #region FOTO

        XmlNode nodoFoto = doc.CreateNode("element", Models.Const.Tipos.FOTO, "");
        XmlNode elementFotoGuid = doc.CreateNode("element", "GUIDDOC", "");

        if (lstArchivo.FirstOrDefault(x => x.tipo.Equals(Models.Const.Tipos.FOTO)) != null)
        {
          elementFotoGuid.InnerText = tramite.result.Documentos.First(x => x.TipoDocumento.Equals(Models.Const.Tipos.FOTO)).Id
              .ToString();
        }
        else
        {
          elementFotoGuid.InnerText = Guid.Empty.ToString();
        }

        nodoFoto.AppendChild(elementFotoGuid);
        root.AppendChild(nodoFoto);

        #endregion FOTO

        //con carpeta compartida
        string archivo = Path.Combine(ConfigurationManager.AppSettings["pathZip"], tramite.result.Beneficiario.CodigoMDG + ".zip");
        //con unidad compartida
        using (var outStream = new MemoryStream())
        {
          using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
          {
            //xml
            byte[] bytesXml = Encoding.Default.GetBytes(doc.OuterXml);
            string fileNameXml = tramite.result.Beneficiario.CodigoMDG + ".xml";
            var fileInArchiveXml = archive.CreateEntry(fileNameXml, CompressionLevel.Optimal);
            using (var entryStream = fileInArchiveXml.Open())
            using (var fileToCompressStream = new MemoryStream(bytesXml))
            {
              fileToCompressStream.CopyTo(entryStream);
            }

            //archivos
            foreach (var datos in lstArchivo)
            {
              byte[] fileBytes = datos.documento;
              string fileName = datos.nombre;
              var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
              using (var entryStream = fileInArchive.Open())
              using (var fileToCompressStream = new MemoryStream(fileBytes))
              {
                fileToCompressStream.CopyTo(entryStream);
              }
            }
          }

          using (var fileStream = new FileStream(archivo, FileMode.Create))
          {
            outStream.Seek(0, SeekOrigin.Begin);
            outStream.CopyTo(fileStream);
          }

          //temporal
          try
          {
            var temporalArchivo = Path.Combine(ConfigurationManager.AppSettings["pathZip"] + "Temporal", tramite.result.Beneficiario.CodigoMDG + ".zip");
            using (var fileStream = new FileStream(temporalArchivo, FileMode.Create))
            {
              outStream.Seek(0, SeekOrigin.Begin);
              outStream.CopyTo(fileStream);
            }
          }
          catch
          {

          }
        }

        resultado = new Resultado
        {
          Estado = "OK",
          Mensaje = "Archivo almacenado",
          Ruta = archivo
        };

      }
      catch (Exception ex)
      {
        resultado = new Resultado
        {
          Estado = "ERROR",
          Mensaje = ex.Message,
          Ruta = string.Empty
        };
      }

      return resultado;
    }
    #endregion Métodos públicos del controlador

    #region Metodos privados

    private bool ValidarTipoNombre(List<Archivo> archivos)
    {
      bool resultado = true;
      foreach (var item in archivos)
      {
        switch (item.tipo)
        {
          case Models.Const.Tipos.PASP:
          case Models.Const.Tipos.CEDU:
          case Models.Const.Tipos.RCON:
          case Models.Const.Tipos.APEN:
          case Models.Const.Tipos.PAGO1:
          case Models.Const.Tipos.PAGO2:
          case Models.Const.Tipos.PAGO3:
          case Models.Const.Tipos.PAGO4:
          case Models.Const.Tipos.PNAC:
          case Models.Const.Tipos.COND:
          case Models.Const.Tipos.FOTO:
            resultado = true;
            break;
          default:
            return false;
        }

      }
      return resultado;
    }
    private List<Archivo> ObtenerArhivos(System.Web.HttpContext files)
    {
      var lista = new List<Archivo>();
      int contador = files.Request.Files.Count;
      try
      {
        for (int i = 0; i < contador; i++)
        {
          var doc3 = System.Web.HttpContext.Current.Request.Files[i]; //archivo
          string[] filtro1 = Path.GetFileName(doc3.FileName).Split('_');
          string[] filtro2 = filtro1[1].ToString().Split('.');
          var tipo = filtro2[0];

          lista.Add(new Archivo
          {
            nombre = Path.GetFileName(doc3.FileName),
            extension = Path.GetExtension(doc3.FileName),
            documento = ReadToEnd(doc3.InputStream),
            tipo = tipo
          });
        }
      }
      catch
      {
        lista = new List<Archivo>();
      }
      return lista;
    }


    /// <summary>
    /// Obtiene el MIME del archivo en la URl, el archivo se encuentra al final
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private string ObtenerMimeArchivoUrl(string url)
    {
      var arreglo = url.Split('/');
      var archivo = arreglo[arreglo.Length - 1];
      var contentType = MimeMapping.GetMimeMapping(archivo);
      return contentType;
    }

    /// <summary>
    /// Metodo de Autenticación del usuario
    /// </summary>
    /// <returns></returns>
    private static ClientContext AutenticacionteUsuario()
    {
      var targetSiteUrl = new Uri(ConfigurationManager.AppSettings["server"]);
      var environmentvalue = ConfigurationManager.AppSettings["environment"];
      var username = ConfigurationManager.AppSettings["userName"];
      var password = ConfigurationManager.AppSettings["password"];
      var domain = ConfigurationManager.AppSettings["domain"];

      var context = new ClientContext(targetSiteUrl);
      // Based on the environmentvalue provided it execute the function.
      if (string.Compare(environmentvalue, "onpremises", StringComparison.OrdinalIgnoreCase) == 0)
      {
        context = IniciarSesion(username, password, targetSiteUrl, domain);
      }

      return context;
    }

    /// <summary>
    /// Metodo que nos permite validar si la conexion de login con sharepoint es estable
    /// </summary>
    /// <param name="targetSiteUrl"></param>
    /// <param name="environmentvalue"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    private static string AutenticacionteUsuarioBasica(Uri targetSiteUrl, string environmentvalue, string username,
        string password, string domain)
    {
      try
      {
        // Based on the environmentvalue provided it execute the function.
        if (string.Compare(environmentvalue, "onpremises", StringComparison.OrdinalIgnoreCase) == 0)
        {
          IniciarSesion(username, password, targetSiteUrl, domain);
          return "Autenticacion exitosa";
        }

        return "Solo se permite onpremise";
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    /// <summary>
    /// Metodo para grabar
    /// </summary>
    /// <param name="context"></param>
    /// <param name="titulo"></param>
    /// <param name="docu"></param>
    /// <param name="carpetaMdg"></param>
    /// <param name="archivo"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    private static ResultadoGrabarArchivo GrabarArchivo(ClientContext context, string titulo, byte[] docu,
        string carpetaMdg, string archivo, string extension)
    {
      var nombreFichero = archivo + extension;

      var fileCreationInfo = new FileCreationInformation
      {
        Content = docu,
        Overwrite = true,
        Url = nombreFichero
      };
      var resultado = new ResultadoGrabarArchivo();
      if (titulo.Equals("Documentos") || titulo.Equals("SoporteGestiones"))
      {
        var list = context.Web.Lists.GetByTitle(titulo);
        list.EnableFolderCreation = true;
        var carpetaMdgCrear = "MDG-" + carpetaMdg;
        var result = list.RootFolder.Folders.Add(carpetaMdgCrear);
        var file = result.Files.Add(fileCreationInfo);

        context.Load(list, l => l.DefaultDisplayFormUrl);
        context.Load(file.ListItemAllFields, item => item.Id);
        context.Load(context.Site, s => s.Url);
        context.Load(file);
        context.ExecuteQuery();

        //metadata
        var fileItem = result.Files[0];
        ListItem newItem = fileItem.ListItemAllFields;
        newItem["IdentificadorCiudadano"] = carpetaMdg;
        newItem.Update();
        context.ExecuteQuery();

        //rutaSp = String.Format("{0}{1}?ID={2}", context.Site.Url, list.DefaultDisplayFormUrl, file.ListItemAllFields.Id);
        var rutaSp = $"{context.Site.Url}{list.DefaultDisplayFormUrl}?ID={file.ListItemAllFields.Id}";
        // var ruta = Path.Combine(context.Site.Url, titulo, carpetaMdgCrear, (archivo + extension));
        var ruta = $"{context.Site.Url}/{titulo}/{carpetaMdgCrear}/{nombreFichero}";

        resultado = new ResultadoGrabarArchivo
        {
          Ruta = ruta,
          RutaSp = rutaSp,
          ListaSp = titulo,
          SitioSp = context.Site.Url,
          CarpetaSp = carpetaMdgCrear,
          NombreFichero = nombreFichero
        };
      }
      else
      {
        var list = context.Web.Lists.GetByTitle(titulo);
        //list.EnableFolderCreation = true;
        //var carpetaMdgCrear = "MDG-" + carpetaMdg;
        var file = list.RootFolder.Files.Add(fileCreationInfo);

        context.Load(list, l => l.DefaultDisplayFormUrl);
        context.Load(file.ListItemAllFields, item => item.Id);
        context.Load(context.Site, s => s.Url);
        context.Load(file);
        context.ExecuteQuery();

        //metadata
        var fileItem = list.RootFolder.Files[0];
        ListItem newItem = fileItem.ListItemAllFields;
        newItem["IdentificadorCiudadano"] = carpetaMdg;
        newItem.Update();
        context.ExecuteQuery();

        //rutaSp = String.Format("{0}{1}?ID={2}", context.Site.Url, list.DefaultDisplayFormUrl, file.ListItemAllFields.Id);
        var rutaSp = $"{context.Site.Url}{list.DefaultDisplayFormUrl}?ID={file.ListItemAllFields.Id}";
        // var ruta = Path.Combine(context.Site.Url, titulo, carpetaMdgCrear, (archivo + extension));
        var ruta = $"{context.Site.Url}/{titulo}/{nombreFichero}";

        resultado = new ResultadoGrabarArchivo
        {
          Ruta = ruta,
          RutaSp = rutaSp,
          ListaSp = titulo,
          SitioSp = context.Site.Url,
          CarpetaSp = string.Empty,
          NombreFichero = nombreFichero
        };
      }

      return resultado;
    }

    /// <summary>
    /// Metodo que realiza la accion de login con sharepoint
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="url"></param>
    /// <param name="domain"></param>
    /// <returns></returns>
    private static ClientContext IniciarSesion(string userName, string password, Uri url, string domain)
    {
      ClientContext clientContext = null;
      ClientContext ctx;
      try
      {
        clientContext = new ClientContext(url);

        // Condition to check whether the user name is null or empty.
        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
          SecureString securestring = new SecureString();
          password.ToCharArray().ToList().ForEach(s => securestring.AppendChar(s));
          clientContext.Credentials = new NetworkCredential(userName, securestring, domain);
          clientContext.Load(clientContext.Web);
          clientContext.ExecuteQuery();
          // var titulo = clientContext.Web.Title;
        }
        else
        {
          clientContext.Credentials = CredentialCache.DefaultNetworkCredentials;
          clientContext.ExecuteQuery();
        }

        ctx = clientContext;
      }
      finally
      {
        if (clientContext != null)
        {
          clientContext.Dispose();
        }
      }

      return ctx;
    }

    /// <summary>
    /// Metodo que nos permite transforma el stream a bytes[]
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static byte[] ReadToEnd(Stream stream)
    {
      long originalPosition = 0;

      if (stream.CanSeek)
      {
        originalPosition = stream.Position;
        stream.Position = 0;
      }

      try
      {
        byte[] readBuffer = new byte[4096];

        int totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
        {
          totalBytesRead += bytesRead;

          if (totalBytesRead == readBuffer.Length)
          {
            int nextByte = stream.ReadByte();
            if (nextByte != -1)
            {
              byte[] temp = new byte[readBuffer.Length * 2];
              Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
              Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
              readBuffer = temp;
              totalBytesRead++;
            }
          }
        }

        byte[] buffer = readBuffer;
        if (readBuffer.Length != totalBytesRead)
        {
          buffer = new byte[totalBytesRead];
          Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
        }

        return buffer;
      }
      finally
      {
        if (stream.CanSeek)
        {
          stream.Position = originalPosition;
        }
      }
    }


    #endregion


  }
}