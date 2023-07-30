namespace E_Shop.Models
{
    public class Warehouse
    {
        //WarehouseID	ParentID	Size	Stock
        public int WarehouseId { get; set; }
        public int ParentID { get; set; } //Product Detail
        public int Size { get; set; }
        public int Stock { get; set; }
    }
}
