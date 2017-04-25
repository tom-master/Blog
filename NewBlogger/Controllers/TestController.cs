using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;

namespace NewBlogger.Controllers
{
    public class TestController : Controller
    {

        private readonly ICategoryService _categoryService;

        private readonly IBlogService _blogService;

        private readonly ITagService _tagService;


        public TestController(ICategoryService categoryService, IBlogService blogService, ITagService tagService)
        {
            _categoryService = categoryService;

            _blogService = blogService;

            _tagService = tagService;
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

        public IActionResult AddBlog(String title, String content, Guid categoryId, Guid tagId)
        {
            _blogService.AddNewBlog(title, content, categoryId, tagId);

            return Json(new { });
        }

        public async Task<IActionResult> AddTag(String tagName)
        {
            await _tagService.AddTagAsync(tagName);

            return Json(new { });
        }
    }
}
