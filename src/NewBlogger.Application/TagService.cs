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

        public async Task AddTagAsync(String tagName)
        {
            var tagRedisKey = "NewBlogger:Tags";

            await _redisRepository.ListRightPushAsync(tagRedisKey, new Tag(tagName));
        }


        public async Task RemoveTagAsync(Guid tagId)
        {
            var categoryRedisKey = "NewBlogger:Tags";

            var category = _redisRepository.ListRange<Tag>(categoryRedisKey).FirstOrDefault(w => w.Id == tagId);

            await _redisRepository.ListRemoveAsync(categoryRedisKey, category);
        }

    }
}
