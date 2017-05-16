using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ICommentService _commentService;

        public BlogService(RedisRepositoryBase redisRepository, ICommentService commentService)
        {
            _redisRepository = redisRepository;

            _commentService = commentService;
        }

        public IList<BlogDto> GetBlogs(Guid categoryId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            Int32 internalStart = (pageIndex - 1) * pageSize, internalEnd = (pageSize + internalStart) - 1;

            var blogIdsRedisKey = $"NewBlogger:BlogIds:Id";

            var blogIds = _redisRepository.ListRange<Guid>(blogIdsRedisKey, internalStart, internalEnd);

            totalCount = (Int32)_redisRepository.ListLength(blogIdsRedisKey);

            return blogIds.Select(GetBlog).Where(w=>categoryId==Guid.Empty?true:w.CategoryId==categoryId).ToList();

        }

        public BlogDto GetBlog(Guid blogId)
        {
            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            var blog = _redisRepository.HashGet<Blog>(blogRedisKey).FirstOrDefault();

            var categoryRedisKey = "NewBlogger:Categorys";

            return new BlogDto
            {
                CategoryId = blog.CategoryId,
                CategoryName = _redisRepository.ListRange<Category>(categoryRedisKey, 0, -1).FirstOrDefault(d => d.Id == blog.CategoryId).Name,
                Content = blog.Content,
                Id = blog.Id,
                Title = blog.Title,
                ViewCount = blog.ViewCount,
                AddTime = DateTime.Parse(blog.AddTime.ToString("yyyy-MM-dd HH:mm:ss")),
                Comments = _commentService.GetComments(blog.Id),
                Tags = GetBlogTag(blog.Tags),
                CommentCount = blog.CommentCount
            };
        }

        public void AddViewCount(Guid blogId)
        {
            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            _redisRepository.HashIncrement(blogRedisKey, "ViewCount");
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
                new HashEntry(nameof(blog.ViewCount),0),
                new HashEntry(nameof(blog.CommentCount),0)
            }.ToArray());

            var blogIdsRedisKey = $"NewBlogger:BlogIds:Id";

            _redisRepository.ListRightPush(blogIdsRedisKey, blog.Id);

            var categoryBlogCountRedisKey = $"NewBlogger:CategoryBlogCount:Category:{categoryId}";

            if(!_redisRepository.KeyExists(categoryBlogCountRedisKey))
            {
                _redisRepository.StringSet(categoryBlogCountRedisKey,1);
            }
            else
            {
                _redisRepository.StringIncrement(categoryBlogCountRedisKey);
            }
        }
    }
}
