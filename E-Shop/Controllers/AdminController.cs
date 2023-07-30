using Microsoft.AspNetCore.Mvc;
using WebApplication2.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Shop.Models;
using System.Security.Policy;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting.Server;
using System.Linq;
using System.Web;

namespace E_Shop.Controllers
{
    public class AdminController : BaseController
    {
        MyContext Datas = new MyContext();
        private MailServices MailSrv = new MailServices();
        private BetterViewGenerator BVG = new BetterViewGenerator();
        public IActionResult Index()
        {
            this.ViewBag.Cats = this.Datas.Category.ToList();
            return View();
        }
        public IActionResult ImgEdit(int ID)
        {
            Image main = this.Datas.Image.Where(x => x.ParentID == ID && x.ImgPriority == 1).First();
            ImgURLs Imgs = new ImgURLs();
            List<Image> images = this.Datas.Image.ToList().FindAll(x => x.ParentID == ID && x.ImgPriority != 1);
            if(main != null)
            {

                Imgs.main = main.ImgURL;
                   Imgs.OldM = main.ImgURL;
           
                if (images.Count >= 3)
                {
                    Imgs.Third = images[2].ImgURL;
                    Imgs.OldT = images[2].ImgURL;
                }
                if (images.Count >= 2)
                {
                    Imgs.Second = images[1].ImgURL;
                    Imgs.OldS = images[1].ImgURL;
                }
                if (images.Count >= 1)
                {
                    Imgs.OldF = images[0].ImgURL;
                    Imgs.First = images[0].ImgURL;
                }
            }
            Imgs.Parent = ID;
            return View(Imgs);
        }
        public IActionResult UploadImg(ImgURLs Src)
        {
            if (Src.OldM != Src.main)
            {
                Image replace = this.Datas.Image.ToList().Find(x => x.ParentID == Src.Parent && x.ImgPriority == 1);
                replace.ImgURL = Src.main;
                this.Datas.SaveChanges();
            }
            if (Src.OldF != Src.First)
            {
                    Image replace = this.Datas.Image.ToList().Find(x => x.ImgURL == Src.OldF && x.ImgPriority != 1);                
                    replace.ImgURL = Src.First;
                    this.Datas.SaveChanges();    
            }
            if (Src.OldS != Src.Second)
            {
                Image replace = this.Datas.Image.ToList().Find(x => x.ImgURL == Src.OldS && x.ImgPriority != 1);
                replace.ImgURL = Src.Second;
                this.Datas.SaveChanges();
            }
            if (Src.OldT != Src.Third)
            {
                Image replace = this.Datas.Image.ToList().Find(x => x.ImgURL == Src.OldT && x.ImgPriority != 1);
                replace.ImgURL = Src.Third;
                this.Datas.SaveChanges();
            }
            return RedirectToAction("Editor", new RouteValueDictionary(new { controller = "Admin", action = "Editor", Ed = Src.Parent }));
        }
        public IActionResult CancelOrder(int ID)
        {
            Order Order = this.Datas.Order.ToList().Find(x => x.OrderID == ID);
            this.ViewBag.Order = Order;
            EmailModel Email = new EmailModel()
            {
                To = Order.CEmail,
                Subject = "Your order has been canceled!",
            };
            this.ViewBag.Email = Email;

            return View(Email);
        }
        public IActionResult DeleteOrder(EmailModel Email)
        {
            Order Order = this.Datas.Order.ToList().Find(x => Email.ID == x.OrderID);
          //  MailSrv.Cancel(Email.To, Email.Subject, Email.Body, Order);
            this.Datas.Remove<Order>(Order);
            this.Datas.SaveChanges();
            return RedirectToAction("ActiveOrders", "Admin");
        }
        public IActionResult StockManagment (FilterModel Filter)
        { 
            List<Product> products = Datas.Product.ToList();
            List<Product> Filtered = new List<Product>();
            if(Filter.Search != null)
            {
                foreach (var item in products)
                {
                    if (item.ShoeName.Contains(Filter.Search) ||
                        item.Manufacturer.Contains(Filter.Search) ||
                        item.ShoeMaterial.Contains(Filter.Search) ||
                        item.Description.Contains(Filter.Search) ||
                        item.ProductID.ToString().Contains(Filter.Search) ||
                        item.ProductCode.Contains(Filter.Search)
                        )
                    { Filtered.Add(item); }                
                }
            }
            else
            {
                Filtered = products;
            }      
            this.ViewBag.Products = Filtered;
            return View(Filter);        
        }
        public IActionResult UploadNewProduct(Product NEW)
        {
            NEW.ProductID = NextID();
            this.LogAction($"Uploaded new product {NEW.ShoeName} {NEW.ProductID}", "New");
            this.Datas.Add<Product>(NEW);
            this.Datas.SaveChanges();
            Image New = new Image()
            {
                ImgPriority = 1,
                ImgURL = "",
                ParentID = NEW.ProductID
            };
            this.Datas.Add<Image>(New);
            this.Datas.SaveChanges();
            this.OtherImages(NEW.ProductID);
            return RedirectToAction("Editor", new RouteValueDictionary(new { controller = "Admin", action = "Editor", Ed = NEW.ProductID }));
        }
        public IActionResult UploadNewDetail(ProductDetail NEW)
        {
            NEW.ProductDetailID = NextDet();
            double perc = 0.21 * NEW.ProductPrice;
            NEW.DPH = (int)perc;
            this.LogAction($"Added new variant for {NEW.ParentID} ({NEW.ProductDetailID})", "New");
            this.Datas.Add<ProductDetail>(NEW);
            this.Datas.SaveChanges();
            this.GenerateStock(NEW.ProductSizes, NEW.ProductDetailID);
            return RedirectToAction("Editor", new RouteValueDictionary(new { controller = "Admin", action = "Editor", Ed = NEW.ParentID }));
        }
        public IActionResult NewProduct () { return View(); }
        public IActionResult NewDetail(int ParentID) {
            this.ViewBag.ParentID = ParentID;
            return View(); 
        }
        public IActionResult DeleteProduct(int ID)
        {
            Product ToRemove = this.Datas.Product.Where(x => x.ProductID == ID).First();
            List<ProductDetail> ChildDetails = this.Datas.ProductDetail.Where(x => x.ParentID == ID).ToList();
            if(ChildDetails.Count > 0)
            {
                foreach (ProductDetail Del in ChildDetails)
                {
                    this.Datas.Remove<ProductDetail>(Del);
                    this.Datas.SaveChanges();
                }
            }
            List<Image> ChildImages = this.Datas.Image.Where(x => x.ParentID == ID).ToList();
            if (ChildImages.Count > 0)
            {
                foreach (Image Del in ChildImages)
                {
                    this.Datas.Remove<Image>(Del);
                    this.Datas.SaveChanges();
                }
            }
            this.LogAction($"Deleted product {ID} with it's Variants!", "Delete");
            this.Datas.Remove<Product>(ToRemove);
            this.Datas.SaveChanges();
            return RedirectToAction("StockManagment", "Admin");
        }

