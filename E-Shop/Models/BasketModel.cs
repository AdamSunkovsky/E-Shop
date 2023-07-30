namespace E_Shop.Models
{
    public class BasketModel
    {
        public List<ProductInBasket> Products = new List<ProductInBasket>();
        public int PriceNoDPH { get; set; }
        public int DPH { get; set; }

        public int TotalPrice { get; set; }
        public int Count { get; set; }

        public void CalculateDetails ()
        {
            foreach (var item in Products)
            {
                this.TotalPrice += item.Price;
                this.Count += item.Count;
            }
            double perc = 0.21 * this.TotalPrice;
            this.DPH = (int)perc;
            this.PriceNoDPH = this.TotalPrice - DPH;
        }
    }
}
