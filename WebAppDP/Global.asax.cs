using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebAppDP.Controllers;

namespace WebAppDP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "NotFound";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 404;
            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "AccessDenied";
                        break;
                    case 404:
                        routeData.Values["action"] = "NotFound";
                        break;
                    case 500:
                        routeData.Values["action"] = "InternalServerError";
                        break;
                    default:
                        routeData.Values["action"] = "OtherHttpStatusCode";
                        routeData.Values["httpStatusCode"] = Response.StatusCode;
                        break;
                }
            }
            else
            {
                routeData.Values["action"] = "OtherHttpStatusCode";
                routeData.Values["httpStatusCode"] = Response.StatusCode;

            }

            IController errorsController = new ErrorController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(rc);
        }
    }
}
