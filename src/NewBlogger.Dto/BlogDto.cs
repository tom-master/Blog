using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Dto
{
    public class BlogDto
    {
        public BlogDto()
        {
        }

        public Guid Id { get; set; }

        public String Title { get; set; }

        public String Content { get; set; }

        public Guid CategoryId { get; set; }

        public Int32 ViewCount { get; set; }

        public Int32 CommentCount { get; set; }

        public String CategoryName { get; set; }

        public DateTime AddTime { get; set; }
    }
}
