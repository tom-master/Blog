using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongodbServiceCore;
using NewBlogger.Model;

namespace NewBlogger.Repository
{
    public class MongodbRepository<T> : IRepository<T> where T : ModelBase
    {
        private readonly MongoServiceApi _mongoServiceApi = new MongoServiceApi();

        public virtual IQueryable<T> Find(Expression<Func<T, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            var dataSource = _mongoServiceApi.Find<T>().Where(filter);

            totalCount = dataSource.Count();

            return dataSource.Skip((pageIndex - 1) * pageSize).Take(pageSize);

        }

        public virtual IQueryable<T> Find()
        {
            return _mongoServiceApi.Find<T>();
        }

        public virtual async Task AddAsync(T model)
        {
            await _mongoServiceApi.AddAsync(model);
        }

        public virtual async Task ModifyAsync(Expression<Func<T, Boolean>> filter, IEnumerable<Tuple<Object, Object>> fields)
        {
            await _mongoServiceApi.UpdateOneAsync(filter, fields);
        }

        public virtual async Task RemoveAsync(Guid modelId)
        {
            await _mongoServiceApi.RemoveAsync<T>(m => m.Id == modelId);
        }

    }
}
