using Newtonsoft.Json;

namespace CbcRoastersErp.Models
{
    public class OrderProduct
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price_inc_tax")]
        public decimal PriceIncTax { get; set; }

        [JsonProperty("sku")]
        public string SKU { get; set; }
    }
}
