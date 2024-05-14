using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppDP.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ViewResult InternalServerError()
        {
            return View();
        }

        public ViewResult OtherHttpStatusCode(int httpStatusCode)
        {
            Response.StatusCode = httpStatusCode;
            return View();
        }
    }
}