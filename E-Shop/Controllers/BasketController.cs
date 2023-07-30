using E_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication2.Controllers;

namespace E_Shop.Controllers
{
    public class BasketController : BaseController
    {
        private MyContext Datas = new MyContext();
        private MailServices Mail = new MailServices();
        public IActionResult Index()
        {
            BasketModel CurrentBasket = new BasketModel();
            string JsonBasket =this.HttpContext.Session.GetString("Basket");
            if(!string.IsNullOrEmpty(JsonBasket))
            {
                CurrentBasket = JsonConvert.DeserializeObject<BasketModel>(JsonBasket);
            }
            if(CurrentBasket.Products.Count == 0)
            {
                this.ViewBag.Empty = true;
            } else
            {
                this.ViewBag.Empty = false;
            }
            foreach (var item in CurrentBasket.Products)
            {
                ProductDetail P = this.Datas.ProductDetail.ToList().Where(x => x.ProductDetailID == item.VariantID).First();
                item.VariantString = P.ColorCode;
            }
            CurrentBasket.CalculateDetails();
            return View(CurrentBasket);
        }
     
        public IActionResult Checkout()
        {
            BasketModel CurrentBasket = new BasketModel();
            string JsonBasket = this.HttpContext.Session.GetString("Basket");
            if (!string.IsNullOrEmpty(JsonBasket))
            {
                CurrentBasket = JsonConvert.DeserializeObject<BasketModel>(JsonBasket);
            }
            CurrentBasket.CalculateDetails();
            this.ViewBag.Basket = CurrentBasket;
            this.LowerStock(CurrentBasket);
            
            return View();
        }
        public IActionResult RemoveFromBasket(int ID, int CHILDID)
        {
            BasketModel CurrentBasket = new BasketModel();
            string JsonBasket = this.HttpContext.Session.GetString("Basket");
            if (!string.IsNullOrEmpty(JsonBasket))
            {
                CurrentBasket = JsonConvert.DeserializeObject<BasketModel>(JsonBasket);
            }
            ProductInBasket Target = CurrentBasket.Products.Where(x => x.ProductID == ID && x.ChildStockID == CHILDID).First();
            CurrentBasket.Products.Remove(Target);

            string BasketInJson = JsonConvert.SerializeObject(CurrentBasket);
            this.HttpContext.Session.SetString("Basket", BasketInJson);
            return RedirectToAction("Index");
        }
        public IActionResult PaidOrder(CustomerData Secret)
        {
            if(String.IsNullOrEmpty(Secret.Address2))
            {
                Secret.Address2 = "";
            }
            BasketModel CurrentBasket = new BasketModel();
            string JsonBasket = this.HttpContext.Session.GetString("Basket");
            if (!string.IsNullOrEmpty(JsonBasket))
            {
                CurrentBasket = JsonConvert.DeserializeObject<BasketModel>(JsonBasket);
            }
            string Basket = "";
            foreach (var item in CurrentBasket.Products)
            {
                Basket = Basket + "  " + item.ProductName + $"({item.VariantID})";
            }
            Secret.Order = Basket;
            this.NewOrder(Secret);
            //this.Mail.Order(Secret.Email, "Potrvzení objevnávka","", Secret);
            BasketModel ClearBasket = new BasketModel();
            this.HttpContext.Session.SetString("Basket", JsonConvert.SerializeObject(ClearBasket));
            return View();
        }

        public void LowerStock(BasketModel Basket)
        {
            foreach (var item in Basket.Products)
            {           
                using (var db = new MyContext())
                {
                   Warehouse wh = this.Datas.Warehouse.ToList().Where(x => x.WarehouseId == item.ChildStockID).First();
                    wh.Stock = wh.Stock - item.Count;
                    this.Datas.SaveChanges();

                }
            }
        }
        public void NewOrder(CustomerData S)
        {
            Order NewOrder = new Order();
            NewOrder.CEmail = S.Email;
            NewOrder.CSurname = S.Surname;
            NewOrder.CName = S.Name;
            NewOrder.CAdress = S.Address;
            NewOrder.OrderTime = DateTime.Now;
            NewOrder.OrderStatus = "Active";
            NewOrder.Note = "";
            NewOrder.Basket = S.Order;
            NewOrder.Payment = "Paypal";
            this.Datas.Add<Order>(NewOrder);
            this.Datas.SaveChanges();
        }


    }
}
