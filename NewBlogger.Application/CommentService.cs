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

        /// <summary>
        /// 获取文章回复列表
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public IList<CommentDto> GetComments(Guid blogId)
        {
            if (blogId==Guid.Empty)
            {
                throw new ArgumentNullException($"{blogId}");
            }

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

        /// <summary>
        /// 添加文章回复
        /// </summary>
        /// <param name="nickName"></param>
        /// <param name="emailAddress"></param>
        /// <param name="blogId"></param>
        /// <param name="content"></param>
        /// <param name="replyId"></param>
        public void AddComment(String nickName, String emailAddress, Guid blogId, String content, Guid replyId = default(Guid))
        {
            if (String.IsNullOrEmpty(nickName))
            {
                throw new ArgumentNullException($"{nickName}");
            }

            if (String.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException($"{emailAddress}");
            }

            if (String.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException($"{content}");
            }

            if (blogId==Guid.Empty)
            {
                throw new ArgumentNullException($"{blogId}");
            }


            var comment = new Comment(nickName, emailAddress, blogId, content, replyId);

            var commentBlogRedisKey = $"NewBlogger:Comments:BlogId:{blogId}";

            _redisRepository.ListRightPush(commentBlogRedisKey,comment);

            var blogRedisKey = $"NewBlogger:Blogs:Id:{blogId}";

            _redisRepository.HashIncrement(blogRedisKey, "CommentCount");
        }
    }
}
