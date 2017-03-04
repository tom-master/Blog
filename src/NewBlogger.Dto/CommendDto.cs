using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Dto
{
    public class CommendDto
    {
        public CommendDto() { }


        public Guid BlogId { get; set; }

        public String Content { get; set; }

        public Guid ReplyId { get; set; }

        public DateTime AddTime { get; set; }
    }
}
