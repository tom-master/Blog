using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NewBlogger.Model;
using NewBlogger.Repository;
using NewBlogger.Repository.Base;
using Xunit;

namespace NewBlogger.Test
{
    public class Class1
    {

        public Class1()
        {

        }

        [Fact]
        public async void AddCategoryAsync()
        {
            var category1 = new Category("test1");

            var a = Directory.GetCurrentDirectory();

            RepositoryBase<Category> categoryRepository = new MongodbRepository<Category>();

            await categoryRepository.AddAsync(category1);
        }

        [Fact]
        public async Task AddBlogAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                var blog = new Blog("title1", @"Donec mattis, purus nec placerat bibendum, dui pede condimentum odio, 
    ac blandit ante orci ut diam. Cras fringilla magna. Phasellus suscipit, leo a pharetra condimentum, 
    lorem tellus eleifend magna, eget fringilla velit magna id neque. Curabitur vel urna. In tristique orci
    porttitor ipsum. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Donec libero.
    Suspendisse bibendum. Cras id urna. Morbi tincidunt, orci ac convallis aliquam.",Guid.Parse("f7e371e3-9ad2-4ef9-9c6c-559409b67840"));

                RepositoryBase<Blog> blogRepository = new MongodbRepository<Blog>();

                await blogRepository.AddAsync(blog);
            }
        }
    }
}
