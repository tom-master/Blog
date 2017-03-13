using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;

namespace NewBlogger.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;

        private readonly ICategoryService _categoryService;

        private readonly ICommentService _commentService;

        public HomeController(IBlogService blogService, ICategoryService categoryService, ICommentService commentService)
        {
            _blogService = blogService;

            _categoryService = categoryService;

            _commentService = commentService;
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

        public async Task<IActionResult> Reply()
        {

            var nickName = Request.Form["name"];

            var email = Request.Form["email"];

            var blogId = Guid.Parse(Request.Form["blogId"]);

            var replyId =Guid.NewGuid() /*Guid.Parse(Request.Form["replyId"])*/;

            var message = Request.Form["message"];

            await _commentService.AddCommentAsync(nickName, email, blogId, message, replyId);

            return Json(new { });
        }
    }
}
