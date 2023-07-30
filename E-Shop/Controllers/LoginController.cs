using E_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication2.Controllers;

namespace E_Shop.Controllers
{
    public class LoginController : BaseController
    {
        [HttpGet]
        public IActionResult Index(string next = "")
        {
            return View(new Login() );
        }
        MyContext Cntx = new MyContext();
       
        [HttpPost]
        public IActionResult Index(Login model)
        {
         
            if (ValidateLogin(model.Username,model.Password))
            {
                this.HttpContext.Session.SetString("login", model.Username);
                var NewLog = new WebsiteLog {Info = $"{model.Username}: Login OK", Time = DateTime.Now, Type="" };
                Cntx.Add<WebsiteLog>(NewLog);
                Cntx.SaveChanges();
                return RedirectToAction("Index","Home");
            } else
            {
                ViewBag.FalseLogin = false;
            }
            return View(model);
        }
        public IActionResult Logout()
        {
            this.HttpContext.Session.Remove("login");
            return RedirectToAction("Index", "Login");
        }
        public bool ValidateLogin(string Usrnm, string pwd)
        {
            List<Login> Admins = Cntx.Login.ToList<Login>();
            string password = "";
            foreach (Login item in Admins)
            {
                if(item.Username == Usrnm)
                {
                    password = item.Password;
                }
            }
            if (pwd == password)
            {
                return true;
            }      
            else
                return false;


        }
    }
}
