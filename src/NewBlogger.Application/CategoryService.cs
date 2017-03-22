using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository;
using NewBlogger.Repository.Base;

namespace NewBlogger.Application
{
    public class CategoryService : ICategoryService
    {
        private readonly RepositoryBase<Category> _categoryRepository;

        private readonly RepositoryBase<Blog> _blogRepository;


        public CategoryService(RepositoryBase<Category> categoryRepository, RepositoryBase<Blog> blogRepository)
        {
            _categoryRepository = categoryRepository;

            _blogRepository = blogRepository;
        }

        public IList<CategoryDto> GetCategorys()
        {
            return _categoryRepository.Find().ToList().Select(s => new CategoryDto
            {
                BlogCount = _blogRepository.Find().Count(d => d.CategoryId == s.Id),
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public async Task AddCategoryAsync(String categoryName)
        {
            var category = new Category(categoryName);

            await _categoryRepository.AddAsync(category);
        }

        public async Task RemoveCategoryAsync(Guid categoryId)
        {
            await _categoryRepository.RemoveAsync(categoryId);
        }

        public async Task ModifyCategoryAsync(Guid categoryId, String newCategoryName)
        {
            IList<Tuple<Object, Object>> fields = new List<Tuple<Object, Object>>
            {
                new Tuple<Object, Object>("Name", newCategoryName)
            };

            await _categoryRepository.ModifyAsync(d => d.Id == categoryId, fields);
        }
    }
}
