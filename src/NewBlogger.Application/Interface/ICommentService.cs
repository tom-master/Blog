using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Model;

namespace NewBlogger.Application.Interface
{
    public interface ICommentService
    {
        IList<Comment> GetComments(Expression<Func<Comment, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        Task AddCommentAsync(String blogId, String content, String replyId);
    }
}