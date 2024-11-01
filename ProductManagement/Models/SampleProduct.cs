namespace ProductManagement.Models
{
    public class SampleProduct
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string[] Images { get; set; } = new string[] {};
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
