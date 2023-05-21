#nullable disable

using System.Text.Json.Serialization;

namespace Bytewardens.Models
{
    public class Images
    {
        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }

    public class Store
    {
        [JsonPropertyName("storeID")]
        public string StoreID { get; set; }

        [JsonPropertyName("storeName")]
        public string StoreName { get; set; }

        [JsonPropertyName("isActive")]
        public int IsActive { get; set; }

        [JsonPropertyName("images")]
        public Images Images { get; set; }
    }

}
