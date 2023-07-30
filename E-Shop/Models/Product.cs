namespace E_Shop.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public int Categories { get; set; }
        public string ShoeName { get; set; }
        public string? Manufacturer { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? ShoeMaterial { get; set; }
        public string? ProductCode { get; set; }
        public int DefaultPrice { get; set; }
        public int LowestPrice { get; set; }
    }
}
