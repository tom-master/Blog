using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl;

namespace NewBlogger.Application
{
    public class BlogService : IBlogService
    {

        private readonly RedisRepositoryBase _redisRepository;

        public BlogService(RedisRepositoryBase redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public IList<BlogDto> GetBlogs(Guid? categoryId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Int32 internalStart = (pageIndex - 1) * pageSize, internalEnd = (pageSize + internalStart) - 1;

            var blogRedisKey = "NewBlogger:Blogs";

            var blogs = _redisRepository.ListRange<BlogDto>(blogRedisKey, internalStart, internalEnd);

            totalCount = (Int32)_redisRepository.ListLength(blogRedisKey);

            var commentBlogsRedisKey = "NewBlogger:Comments:BlogId:";

            return blogs.Select(s => new BlogDto
            {
                CategoryId = s.CategoryId,
                CommentCount = (Int32)_redisRepository.ListLength(commentBlogsRedisKey + s.Id),
                Content = s.Content,
                Id = s.Id,
                Title = s.Title,
                ViewCount = s.ViewCount,
                AddTime = s.AddTime,
            }).ToList();
        }

        public BlogDto GetBlog(Guid blogId)
        {
            var blogRedisKey = "NewBlogger:Blogs";

            var internalBlog = _redisRepository.ListRange<Blog>(blogRedisKey, 0, -1).FirstOrDefault(d => d.Id == blogId);

            var categoryRedisKey = "NewBlogger:Categorys";

            return new BlogDto
            {
                CategoryId = internalBlog.CategoryId,
                CategoryName = _redisRepository.ListRange<Category>(categoryRedisKey, 0, -1).FirstOrDefault(d => d.Id == internalBlog.CategoryId).Name,
                Content = internalBlog.Content,
                Id = internalBlog.Id,
                Title = internalBlog.Title,
                ViewCount = internalBlog.ViewCount,
                AddTime = internalBlog.AddTime,
                Comments = GetBlogComments(internalBlog.Id),
                Tags = GetBlogTag(internalBlog.Tags)
            };
        }

        public void AddViewCount(Guid blogId)
        {

            var blogViewCountRedisKey = $"NewBlogger:BlogsViewCount:BlogId:{blogId}";

            var value = _redisRepository.StringGet(blogViewCountRedisKey);

            if ((value + "").Length <= 0)
            {
                _redisRepository.StringSet(blogViewCountRedisKey, 1);
            }
            else
            {
                _redisRepository.StringIncrement(blogViewCountRedisKey);
            }
        }

        private IList<CommentDto> GetBlogComments(Guid blogId)
        {
            var commentBlogsRedisKey = $"NewBlogger:Comments:BlogId:{blogId}";

            var comments = _redisRepository.ListRange<Comment>(commentBlogsRedisKey, 0, -1);

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

            var tagRedisKey = "NewBlogger:Tags";

            var tags = _redisRepository.ListRange<Tag>(tagRedisKey).Where(w => tagIds.Contains(w.Id));

            return tags.Any() ? tags.Select(
                s => new TagDto
                {
                    AddTime = s.AddTime,
                    Id = s.Id,
                    Name = s.Name
                }).ToList() : new List<TagDto>();
        }

        public void AddNewBlog(String title, String content, Guid categoryId, params Guid[] tagIds)
        {
            var blog = new Blog(title, content, categoryId, tagIds);

            var blogRedisKey = "NewBlogger:Blogs";

            _redisRepository.ListRightPush(blogRedisKey, blog);

            var categoryBlogCountRedisKey = $"NewBlogger:CategoryBlogCount:Category:{categoryId}";

            var value = _redisRepository.StringGet(categoryBlogCountRedisKey);

            if ((value + "").Length <= 0)
            {
                _redisRepository.StringSet(categoryBlogCountRedisKey, 1);
            }
            else
            {
                _redisRepository.StringIncrement(categoryBlogCountRedisKey);
            }
        }
    }
}
