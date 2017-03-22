using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NewBlogger.Model;

namespace NewBlogger.Repository.Base
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : ModelBase
    {

        public virtual IList<T> Find(Expression<Func<T, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            throw new NotImplementedException();
        }

        public virtual IList<T> Find(Expression<Func<T, Boolean>> filter = default(Expression<Func<T, Boolean>>))
        {
            throw new NotImplementedException();
        }

        public virtual async Task AddAsync(T model)
        {
            await Task.Run(() =>
              {
                  throw new NotImplementedException();
              });
        }

        public virtual async Task ModifyAsync(Expression<Func<T, Boolean>> filter, IEnumerable<Tuple<Object, Object>> fields)
        {
            await Task.Run(() =>
              {
                  throw new NotImplementedException();
              });
        }

        public virtual async Task RemoveAsync(Guid modelId)
        {
            await Task.Run(() =>
             {
                 throw new NotImplementedException();
             });
        }
    }
}
