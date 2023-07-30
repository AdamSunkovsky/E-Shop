using E_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication2.Controllers;

namespace E_Shop.Controllers
{
    public class CatalogController : BaseController
    {
        private MyContext Datas = new MyContext();
        private List<Product> FinalCatalog = new List<Product>();
        private SaleTranslator SaleTranslator = new SaleTranslator();
        public IActionResult Index(FilterModel srch)
        {
            this.FinalCatalog =  Datas.Product.ToList();
        
            if(string.IsNullOrEmpty(srch.Search) == false)
            {
               this.Search(srch.Search);
            }
            this.MinPrice(srch.Min);
            if(srch.Max != 0)
            {
                this.Max(srch.Max);
            }
            if(string.IsNullOrEmpty(srch.Cat) == false || srch.Cat != "X" || srch.Cat != "")
            {
            //    this.CategoryFilter(srch.Cat);
            }
         
             List<Image> imgs = this.Datas.Image.ToList().FindAll(x => x.ImgPriority == 1);
            this.ViewBag.Images = imgs;
        
            this.GetLowestSale();
          
            this.ViewBag.Categories = this.Categories();
            if (srch.Order == 1)//nejvyšší
            {
                this.FinalCatalog = this.FinalCatalog.OrderByDescending(o => o.DefaultPrice).ToList();
            }
            else if (srch.Order == 2)
            {
                this.FinalCatalog = this.FinalCatalog.OrderBy(o => o.DefaultPrice).ToList();
            }
            this.ViewBag.Products = FinalCatalog;
            return View(srch);
        }
           public void CategoryFilter(string Cat)
        {
            List<Category> all = this.Datas.Category.ToList().Where(x => x.PrimaryCat.Contains(Cat)).ToList();
            List<int> list = new List<int>();
            foreach (var item in all)
            {
                list.Add(item.CategoryID);
            }
            List<Product> Filtered = new List<Product>();
            foreach (var item in this.FinalCatalog)
            {
                if(list.Contains(item.Categories))
                {
                    Filtered.Add(item);
                }
            }
            this.FinalCatalog = Filtered;
        }
        public void MinPrice(int min)
        {
            List<Product> AllProducts = FinalCatalog;
            List<Product> Filtered = new List<Product>();
            foreach (var item in AllProducts)
            {
                if(item.DefaultPrice >= min )
                {
                    Filtered.Add(item);
                 
                   
                }
            }
            this.FinalCatalog = Filtered;
        }
        public void GetLowestSale()
        {
            List<ProductDetail> AllDetails = Datas.ProductDetail.ToList();
            foreach (var item in this.FinalCatalog)
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

        }
        public void Max(int max)
        {
            List<Product> AllProducts = FinalCatalog;
            List<Product> Filtered = new List<Product>();
            foreach (var item in AllProducts)
            {
                if (item.DefaultPrice <= max)
                {
                    Filtered.Add(item);
                }
            }
            this.FinalCatalog = Filtered;
        }
        public void Search(string sTeach)
        {
            List<Product> AllProducts = FinalCatalog;   
            List<Product> Filtered = new List<Product>();
            foreach (var item in AllProducts)
            {
                if(item.ShoeName.Contains(sTeach) || item.Manufacturer.Contains(sTeach))
                {
                    Filtered.Add(item);
                }
            }
            this.FinalCatalog = Filtered;
        }
        public List<string> Categories()
        {
            List<Category> ALL = this.Datas.Category.ToList();
            List<string> Final = new List<string>();
            foreach (var item in ALL)
            {
                Final.Add(item.PrimaryCat);
            }
            Final = Final.Distinct().ToList();
            return Final;
        }

    }
}
