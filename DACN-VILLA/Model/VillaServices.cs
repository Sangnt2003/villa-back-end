using System.Text.Json.Serialization;

public class VillaServices
{
    public Guid VillaId { get; set; }
    public Guid ServiceId { get; set; }
    public bool IsAvailable { get; set; }
    public decimal TotalPrice {  get; set; }
    public Villa Villa { get; set; }
    public Services Service { get; set; }
}
