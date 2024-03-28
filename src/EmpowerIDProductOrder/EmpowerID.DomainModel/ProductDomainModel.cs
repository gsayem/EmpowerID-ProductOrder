using System.Text.Json.Serialization;

namespace EmpowerID.DomainModel
{
    public class ProductDomainModel
    {
        [JsonPropertyName("@search.score")]
        public decimal searchscore { get; set; }
        [JsonPropertyName("product_id")]
        public string Id { get; set; }
        [JsonPropertyName("product_name")]
        public string Name { get; set; }
        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; }
        [JsonPropertyName("price")]
        public string Price { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("date_added")]
        public string DateAdded { get; set; }
    }
}
