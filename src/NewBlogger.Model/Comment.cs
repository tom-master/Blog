using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewBlogger.Model
{
    public class Comment : ModelBase
    {

        public Comment(String replyNickName, String replyEmailAddress, Guid blogId, String content, Guid replyId = default(Guid))
        {

            if ((replyNickName + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(replyNickName)} cannot be null");
            }

            if ((replyEmailAddress + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(replyEmailAddress)} cannot be null");
            }

            if ((blogId + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(blogId)} cannot be zero");
            }

            if ((content + "").Length <= 0)
            {
                throw new ArgumentNullException($"{nameof(content)} cannot be null");
            }

            ReplyEmailAddress = replyEmailAddress;

            ReplyNickName = replyNickName;

            BlogId = blogId;

            Content = content;

            ReplyId = replyId;

            Id = Guid.NewGuid();

            AddTime = DateTime.Now;
        }


        public String ReplyNickName { get; set; }

        public String ReplyEmailAddress { get; set; }

        public Guid BlogId { get; private set; }

        public String Content { get; private set; }

        public Guid ReplyId { get; private set; }

    }
}
