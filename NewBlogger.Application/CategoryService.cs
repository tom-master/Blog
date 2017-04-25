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
                BlogCount =Int32.Parse(_redisRepository.StringGet(categoryBlogCountRedisKey + s.Id)), 
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public async Task AddCategoryAsync(String categoryName)
        {
            var category = new Category(categoryName);

            var categoryRedisKey = "NewBlogger:Categorys";

            await _redisRepository.ListRightPushAsync(categoryRedisKey, category);
        }

        public async Task RemoveCategoryAsync(Guid categoryId)
        {
            var categoryRedisKey = "NewBlogger:Categorys";

            var category = _redisRepository.ListRange<Category>(categoryRedisKey).FirstOrDefault(w => w.Id == categoryId);

            await _redisRepository.ListRemoveAsync(categoryRedisKey, category);
        }

        public async Task ModifyCategoryAsync(Guid categoryId, String newCategoryName)
        {
            await RemoveCategoryAsync(categoryId);

            await AddCategoryAsync(newCategoryName);
        }
    }
}
