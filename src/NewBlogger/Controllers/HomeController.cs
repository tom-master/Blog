using System;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;

namespace NewBlogger.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;

        private readonly ICategoryService _categoryService;

        public HomeController(IBlogService blogService, ICategoryService categoryService)
        {
            _blogService = blogService;

            _categoryService = categoryService;
        }

        #region pages

        public IActionResult Index()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// get all Categories
        /// </summary>
        /// <returns></returns>
        public IActionResult GetCategories()
        {
            return Json(new
            {
                categories = _categoryService.GetCategorys()
            });
        }


        /// <summary>
        /// get all blog
        /// </summary>
        /// <returns></returns>
        public IActionResult GetBlogs(Int32 pageIndex, Int32 pageSize, Guid? categoryId = default(Guid?))
        {
            Int32 totalCount;

            var blogs = _blogService.GetBlogs(categoryId, pageIndex, pageSize, out totalCount);

            return Json(new
            {
                totalCount,
                blogs
            });
        }
    }
}
