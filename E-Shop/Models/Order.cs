namespace E_Shop.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string Basket { get; set; }
        public string CName { get; set; }
        public string CSurname { get; set; }
        public string CEmail {get; set; }
        public string CAdress { get; set; }
        public string Payment { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderTime { get; set; }
        public string Note { get; set; }
    }
}
