using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewBlogger.Controllers
{
    public class TestController : Controller
    {

        private readonly ICategoryService _categoryService;

        private readonly IBlogService _blogService;


        public TestController(ICategoryService categoryService, IBlogService blogService)
        {
            _categoryService = categoryService;

            _blogService = blogService;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> AddCategory(String categoryName)
        {
            await _categoryService.AddCategoryAsync(categoryName);

            return Json(new { });
        }

        public IActionResult AddBlog(String title, String content, Guid categoryId)
        {
            _blogService.AddNewBlog(title, content, categoryId);

            return Json(new { });
        }
    }
}
