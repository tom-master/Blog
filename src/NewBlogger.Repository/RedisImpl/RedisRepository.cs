using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NewBlogger.Model;
using NewBlogger.Repository.Base;

namespace NewBlogger.Repository.RedisImpl
{
    public class RedisRepository<T> : RepositoryBase<T> where T : ModelBase
    {
        public override IList<T> Find(Expression<Func<T, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            totalCount = 0;

            return default(IList<T>);
        }

        public override IList<T> Find(Expression<Func<T, Boolean>> filter = default(Expression<Func<T, Boolean>>))
        {
            return default(IList<T>);
        }

        public override async Task AddAsync(T model)
        {
        }

        public override async Task ModifyAsync(Expression<Func<T, Boolean>> filter, IEnumerable<Tuple<Object, Object>> fields)
        {
        }

        public override async Task RemoveAsync(Guid modelId)
        {
        }
    }
}
