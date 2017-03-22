using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongodbServiceCore;
using NewBlogger.Model;
using NewBlogger.Repository.Base;

namespace NewBlogger.Repository.MongodbImpl
{
    public class MongodbRepository<T> : RepositoryBase<T> where T : ModelBase
    {
        private readonly MongoServiceApi _mongoServiceApi = new MongoServiceApi();

        public override IList<T> Find(Expression<Func<T, Boolean>> filter, Int32 pageIndex, Int32 pageSize, out Int32 totalCount)
        {
            var dataSource = _mongoServiceApi.Find<T>().Where(filter);

            totalCount = dataSource.Count();

            return dataSource.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        }

        public override IList<T> Find(Expression<Func<T, Boolean>> filter = default(Expression<Func<T, Boolean>>))
        {
            return filter == default(Expression<Func<T, Boolean>>) ? _mongoServiceApi.Find<T>().ToList() : _mongoServiceApi.Find<T>().Where(filter).ToList();
        }

        public override async Task AddAsync(T model)
        {
            await _mongoServiceApi.AddAsync(model);
        }

        public override async Task ModifyAsync(Expression<Func<T, Boolean>> filter, IEnumerable<Tuple<Object, Object>> fields)
        {
            await _mongoServiceApi.UpdateOneAsync(filter, fields);
        }

        public override async Task RemoveAsync(Guid modelId)
        {
            await _mongoServiceApi.RemoveAsync<T>(m => m.Id == modelId);
        }

    }
}
