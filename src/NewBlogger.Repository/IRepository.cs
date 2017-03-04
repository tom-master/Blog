using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Model;

namespace NewBlogger.Repository
{
    public interface IRepository<T> where T : ModelBase
    {
        IQueryable<T> Find(Expression<Func<T, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount);

        IQueryable<T> Find();

        Task AddAsync(T model);

        Task ModifyAsync(Expression<Func<T, Boolean>> filter, IEnumerable<Tuple<Object, Object>> fields);

        Task RemoveAsync(Guid modelId);
    }
}
