using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NewBlogger.Dto
{
    public class CategoryDto
    {
        public CategoryDto() { }


        public Guid Id { get; set; }

        public String Name { get; set; }

        public Int32 BlogCount { get; set; }
    }
}
