using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository;
using NewBlogger.Repository.Base;

namespace NewBlogger.Application
{
    public class BlogService : IBlogService
    {
        private readonly RepositoryBase<Blog> _blogRepository;

        private readonly RepositoryBase<Category> _categoryRepository;

        private readonly RepositoryBase<Comment> _commentRepository;

        private readonly RepositoryBase<Tag> _tagRepository;

        public BlogService(RepositoryBase<Blog> blogRepository, RepositoryBase<Category> categoryRepository, RepositoryBase<Comment> commentRepository, RepositoryBase<Tag> tagRepository)
        {
            _blogRepository = blogRepository;

            _categoryRepository = categoryRepository;

            _commentRepository = commentRepository;

            _tagRepository = tagRepository;
        }

        public IList<BlogDto> GetBlogs(Guid? categoryId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            return _blogRepository.Find(b => (categoryId == default(Guid?) || b.CategoryId == categoryId), pageIndex, pageSize, out totalCount).ToList().Select(s => new BlogDto
            {
                CategoryId = s.CategoryId,
                CommentCount = _commentRepository.Find().Count(c => c.BlogId == s.Id),
                Content = s.Content,
                Id = s.Id,
                Title = s.Title,
                ViewCount = s.ViewCount,
                AddTime = s.AddTime,
            }).ToList();
        }

        public BlogDto GetBlog(Guid blogId)
        {

            var internalBlog = _blogRepository.Find().FirstOrDefault(b => b.Id == blogId);

            return new BlogDto
            {
                CategoryId = internalBlog.CategoryId,
                CategoryName = _categoryRepository.Find().FirstOrDefault(c => c.Id == internalBlog.CategoryId).Name,
                Content = internalBlog.Content,
                Id = internalBlog.Id,
                Title = internalBlog.Title,
                ViewCount = internalBlog.ViewCount,
                AddTime = internalBlog.AddTime,
                Comments = GetBlogComments(internalBlog.Id),
                Tags = GetBlogTag(internalBlog.Tags)
            };
        }

        public async Task AddViewCountAsync(Guid blogId)
        {
            var blog = _blogRepository.Find().FirstOrDefault(b => b.Id == blogId);

            blog.AddViewCount();

            IList<Tuple<Object, Object>> fields = new List<Tuple<Object, Object>>
            {
                new Tuple<Object, Object>("ViewCount", blog.ViewCount)
            };

            await _blogRepository.ModifyAsync(d => d.Id == blogId, fields);
        }

        private IList<CommentDto> GetBlogComments(Guid blogId)
        {
            var comments = _commentRepository.Find().Where(w => w.BlogId == blogId);

            return comments.Any() ? comments.ToList().Where(w => w.ReplyId == w.Id || w.BlogId == blogId).Select(
                s => new CommentDto
                {
                    AddTime = s.AddTime,
                    Content = s.Content,
                    ReplyNickName = s.ReplyNickName,
                    ParentReplyId = s.ReplyId,
                    Id = s.Id
                }).ToList() : new List<CommentDto>();
        }

        private IList<TagDto> GetBlogTag(params Guid[] tagIds)
        {
            if (tagIds == null)
            {
                return new List<TagDto>();
            }

            var tags = _tagRepository.Find().Where(w => tagIds.Contains(w.Id));

            return tags.Any() ? tags.Select(
                s => new TagDto
                {
                    AddTime = s.AddTime,
                    Id = s.Id,
                    Name = s.Name
                }).ToList() : new List<TagDto>();
        }
    }
}
