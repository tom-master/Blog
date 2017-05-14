using NewBlogger.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewBlogger.Dto;

namespace NewBlogger.Application.Interface
{
    public interface ICategoryService
    {
        IList<CategoryDto> GetCategorys();

        void AddCategory(String categoryName);

        void RemoveCategory(Guid categoryId);

        void ModifyCategory(Guid categoryId,String newCategoryName);
    }
}
