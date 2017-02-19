using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository;

namespace NewBlogger.Application
{
    public class BlogService : IBlogService
    {
        private readonly IRepository<Blog> _blogRepository;

        private readonly IRepository<Category> _categoryRepository;

        private readonly IRepository<Comment> _commentRepository;

        public BlogService(IRepository<Blog> blogRepository, IRepository<Category> categoryRepository, IRepository<Comment> commentRepository)
        {
            _blogRepository = blogRepository;

            _categoryRepository = categoryRepository;

            _commentRepository = commentRepository;
        }

        public IList<BlogDto> GetBlogs(String categoryId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            return _blogRepository.Find(b => (categoryId == default(String) || b.CategoryId == categoryId), pageIndex, pageSize, out totalCount).ToList().Select(s => new BlogDto
            {
                CategoryId = s.CategoryId,
                CategoryName = _categoryRepository.Find().FirstOrDefault(c => c.Id == s.CategoryId).Name,
                CommentCount = _commentRepository.Find().Count(c => c.BlogId == s.Id),
                Content = s.Content,
                Id = s.Id,
                Title = s.Title,
                ViewCount = s.ViewCount,
                AddTime = s.AddTime
            }).ToList();
        }

        public BlogDto GetBlog(String blogId)
        {
            var internalBlog = _blogRepository.Find().FirstOrDefault(b => b.Id == blogId);

            return new BlogDto
            {
                CategoryId = internalBlog.CategoryId,
                CategoryName = _categoryRepository.Find().FirstOrDefault(c => c.Id == internalBlog.CategoryId).Name,
                CommentCount = _commentRepository.Find().Count(c => c.BlogId == internalBlog.Id),
                Content = internalBlog.Content,
                Id = internalBlog.Id,
                Title = internalBlog.Title,
                ViewCount = internalBlog.ViewCount,
                AddTime = internalBlog.AddTime
            };
        }

        public async Task AddViewCountAsync(String blogId)
        {
            var blog = _blogRepository.Find().FirstOrDefault(b => b.Id == blogId);

            blog.AddViewCount();

            IList<Tuple<Object, Object>> fields = new List<Tuple<Object, Object>>
            {
                new Tuple<Object, Object>("ViewCount", blog.ViewCount)
            };

            await _blogRepository.ModifyAsync(d => d.Id == blogId, fields);
        }
    }
}
