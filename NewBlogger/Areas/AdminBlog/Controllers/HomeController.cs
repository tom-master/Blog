using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NewBlogger.Areas.AdminBlog.Controllers
{
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