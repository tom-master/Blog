using System;
using System.Collections.Generic;
using System.Text;

namespace NewBlogger.Dto
{
    public class TagDto
    {
        public Guid Id { get; set; }

        public DateTime AddTime { get; set; }

        public String Name { get; set; }
    }
}
