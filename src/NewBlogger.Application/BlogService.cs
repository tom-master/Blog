using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl;
using Newtonsoft.Json;
using StackExchange.Redis;

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

            var blogIdsRedisKey = "NewBlogger:BlogIds:Id";

            var blogIds = _redisRepository.ListRange<Guid>(blogIdsRedisKey, internalStart, internalEnd);

            totalCount = (Int32)_redisRepository.ListLength(blogIdsRedisKey);

            return blogIds.Select(GetBlog).ToList();

        }

        public BlogDto GetBlog(Guid blogId)
        {
            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            var internalBlog = _redisRepository.HashGet<Blog>(blogRedisKey, new RedisValue[0]).FirstOrDefault();

            var categoryRedisKey = "NewBlogger:Categorys";

            return new BlogDto
            {
                CategoryId = internalBlog.CategoryId,
                CategoryName = _redisRepository.ListRange<Category>(categoryRedisKey, 0, -1).FirstOrDefault(d => d.Id == internalBlog.CategoryId).Name,
                Content = internalBlog.Content,
                Id = internalBlog.Id,
                Title = internalBlog.Title,
                ViewCount = internalBlog.ViewCount,
                AddTime = DateTime.Parse(internalBlog.AddTime.ToString("yyyy-MM-dd HH:mm:ss")),
                Comments = GetBlogComments(internalBlog.Id),
                Tags = GetBlogTag(internalBlog.Tags),
                CommentCount = 0
            };
        }

        public void AddViewCount(Guid blogId)
        {
            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            _redisRepository.HashIncrement(blogRedisKey, "ViewCount");
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

            var blogRedisKey = $"NewBlogger:Blogs:Id:{blog.Id}";

            _redisRepository.HashSet(blogRedisKey, new List<HashEntry>
            {
                new HashEntry(nameof(blog.Id),$"{blog.Id}"),

                new HashEntry(nameof(blog.Title),blog.Title),

                new HashEntry(nameof(blog.Content),blog.Content),

                new HashEntry(nameof(blog.CategoryId),$"{blog.CategoryId}"),

                new HashEntry(nameof(blog.AddTime),$"{blog.AddTime}"),

                new HashEntry(nameof(blog.Tags),JsonConvert.SerializeObject(blog.Tags)),

                new HashEntry(nameof(blog.ViewCount),0)
            }.ToArray());

            var blogIdsRedisKey = "NewBlogger:BlogIds:Id";

            _redisRepository.ListRightPush(blogIdsRedisKey, blog.Id);
        }
    }
}
