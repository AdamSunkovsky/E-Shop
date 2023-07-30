namespace E_Shop.Models
{
    public class ProductDetail
    {
        public int ProductDetailID { get; set; }
        public int ParentID { get; set; }
        public string ColorCode { get; set; }
        public string ProductSizes { get; set; }
        public int ProductPrice { get; set; }
        public int DPH { get; set; }
        public int ProductSale { get; set; }
        public int StockDetail { get; set; }
    }
}
