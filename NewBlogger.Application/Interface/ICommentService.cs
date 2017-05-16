using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewBlogger.Dto;
using NewBlogger.Model;

namespace NewBlogger.Application.Interface
{
    public interface ICommentService
    {
        IList<CommentDto> GetComments(Guid blogId);

        void AddComment(String nickName, String emailAddress, Guid blogId, String content, Guid replyId = default(Guid));
    }
}