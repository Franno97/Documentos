namespace Mre.Servicios.SharePoint.Api.Models
{
  public class Archivo
  {
    public string nombre { get; set; }
    public string extension { get; set; }
    public byte[] documento { get; set; }
    public string tipo { get; set; }
  }
}