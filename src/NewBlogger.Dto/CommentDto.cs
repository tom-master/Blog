using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Dto
{
    public class CommentDto
    {
        public CommentDto() { }


        public String ReplyNickName { get; set; }

        public String ReplyEmailAddress { get; set; }

        public Guid BlogId { get; set; }

        public String Content { get; set; }

        public Guid ReplyId { get; set; }

        public DateTime AddTime { get; set; }
    }
}
