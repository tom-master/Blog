using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewBlogger.Model;

namespace NewBlogger.Application.Interface
{
    public interface ICommentService
    {
        IList<Comment> GetComments(Guid blogId);

        void AddComment(String nickName, String emailAddress, Guid blogId, String content, Guid? replyId = default(Guid?));
    }
}