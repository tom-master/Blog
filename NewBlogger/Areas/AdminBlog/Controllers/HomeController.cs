using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NewBlogger.Areas.AdminBlog.Controllers
{
  [Area("AdminBlog")]
  public class HomeController : Controller
  {

    public HomeController()
    {

    }

    
    public IActionResult Index()
    {
        return View();
    }
  }
}