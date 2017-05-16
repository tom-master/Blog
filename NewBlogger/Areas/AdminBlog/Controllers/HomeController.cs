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

        [HttpPost]
        public IActionResult AddCategory(String categoryName)
        {
            _categoryService.AddCategory(categoryName);

            return Json(new { });
        }

        [HttpPost]
        public IActionResult AddBlog(String title, String content, Guid categoryId, Guid tagId)
        {
            _blogService.AddNewBlog(title, content, categoryId, tagId);

            return Json(new { });
        }

        [HttpPost]
        public IActionResult AddTag(String tagName)
        {
            _tagService.AddTag(tagName);

            return Json(new { });
        }

        [HttpGet]
        public IActionResult GetBlogs(Int32 pageIndex, Int32 pageSize)
        {
            var totalCount = 0;

            var blogs = _blogService.GetBlogs(Guid.Empty, pageIndex, pageSize, out totalCount);

            return Json(new { blogs = blogs, totalCount = totalCount });
        }

        [HttpPost]
        public IActionResult RemoveTag(Guid tagId)
        {
            _tagService.RemoveTag(tagId);
            
            return Json(new { });
        }

        [HttpPost]
        public IActionResult RemoveCategory(Guid categoryId)
        {
            _categoryService.RemoveCategory(categoryId);

            return Json(new { });
        }

        [HttpPost]
        public IActionResult RemoveBlog(Guid blogId)
        {
            return Json(new {});
        }
    }
}