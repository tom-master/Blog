using System;
using System.Collections.Generic;
using System.Linq;
using NewBlogger.Application.Interface;
using NewBlogger.Dto;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl;
using StackExchange.Redis;

namespace NewBlogger.Application
{
    public class CommentService : ICommentService
    {
        private readonly RedisRepositoryBase _redisRepository;

        public CommentService(RedisRepositoryBase redisRepository)
        {
            _redisRepository = redisRepository;
        }


        public IList<CommentDto> GetComments(Guid blogId)
        {
            var commentBlogRedisKey = $"NewBlogger:Comments:BlogId:{blogId}";

            return _redisRepository.ListRange<Comment>(commentBlogRedisKey,0,-1).Select(comment => new CommentDto
            {
                Id = comment.Id,
                ReplyNickName = comment.ReplyNickName,
                ReplyEmailAddress = comment.ReplyEmailAddress,
                Content = comment.Content,
                BlogId = comment.BlogId,
                AddTime = comment.AddTime
            }).ToList();
        }

        public void AddComment(String nickName, String emailAddress, Guid blogId, String content, Guid replyId = default(Guid))
        {
            var comment = new Comment(nickName, emailAddress, blogId, content, replyId);

            var commentBlogRedisKey = $"NewBlogger:Comments:BlogId:{blogId}";

            _redisRepository.ListRightPush(commentBlogRedisKey,comment);

            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            _redisRepository.HashIncrement(blogRedisKey, "CommentCount");
        }
    }
}
