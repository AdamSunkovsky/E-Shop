using E_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApplication2.Controllers;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting.Server;

namespace E_Shop.Controllers
{
    public class PwdResetController : BaseController
    {
        public string MailCode { get; set; }
        public MailServices EmailServices = new MailServices();
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Submited = false;
            this.MailCode = GetCode();


            return View(new Reset());
        }

        [HttpPost]
        public IActionResult Index(Reset model)
        {
            ViewBag.Submited = true;
           
            
            EmailServices.MailSrv(model.Email,"Forgotten password recovery code", $"Your recovery code is:{this.MailCode}");
            return View(model);
        }
        public IActionResult Mailed(Reset model)
        {
            //Get random number with 8 diggits, if - return view nebo reddict and send email with pwd
            if(CodeCorrect("DefaultRestoreCode", model.RestoreCode) || CodeCorrect(this.MailCode, model.RestoreCode))
            {
                return RedirectToAction("Index", "Login");
            } else
            {              
              return View();
            }
          
        }
        public bool CodeCorrect(string Generated, string Inserted)
        {
            return Generated == Inserted;
        }
        public string GetCode()
        {
            int length = 10;
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                while (0 < length--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }
               string Pwd = res.ToString();
            return Pwd;
        }
       
      
    }
}
