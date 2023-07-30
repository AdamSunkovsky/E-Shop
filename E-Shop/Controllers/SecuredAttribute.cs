using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication2.Controllers
{
    public class SecuredAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Controller ctrl = (Controller) context.Controller;

            if (ctrl.HttpContext.Session.GetString("login") == null)
            {
                string c = ctrl.ControllerContext.RouteData.Values["controller"].ToString();
                string a = ctrl.ControllerContext.RouteData.Values["action"].ToString();

                context.Result = new RedirectToActionResult("Index", "Login", new { next = $"{c}:{a}" });
            }
        }
    }
}
