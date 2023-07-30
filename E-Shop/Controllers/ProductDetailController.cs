using E_Shop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Newtonsoft.Json;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using WebApplication2.Controllers;

namespace E_Shop.Controllers
{
    public class ProductDetailController : BaseController
    {
        MyContext Datas = new MyContext();
        private SaleTranslator ST = new SaleTranslator();
        public IActionResult Index(int ParentProductID, int? DetailID, int? ShoeSize)
        {
            Product ParentProduct = Datas.Product.ToList().Find(x => x.ProductID == ParentProductID);
            List<ProductDetail> AviableDetails = Datas.ProductDetail.ToList().FindAll(x => x.ParentID == ParentProductID);
            if(DetailID == null )
            {
                if(AviableDetails.Count == 0)
                {
                    return RedirectToAction("MissingProduct","Home");
                } else
                {
                    DetailID = AviableDetails[0].ProductDetailID;           //Default
                    
                }
            }
            
            this.ViewBag.AllDetails = AviableDetails;                   //Selector
           ProductDetail CurrDet = AviableDetails.Find(x => x.ProductDetailID == DetailID);   //CurrentDetail
            if (CurrDet.ProductSale == 1 || CurrDet.ProductSale == 2)
            {
                this.ViewBag.Sale = CurrDet.ProductPrice;
                CurrDet.ProductPrice = (int)ST.FinalPrice(CurrDet.ProductSale, CurrDet.ProductPrice);
            }
            this.ViewBag.CurrentDetail = CurrDet;
            Image? Main = Datas.Image.ToList().Find(x => x.ParentID == ParentProductID && x.ImgPriority == 1);
            if(Main != null)
            {
                this.ViewBag.MainIMG = Main.ImgURL;
            } else
            {
                this.ViewBag.MainIMG = "/ShoeImages/default.png";
            }
            List<Product> Random = this.Datas.Product.ToList().FindAll(x => x.ProductID != ParentProductID);
            Random rnd = new Random();
            this.ViewBag.Products = Random.OrderBy(x => rnd.Next()).Take(4);
            List<Image> imgs = this.Datas.Image.ToList().FindAll(x => x.ImgPriority != 1 && x.ParentID == ParentProductID);
            string x = AviableDetails.Find(x => x.ProductDetailID == DetailID).ProductSizes;
            this.ViewBag.Sizes = x.Split(';');
            this.ViewBag.Images = imgs;
            this.ViewBag.CurrentSize = ShoeSize;
            this.ViewBag.StringSize = ShoeSize.ToString();
            this.ViewBag.ParentProduct = ParentProduct;
            this.ViewBag.OtherImages = this.Datas.Image.ToList().FindAll(x => x.ImgPriority == 1);
            if (DetailID > 0 && ShoeSize > 0)
            {
                
                this.ViewBag.AllowBuy = true;
            } else
            {
                this.ViewBag.AllowBuy = false;
            }
            List<Warehouse> NoStockSize = this.Datas.Warehouse.ToList().FindAll(x => x.ParentID == DetailID && x.Stock < 1);
            List<string> Nostock = new List<string>();
            foreach (Warehouse item in NoStockSize)
            {
                Nostock.Add(item.Size.ToString());
            }
            this.ViewBag.Empty = Nostock;
            return View();
        }

        public IActionResult ProductADD(ProductInBasket Model)
        {

            string Json = this.HttpContext.Session.GetString("Basket");
            if (Json == null)
            {
                this.CreateSessionBasket();
                Json = this.HttpContext.Session.GetString("Basket");
            }
            Model.ProductName = this.Datas.Product.ToList().Find(x => x.ProductID == Model.ProductID).ShoeName;
            Model.ImgURL = this.Datas.Image.ToList().Find(x => x.ParentID == Model.ProductID && x.ImgPriority == 1).ImgURL;
            Model.ChildStockID = this.GetChildStock(Model.VariantID, Model.Size);
            BasketModel BModel = JsonConvert.DeserializeObject<BasketModel>(Json);
            BModel.Products.Add(Model);

            string BasketInJson = JsonConvert.SerializeObject(BModel);
             this.HttpContext.Session.SetString("Basket", BasketInJson);
            return RedirectToAction("Index", "Basket");
        }
        public void CreateSessionBasket()
        {
            BasketModel BModel = new BasketModel();

            string Json = JsonConvert.SerializeObject(BModel);
            this.HttpContext.Session.SetString("Basket", Json);
        }
        public int GetChildStock(int variant, int size)
        {
            return this.Datas.Warehouse.ToList().Find(x => x.ParentID == variant && x.Size == size).WarehouseId;
        }
    }
    
}
