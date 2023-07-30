using E_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySqlX.XDevAPI;

namespace WebApplication2.Controllers
{
    public abstract class BaseController : Controller
    {
     
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
         
            this.ViewBag.Authenticated = this.HttpContext.Session.GetString("login") != null;
        }
    }
}
