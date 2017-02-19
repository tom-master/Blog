using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public class Comment : ModelBase
    {

        public Comment(String blogId, String content, String replyId = default(String))
        {
            if ((blogId + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(blogId)} cannot be zero");
            }

            if ((content + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(content)} cannot be null");
            }

            BlogId = blogId;

            Content = content;

            ReplyId = replyId;

            Id = Guid.NewGuid().ToString();

            AddTime = DateTime.Now;
        }

        public String BlogId { get; private set; }

        public String Content { get; private set; }

        public String ReplyId { get; private set; }

    }
}
