using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //if API dosen't know what to do with route, then it falls back to this controller and return this particular file
    //index.html is responsible for loading our Angular app
    //Angular app definetley knows what to do with the routes, for the Angualr application
        public class FallbackController: Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}