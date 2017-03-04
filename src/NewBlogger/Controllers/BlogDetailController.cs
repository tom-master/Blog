using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NewBlogger.Controllers
{
    public class BlogDetailController : Controller
    {

        private readonly IBlogService _blogService;

        public BlogDetailController(IBlogService blogService)
        {
            _blogService = blogService;
        }


        // GET: /<controller>/
        public IActionResult Index(Guid id)
        {
            _blogService.GetBlog(id);

            return View();
        }
    }
}
