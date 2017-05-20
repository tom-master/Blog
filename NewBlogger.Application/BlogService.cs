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

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IList<BlogDto> GetBlogs(Guid categoryId, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            //Int32 internalStart = (pageIndex - 1) * pageSize, internalEnd = (pageSize + internalStart) - 1;

            var blogIdsRedisKey = $"NewBlogger:BlogIds:Id";

            var blogIds = _redisRepository.ListRange<Guid>(blogIdsRedisKey, 0, -1);

            var blogs = blogIds.Select(GetBlog).Where(w => categoryId == Guid.Empty || w.CategoryId == categoryId);

            totalCount = blogs.Count();

            return blogs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        }

        /// <summary>
        /// 获取文章
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public BlogDto GetBlog(Guid blogId)
        {

            if (blogId == Guid.Empty)
            {
                throw new ArgumentNullException($"{nameof(blogId)}");
            }

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

        /// <summary>
        /// 添加文章访问量
        /// </summary>
        /// <param name="blogId"></param>
        public void AddViewCount(Guid blogId)
        {
            if (blogId == Guid.Empty)
            {
                throw new ArgumentNullException($"{nameof(blogId)}");
            }

            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            _redisRepository.HashIncrement(blogRedisKey, "ViewCount");
        }


        /// <summary>
        /// 获取文章标签
        /// </summary>
        /// <param name="tagIds"></param>
        /// <returns></returns>
        private IList<TagDto> GetBlogTag(params Guid[] tagIds)
        {
            if (!tagIds.Any())
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

        /// <summary>
        /// 新增文章
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="categoryId"></param>
        /// <param name="tagIds"></param>
        public void AddNewBlog(String title, String content, Guid categoryId, params Guid[] tagIds)
        {
            if (String.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException($"{nameof(title)}");
            }

            if (String.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException($"{nameof(content)}");
            }

            if (categoryId==Guid.Empty)
            {
                throw new ArgumentNullException($"{nameof(categoryId)}");
            }

            if (!tagIds.Any())
            {
                throw new ArgumentNullException($"{nameof(tagIds)}");
            }

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

            if (!_redisRepository.KeyExists(categoryBlogCountRedisKey))
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
