using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;

namespace NewBlogger.Components
{
    public class BlogDetailComponent : ViewComponent
    {
        private readonly IBlogService _blogService;

        public BlogDetailComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public IViewComponentResult Inovke(Guid id)
        {

            var blog = _blogService.GetBlog(id);

            return View("Index", blog);
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid id)
        {
            var blog = await Task.Run(() => _blogService.GetBlog(id));

            return View(blog);
        }
    }
}

