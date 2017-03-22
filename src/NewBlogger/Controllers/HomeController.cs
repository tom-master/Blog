using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;

namespace NewBlogger.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;

        private readonly ICategoryService _categoryService;

        private readonly ICommentService _commentService;

        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IBlogService blogService, ICategoryService categoryService, ICommentService commentService, IHostingEnvironment hostingEnvironment)
        {
            _blogService = blogService;

            _categoryService = categoryService;

            _commentService = commentService;

            _hostingEnvironment = hostingEnvironment;
        }

        #region pages

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BlogDetail(Guid blogId)
        {
            return View(_blogService.GetBlog(blogId));
        }

        #endregion

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCategories()
        {
            return Json(new
            {
                categories = _categoryService.GetCategorys()
            });
        }


        /// <summary>
        /// 获取所有文章
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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

        /// <summary>
        /// 回复当前博客
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Reply()
        {

            var nickName = Request.Form["name"];

            var email = Request.Form["email"];

            var blogId = Guid.Parse(Request.Form["blogId"]);

            var replyId = (Request.Form["replyId"] + "").Length <= 0 ? null : (Guid?)Guid.Parse(Request.Form["replyId"]);

            var message = Request.Form["message"];

            await _commentService.AddCommentAsync(nickName, email, blogId, message, replyId);

            return Json(new { status = 1 });
        }

        [HttpPost]
        public IActionResult UploadImage()
        {

            var file = Request.Form.Files[0];

            var filePath = $@"{_hostingEnvironment.WebRootPath}\UploadImage\";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var fullPath = $@"{filePath}{file.FileName}";

            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            return Content(fullPath.Replace(_hostingEnvironment.WebRootPath, $"http://{Request.Host.Value}").Replace("\\", "/"));
        }
    }
}