        public int NextID ()
        {       
            List<Product> products = this.Datas.Product.ToList();
            List<int> IDs = new List<int>();
            foreach (var item in products)
            {
                IDs.Add(item.ProductID);
            }
            if(IDs.Count > 0)
            {
                return IDs.Max() + 1;
            } else
            {
                return 1;
            }         
        }
        public int NextDet ()
        {
            List<ProductDetail> det  = this.Datas.ProductDetail.ToList();
            List<int> IDs = new List<int>();
            foreach (var item in det)
            {
                IDs.Add(item.ProductDetailID);
            }
            if (IDs.Count > 0)
            {
                return IDs.Max() + 1;
            }
            else
            {
                return 1;
            }
        }
        public IActionResult LogHistory()
        { 
            List<WebsiteLog> Logs  = Datas.WebSiteLog.ToList() ;
            Logs.Reverse();
            this.ViewBag.Logs = Logs;
            return View();
        }
        public void LogAction(string msg, string? type)
        {
            var NewLog = new WebsiteLog { Info =msg, Time = DateTime.Now, Type = type };
            this.Datas.Add<WebsiteLog>(NewLog);
            this.Datas.SaveChanges();
        }
        public IActionResult Upload(Product Edited)
        {
            this.LogAction($"Edited product {Edited.ShoeName}", "Edit");
            using (var db = new MyContext())
            {
                var results = db.Product.ToList().Where(x => x.ProductID == Edited.ProductID);
                Product Target = results.FirstOrDefault();
                Target.ShoeName = Edited.ShoeName;
                Target.DefaultPrice = Edited.DefaultPrice;
                Target.Description = Edited.Description;
                Target.ProductCode = Edited.ProductCode;
                Target.Type = Edited.Type;
                Target.Categories = Edited.Categories;
                Target.Manufacturer = Edited.Manufacturer;
                db.SaveChanges();
                return RedirectToAction("StockManagment", "Admin");
            }
        }
        public IActionResult EditDetail(int ID)
        {
            return View(this.Datas.ProductDetail.ToList().Find(x => x.ProductDetailID == ID));
        }
        public IActionResult UploadDetail(ProductDetail Edited)
        {
            this.LogAction($"Edited product variant {Edited.ProductDetailID}", "Edit");
            using (var db = new MyContext())
            {
                var results = db.ProductDetail.ToList().Where(x => x.ProductDetailID == Edited.ProductDetailID);
                ProductDetail Target = results.FirstOrDefault();
                Target.ColorCode = Edited.ColorCode;
                Target.ProductSale = Edited.ProductSale;
                Target.ProductPrice = Edited.ProductPrice;         
                db.SaveChanges();
                return RedirectToAction("StockManagment", "Admin");
            }
        }
        public IActionResult DeleteDetail(int ID)
        {
            this.LogAction($"Deleted product varient {ID}", "Delete");
            ProductDetail Adios = this.Datas.ProductDetail.ToList().Find(x =>x.ProductDetailID == ID);
            this.Datas.Remove(Adios);
            var NewLog = new WebsiteLog { Info = $"Deleted Detail {Adios.ProductDetailID} for product {Adios.ParentID}", Time = DateTime.Now, Type = "DELETED" };
            this.Datas.Add<WebsiteLog>(NewLog);
            this.Datas.SaveChanges();
            return RedirectToAction("StockManagment", "Admin");
        }
        public int TotalStock(int ProductID)
        {
            int Total = 0;
            List<ProductDetail> AllDets = this.Datas.ProductDetail.ToList().FindAll(x => x.ParentID == ProductID);
            foreach (ProductDetail item in AllDets)
            {
                List<Warehouse> WHs = this.Datas.Warehouse.ToList().FindAll(a => a.ParentID == item.ProductDetailID);
                foreach (Warehouse stock in WHs)
                {
                    Total += stock.Stock;
                }
            }
            return Total;
        }
        public IActionResult Editor(int Ed)
        {
          this.ViewBag.Total = this.TotalStock(Ed);
          try
            {
                Image ALL = Datas.Image.ToList().Find(a => a.ParentID == Ed && a.ImgPriority == 1);
                if(ALL != null) { this.ViewBag.MainIMG = ALL.ImgURL; }       
            }
          catch
            {
                this.ViewBag.MainIMG = "/ShoeImages/default.png";
            }
           List<Image>? SecondaryIMGS = Datas.Image.ToList().FindAll(a => a.ParentID == Ed && a.ImgPriority != 1);
           try
            {
              foreach (var item in SecondaryIMGS)
                {
                    this.ViewBag.OtherIMGS.Add(item.ImgURL);
                }
            } catch
            {
                this.ViewBag.OtherIMGS = "";
            }
            this.ViewBag.Categories = Datas.Category.ToList();
            this.ViewBag.Details = Datas.ProductDetail.ToList().FindAll(y => y.ParentID == Ed);
            return View(Datas.Product.ToList().Find(s => s.ProductID == Ed));
        }
        public void GenerateStock(string FormatedInput, int Parent)
        {
            string[] strings= FormatedInput.Split(';');
            List<string> Sizes = strings.ToList();
            foreach (string val in Sizes)
            {
                Warehouse NewStock = new Warehouse()
                {
                    ParentID = Parent,
                    Size = Int16.Parse(val),
                    Stock = 0,
                };
                this.Datas.Add(NewStock);
                this.Datas.SaveChanges();
            }
        }
        public IActionResult UpdateStock(StockModel_BetterView Model)
        {
            using (var db = new MyContext())
            {
                Warehouse CurrentStock = db.Warehouse.ToList().Where(a => a.WarehouseId == Model.WarehouseId).First();
                this.LogAction($"Stock number updated for {Model.ShoeName}; from {CurrentStock.Stock} to {Model.Stock}", "Stock");
                CurrentStock.Stock = Model.Stock;
                db.SaveChanges();
                return RedirectToAction("WarehouseControl", "Admin");
            }
        }
        public void OtherImages(int ID)
        {
            Image F = new Image()
            {
                ImgURL = "x", ImgPriority = 2, ParentID = ID
            };
            this.Datas.Add<Image>(F);
            this.Datas.SaveChanges();
            Image S = new Image()
            {
                ImgURL = "x",
                ImgPriority = 2,
                ParentID = ID
            };
            this.Datas.Add<Image>(S);
            this.Datas.SaveChanges();
            Image T = new Image()
            {
                ImgURL = "x",
                ImgPriority = 2,
                ParentID = ID
            };
            this.Datas.Add<Image>(T);
            this.Datas.SaveChanges();
        }
        public IActionResult WarehouseControl(int? Parent) {
            List<Warehouse> WH = new List<Warehouse>();
            if (Parent == null)
            {
                WH = Datas.Warehouse.ToList();
            } else
            {
               List<ProductDetail> Dets = this.Datas.ProductDetail.ToList().FindAll(x => x.ParentID == Parent);
                foreach (var item in Dets)
                {
                    List<Warehouse> w = Datas.Warehouse.ToList().FindAll(x => x.ParentID == item.ProductDetailID);
                    foreach (var a in w)
                    {
                        WH.Add(a);
                    }
                }
            }       
            List<StockModel_BetterView> BV_Stock = new List<StockModel_BetterView>();
            foreach (Warehouse warehouse in WH)
            {
                BV_Stock.Add(BVG.GetBetterView(warehouse.WarehouseId));
            }
            this.ViewBag.Stock = BV_Stock;
            return View(); 
        }    
        public IActionResult NewCategory(Category New)
        {
            this.Datas.Add<Category>(New);
            this.Datas.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult DeleteCategory(int CategoryId)
        {
            this.Datas.Remove(this.Datas.Category.ToList().Where(x => x.CategoryID == CategoryId).First());
            this.Datas.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult ActiveOrders()
        {
            this.ViewBag.Orders = this.Datas.Order.ToList();
            return View();
        }
    }
}
