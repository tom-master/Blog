using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewBlogger.Application.Interface;

namespace NewBlogger.Areas.AdminBlog.Controllers
{
    [Area("AdminBlog")]
    public class HomeController : Controller
    {

      private readonly ICategoryService _categoryService;

        private readonly IBlogService _blogService;

        private readonly ITagService _tagService;


        public HomeController(ICategoryService categoryService, IBlogService blogService, ITagService tagService)
        {
            _categoryService = categoryService;

            _blogService = blogService;

            _tagService = tagService;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["Tags"] = _tagService.GetTags();

            ViewData["Categorys"] = _categoryService.GetCategorys();           

            return View();
        }


        public IActionResult AddCategory(String categoryName)
        {
             _categoryService.AddCategory(categoryName);

            return Json(new { });
        }

        public IActionResult AddBlog(String title, String content, Guid categoryId, Guid tagId)
        {
            _blogService.AddNewBlog(title, content, categoryId, tagId);

            return Json(new { });
        }

        public IActionResult AddTag(String tagName)
        {
             _tagService.AddTag(tagName);

            return Json(new { });
        }
    }
}