using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Model;
using NewBlogger.Repository;

namespace NewBlogger.Application
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;

        public CommentService(IRepository<Comment> commentRepository)
        {
            _commentRepository = commentRepository;
        }


        public IList<Comment> GetComments(Expression<Func<Comment, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            return _commentRepository.Find(filter, pageIndex, pageSize, out totalCount).ToList();
        }

        public async Task AddCommentAsync(String nickName, String emailAddress, Guid blogId, String content, Guid? replyId = default(Guid?))
        {
            var comment = new Comment(nickName,emailAddress,blogId, content, replyId);

            await _commentRepository.AddAsync(comment);
        }
    }
}
