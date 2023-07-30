using E_Shop.Models;

namespace E_Shop
{
    public class BetterViewGenerator
    {
        private MyContext Datas = new MyContext();
        
        public StockModel_BetterView GetBetterView(int WarehouseID)
        {
          
            Warehouse ParentStock = Datas.Warehouse.Where(x => x.WarehouseId== WarehouseID).First();
            ProductDetail ParentDetail = Datas.ProductDetail.Where(x => x.ProductDetailID == ParentStock.ParentID).First();
            Product ParentProduct = Datas.Product.Where(x => x.ProductID == ParentDetail.ParentID).First();
            StockModel_BetterView ret = new StockModel_BetterView()
            {
                ShoeName = ParentProduct.ShoeName, Color = ParentDetail.ColorCode, Size = ParentStock.Size, Stock = ParentStock.Stock, WarehouseId= WarehouseID
            };
            return ret;
        }
    }
}
