﻿using NewBlogger.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewBlogger.Dto;

namespace NewBlogger.Application.Interface
{
    public interface ICategoryService
    {
        IList<CategoryDto> GetCategorys();

        Task AddCategoryAsync(String categoryName);

        Task RemoveCategoryAsync(String categoryId);

        Task ModifyCategoryAsync(String categoryId,String newCategoryName);
    }
}