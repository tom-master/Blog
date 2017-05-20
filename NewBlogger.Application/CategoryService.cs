using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl;

namespace NewBlogger.Application
{
    public class CategoryService : ICategoryService
    {

        private readonly RedisRepositoryBase _redisRepository;

        public CategoryService(RedisRepositoryBase redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public IList<CategoryDto> GetCategorys()
        {
            var categoryRedisKey = "NewBlogger:Categorys";

            var categoryBlogCountRedisKey = "NewBlogger:CategoryBlogCount:Category:";

            return _redisRepository.ListRange<Category>(categoryRedisKey).Select(s => new CategoryDto
            {
                BlogCount = !String.IsNullOrEmpty(_redisRepository.StringGet(categoryBlogCountRedisKey + s.Id)) ? Int32.Parse(_redisRepository.StringGet(categoryBlogCountRedisKey + s.Id)) : 0,
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public void AddCategory(String categoryName)
        {
            if (String.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentNullException($"{nameof(categoryName)}");
            }

            var category = new Category(categoryName);

            var categoryRedisKey = "NewBlogger:Categorys";

            _redisRepository.ListRightPush(categoryRedisKey, category);
        }

        public void RemoveCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentNullException($"{nameof(categoryId)}");
            }

            var categoryRedisKey = "NewBlogger:Categorys";

            var category = _redisRepository.ListRange<Category>(categoryRedisKey).FirstOrDefault(w => w.Id == categoryId);

            _redisRepository.ListRemove(categoryRedisKey, category);
        }

        public void ModifyCategory(Guid categoryId, String newCategoryName)
        {
            if (categoryId==Guid.Empty)
            {
                throw new ArgumentNullException($"{categoryId}");
            }

            if (String.IsNullOrEmpty(newCategoryName))
            {
                throw new ArgumentNullException($"{newCategoryName}")
            }

            RemoveCategory(categoryId);

            AddCategory(newCategoryName);
        }
    }
}
