using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public class Comment : ModelBase
    {

        public Comment(Guid blogId, String content, Guid replyId = default(Guid))
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

            Id = Guid.NewGuid();

            AddTime = DateTime.Now;
        }

        public Guid BlogId { get; private set; }

        public String Content { get; private set; }

        public Guid ReplyId { get; private set; }

    }
}
