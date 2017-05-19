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


        /// <summary>
        /// 后台管理首页
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Tags"] = _tagService.GetTags();

            ViewData["Categorys"] = _categoryService.GetCategorys();

            return View();
        }


        

        #region 添加
            /// <summary>
            /// 添加分类
            /// </summary>
            /// <param name="categoryName"></param>
            /// <returns></returns>
            [HttpPost]
            public IActionResult AddCategory(String categoryName)
            {
                _categoryService.AddCategory(categoryName);

                return Json(new { });
            }


            /// <summary>
            /// 添加文章
            /// </summary>
            /// <param name="title"></param>
            /// <param name="content"></param>
            /// <param name="categoryId"></param>
            /// <param name="tagId"></param>
            /// <returns></returns>
            [HttpPost]
            public IActionResult AddBlog(String title, String content, Guid categoryId, Guid tagId)
            {
                _blogService.AddNewBlog(title, content, categoryId, tagId);

                return Json(new { });
            }

            /// <summary>
            /// 添加标签
            /// </summary>
            /// <param name="tagName"></param>
            /// <returns></returns>
            [HttpPost]
            public IActionResult AddTag(String tagName)
            {
                _tagService.AddTag(tagName);

                return Json(new { });
            }    
        #endregion

        #region 获取
            /// <summary>
            /// 获取博客列表
            /// </summary>
            /// <param name="pageIndex"></param>
            /// <param name="pageSize"></param>
            /// <returns></returns>
            [HttpGet]
            public IActionResult GetBlogs(Int32 pageIndex, Int32 pageSize)
            {
                var totalCount = 0;

                var blogs = _blogService.GetBlogs(Guid.Empty, pageIndex, pageSize, out totalCount);

                return Json(new { blogs = blogs, totalCount = totalCount });
            }   
        #endregion

        #region 移除

            /// <summary>
            /// 移除标签
            /// </summary>
            /// <param name="tagId"></param>
            /// <returns></returns>
            [HttpPost]
            public IActionResult RemoveTag(Guid tagId)
            {
                _tagService.RemoveTag(tagId);
                
                return Json(new { });
            }


            /// <summary>
            /// 移除分类
            /// </summary>
            /// <param name="categoryId"></param>
            /// <returns></returns>
            [HttpPost]
            public IActionResult RemoveCategory(Guid categoryId)
            {
                _categoryService.RemoveCategory(categoryId);

                return Json(new { });
            }

            /// <summary>
            /// 移除文章
            /// </summary>
            /// <param name="blogId"></param>
            /// <returns></returns>
            [HttpPost]
            public IActionResult RemoveBlog(Guid blogId)
            {
                return Json(new {});
            }
        #endregion
        
    }
}