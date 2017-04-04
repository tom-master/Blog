using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Application.Interface;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl;

namespace NewBlogger.Application
{
    public class CommentService : ICommentService
    {
        private readonly RedisRepositoryBase _redisRepository;

        public CommentService(RedisRepositoryBase redisRepository)
        {
            _redisRepository = redisRepository;
        }


        public IList<Comment> GetComments(String key, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {

            Int32 internalStart = (pageIndex - 1) * pageSize, internalEnd = (pageSize + internalStart) - 1;

            totalCount = (Int32)_redisRepository.ListLength(key);

            return _redisRepository.ListRange<Comment>(key, internalStart, internalEnd);
        }

        public async Task AddCommentAsync(String nickName, String emailAddress, Guid blogId, String content, Guid? replyId = default(Guid?))
        {
            var comment = new Comment(nickName, emailAddress, blogId, content, replyId);

            var commentBlogRedisKey = $"NewBlogger:Comments:BlogId:{blogId}";

            await _redisRepository.ListRightPushAsync(commentBlogRedisKey, comment);

            var commentBlogCountRedisKey = $"NewBlogger:CommentBlogCount:BlogId:{blogId}";

            var value = _redisRepository.StringGet(commentBlogCountRedisKey);

            if ((value + "").Length <= 0)
            {
                _redisRepository.StringSet(commentBlogCountRedisKey, 1);
            }
            else
            {
                _redisRepository.StringIncrement(commentBlogCountRedisKey);
            }
        }
    }
}
