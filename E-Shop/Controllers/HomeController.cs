using E_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using WebApplication2.Controllers;


namespace E_Shop.Controllers
{
    public class HomeController : BaseController
    {
        MyContext Datas = new MyContext();
        private List<int> UsedProducts { get; set; }
        private SaleTranslator SaleTranslator = new SaleTranslator();
        public IActionResult Index()
        {
        
            this.UsedProducts = new List<int>();
            List<Product> products = Datas.Product.ToList<Product>();
            this.ViewBag.Products = new List<Product>();
            List<Image> imgs = this.Datas.Image.ToList().FindAll(x => x.ImgPriority == 1);
            List<Product> RandomPrdct = new List<Product>();
            this.ViewBag.Images = imgs;
            if(products.Count > 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    RandomPrdct.Add(products[RandomProductID(products.Count)]);
                }
            }
            else
            {
                foreach (var item in products)
                {
                    RandomPrdct.Add(item);
                }
            }
            
            this.ViewBag.Products = GetLowestSale(RandomPrdct);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult MissingProduct()
        {
            return View();
        }
        private int RandomProductID(int max)
        {
            Random random = new Random();
            int ID = random.Next(0, max);
            while (this.UsedProducts.Contains(ID))
            {
                ID = random.Next(1, max);
            }
            this.UsedProducts.Add(ID);
            return ID;
        }

        public List<Product> GetLowestSale(List<Product> Src)
        {
            List<Product> products = Src;
            List<ProductDetail> AllDetails = Datas.ProductDetail.ToList();
            foreach (var item in products)
            {
                List<ProductDetail> Current = AllDetails.FindAll(x => x.ParentID == item.ProductID);
                List<int> Sales = new List<int>();
                foreach (var Det in Current)
                {
                    Sales.Add(Det.ProductSale);
                }
                try
                {
                    if (Sales.Max() > 0)
                    {
                        item.LowestPrice = (int)SaleTranslator.FinalPrice(Sales.Max(), item.DefaultPrice);
                    }
                    else
                    {
                        item.LowestPrice = 0;
                    }
                }
                catch
                {
                    item.LowestPrice = 0;
                }

            }
            return products;
        }

    }
}