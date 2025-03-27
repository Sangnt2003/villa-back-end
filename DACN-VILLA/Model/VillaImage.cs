using System.Text.Json.Serialization;

namespace DACN_VILLA.Model
{
    public class VillaImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid VillaId { get; set; }
        public string ImageUrl { get; set; }
        [JsonIgnore]
        public Villa Villa { get; set; }
    }
}
