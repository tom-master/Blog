using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl;

namespace NewBlogger.Application
{
    public class TagService : ITagService
    {
        private readonly RedisRepositoryBase _redisRepository;

        public TagService(RedisRepositoryBase redisRepository)
        {
            _redisRepository = redisRepository;
        }

        /// <summary>
        /// 获取文章标签列表
        /// </summary>
        /// <returns></returns>
        public IList<TagDto> GetTags()
        {
            var tagRedisKey = "NewBlogger:Tags";

            return _redisRepository.ListRange<Tag>(tagRedisKey).Select(s => new TagDto
            {
                AddTime = s.AddTime,
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        /// <summary>
        /// 新增文章标签
        /// </summary>
        /// <param name="tagName"></param>
        public void AddTag(String tagName)
        {
            var tagRedisKey = "NewBlogger:Tags";

            _redisRepository.ListRightPush(tagRedisKey, new Tag(tagName));
        }

        /// <summary>
        /// 移除文章列表
        /// </summary>
        /// <param name="tagId"></param>
        public void RemoveTag(Guid tagId)
        {
            var categoryRedisKey = "NewBlogger:Tags";

            var category = _redisRepository.ListRange<Tag>(categoryRedisKey).FirstOrDefault(w => w.Id == tagId);

            _redisRepository.ListRemove(categoryRedisKey, category);
        }

    }
}
