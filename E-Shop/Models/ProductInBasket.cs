namespace E_Shop.Models
{
    public class ProductInBasket
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string VariantString { get; set; }
        public int Size { get; set; }
        public int VariantID { get; set; }
        public string ImgURL { get; set; }
        public int Count = 1;
        public int Price { get; set; }
        public int ChildStockID { get; set; }
    
    }
}
