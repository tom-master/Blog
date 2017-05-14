using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NewBlogger.Model;
using NewBlogger.Repository.RedisImpl.InternalRedisHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;

namespace NewBlogger.Repository.RedisImpl
{
    public abstract class RedisRepositoryBase : IRedisRepository
    {
        private Int32 DbNum { get; }
        private readonly ConnectionMultiplexer _conn;

        #region 构造函数

        protected RedisRepositoryBase() : this(0, null)
        {

        }

        protected RedisRepositoryBase(Int32 dbNum = 0)
                : this(dbNum, null)
        {
        }

        protected RedisRepositoryBase(Int32 dbNum, String readWriteHosts)
        {
            DbNum = dbNum;
            _conn =
                String.IsNullOrWhiteSpace(readWriteHosts) ?
                RedisConnectionHelp.Instance :
                RedisConnectionHelp.GetConnectionMultiplexer(readWriteHosts);
        }

        #endregion 构造函数

        #region String

        #region 同步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public virtual Boolean StringSet(String key, String value, TimeSpan? expiry = default(TimeSpan?))
        {

            return Execute(db => db.StringSet(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public virtual Boolean StringSet(IEnumerable<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {

            var newkeyValues =
                keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(p.Key, p.Value)).ToList();
            return Execute(db => db.StringSet(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public virtual Boolean StringSet<TModel>(String key, TModel obj, TimeSpan? expiry = default(TimeSpan?))
        {

            var json = ConvertJson(obj);
            return Execute(db => db.StringSet(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public virtual String StringGet(String key)
        {

            return Execute(db => db.StringGet(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public virtual RedisValue[] StringGet(IEnumerable<String> listKey)
        {
            return Execute(db => db.StringGet(ConvertRedisKeys(listKey.ToList())));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TModel StringGet<TModel>(String key)
        {

            return Execute(db => ConvertObj<TModel>(db.StringGet(key)));
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public virtual Double StringIncrement(String key, Double val = 1)
        {

            return Execute(db => db.StringIncrement(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public virtual Double StringDecrement(String key, Double val = 1)
        {

            return Execute(db => db.StringDecrement(key, val));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public virtual async Task<Boolean> StringSetAsync(String key, String value, TimeSpan? expiry = default(TimeSpan?))
        {

            return await Execute(db => db.StringSetAsync(key, value, expiry));
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public virtual async Task<Boolean> StringSetAsync(IEnumerable<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            var newkeyValues = keyValues.Select(p => new KeyValuePair<RedisKey, RedisValue>(p.Key, p.Value)).ToList();

            return await Execute(db => db.StringSetAsync(newkeyValues.ToArray()));
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public virtual async Task<Boolean> StringSetAsync<TModel>(String key, TModel obj, TimeSpan? expiry = default(TimeSpan?))
        {
            var json = ConvertJson(obj);

            return await Execute(db => db.StringSetAsync(key, json, expiry));
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <returns></returns>
        public virtual async Task<String> StringGetAsync(String key)
        {

            return await Execute(db => db.StringGetAsync(key));
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public virtual async Task<RedisValue[]> StringGetAsync(IEnumerable<String> listKey)
        {
            return await Execute(db => db.StringGetAsync(ConvertRedisKeys(listKey.ToList())));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TModel> StringGetAsync<TModel>(String key)
        {

            String result = await Execute(db => db.StringGetAsync(key));
            return ConvertObj<TModel>(result);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public virtual async Task<Double> StringIncrementAsync(String key, Double val = 1)
        {

            return await Execute(db => db.StringIncrementAsync(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public virtual async Task<Double> StringDecrementAsync(String key, Double val = 1)
        {

            return await Execute(db => db.StringDecrementAsync(key, val));
        }

        #endregion 异步方法

        #endregion String

        #region Hash

        #region 同步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual Boolean HashExists(String key, String dataKey)
        {

            return Execute(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual Boolean HashSet<TModel>(String key, String dataKey, TModel t)
        {

            return Execute(db =>
            {
                var json = ConvertJson(t);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        public virtual Boolean HashSet(String key, String dataKey, String dataValue)
        {
            return Execute(db => db.HashSet(key, dataKey, dataValue));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashEntries"></param>
        /// <returns></returns>
        public virtual void HashSet(String key, HashEntry[] hashEntries)
        {
            Execute(db =>
            {
                db.HashSet(key, hashEntries);
                return true;
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual Boolean HashDelete(String key, String dataKey)
        {

            return Execute(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public virtual Int64 HashDelete(String key, IEnumerable<RedisValue> dataKeys)
        {

            return Execute(db => db.HashDelete(key, dataKeys.ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual TModel HashGet<TModel>(String key, String dataKey)
        {
            return Execute(db =>
            {
                String value = db.HashGet(key, dataKey);
                return ConvertObj<TModel>(value);
            });
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual IList<TModel> HashGet<TModel>(String key)
        {
            return Execute(db =>
             {
                 HashEntry[] value = db.HashGetAll(key);

                 var modelInstance = Activator.CreateInstance(typeof(TModel));

                 var modelPropertys = modelInstance.GetType().GetProperties();

                 foreach (var propertyInfo in modelPropertys)
                 {
                    var hashEntry= value.FirstOrDefault(w=>w.Name==propertyInfo.Name);
                    
                    if(hashEntry.Name.IsNull)
                    {
                        continue;
                    }

                    if(propertyInfo.PropertyType==typeof(DateTime))
                    {
                        propertyInfo.SetValue(modelInstance,DateTime.Parse(hashEntry.Value.ToString()));
                    }
                    else if(propertyInfo.PropertyType==typeof(Guid))
                    {
                        propertyInfo.SetValue(modelInstance,Guid.Parse(hashEntry.Value.ToString()));
                    }
                    else if(propertyInfo.PropertyType==typeof(Guid[]))
                    {
                        Guid[] guids= JsonConvert.DeserializeObject<Guid[]>(hashEntry.Value.ToString());
                        propertyInfo.SetValue(modelInstance,guids);
                    }
                    else if(propertyInfo.PropertyType==typeof(Int32))
                    {
                        propertyInfo.SetValue(modelInstance,Int32.Parse(hashEntry.Value.ToString()));
                    }
                    else 
                    {
                        propertyInfo.SetValue(modelInstance,hashEntry.Value.ToString());
                    }
                 }

                 return new List<TModel>
                 {
                     ((TModel)modelInstance)
                 };
             });
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public virtual Double HashIncrement(String key, String dataKey, Double val = 1)
        {

            return Execute(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public virtual Double HashDecrement(String key, String dataKey, Double val = 1)
        {

            return Execute(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual List<TModel> HashKeys<TModel>(String key)
        {

            return Execute(db =>
            {
                var values = db.HashKeys(key);
                return ConvetList<TModel>(values).ToList();
            });
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual async Task<Boolean> HashExistsAsync(String key, String dataKey)
        {

            return await Execute(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<Boolean> HashSetAsync<TModel>(String key, String dataKey, TModel t)
        {

            return await Execute(db =>
            {
                var json = ConvertJson(t);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual async Task<Boolean> HashDeleteAsync(String key, String dataKey)
        {

            return await Execute(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public virtual async Task<Int64> HashDeleteAsync(String key, IEnumerable<RedisValue> dataKeys)
        {

            //List<RedisValue> dataKeys1 = new List<RedisValue>() {"1","2"};
            return await Execute(db => db.HashDeleteAsync(key, dataKeys.ToArray()));
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual async Task<TModel> HashGeAsync<TModel>(String key, String dataKey)
        {

            String value = await Execute(db => db.HashGetAsync(key, dataKey));
            return ConvertObj<TModel>(value);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public virtual async Task<Double> HashIncrementAsync(String key, String dataKey, Double val = 1)
        {

            return await Execute(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public virtual async Task<Double> HashDecrementAsync(String key, String dataKey, Double val = 1)
        {

            return await Execute(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<List<TModel>> HashKeysAsync<TModel>(String key)
        {

            var values = await Execute(db => db.HashKeysAsync(key));
            return ConvetList<TModel>(values).ToList();
        }

        #endregion 异步方法

        #endregion Hash

        #region List

        #region 同步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void ListRemove<TModel>(String key, TModel value)
        {

            Execute(db => db.ListRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual List<TModel> ListRange<TModel>(String key)
        {

            return Execute(redis =>
            {
                var values = redis.ListRange(key);
                return ConvetList<TModel>(values).ToList();
            });
        }

        public virtual List<TModel> ListRange<TModel>(String key, Int32 start, Int32 end)
        {

            return Execute(redis =>
            {
                var values = redis.ListRange(key, start, end);
                return ConvetList<TModel>(values).ToList();
            });
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void ListRightPush<TModel>(String key, TModel value)
        {

            Execute(db => db.ListRightPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TModel ListRightPop<TModel>(String key)
        {

            return Execute(db =>
            {
                var value = db.ListRightPop(key);
                return ConvertObj<TModel>(value);
            });
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void ListLeftPush<TModel>(String key, TModel value)
        {

            Execute(db => db.ListLeftPush(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TModel ListLeftPop<TModel>(String key)
        {

            return Execute(db =>
            {
                var value = db.ListLeftPop(key);
                return ConvertObj<TModel>(value);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Int64 ListLength(String key)
        {

            return Execute(redis => redis.ListLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual async Task<Int64> ListRemoveAsync<TModel>(String key, TModel value)
        {

            return await Execute(db => db.ListRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<List<TModel>> ListRangeAsync<TModel>(String key)
        {
            var values = await Execute(redis => redis.ListRangeAsync(key));
            return ConvetList<TModel>(values).ToList();
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual async Task<Int64> ListRightPushAsync<TModel>(String key, TModel value)
        {

            return await Execute(db => db.ListRightPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TModel> ListRightPopAsync<TModel>(String key)
        {

            var value = await Execute(db => db.ListRightPopAsync(key));
            return ConvertObj<TModel>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual async Task<Int64> ListLeftPushAsync<TModel>(String key, TModel value)
        {

            return await Execute(db => db.ListLeftPushAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TModel> ListLeftPopAsync<TModel>(String key)
        {

            var value = await Execute(db => db.ListLeftPopAsync(key));
            return ConvertObj<TModel>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<Int64> ListLengthAsync(String key)
        {

            return await Execute(redis => redis.ListLengthAsync(key));
        }

        #endregion 异步方法

        #endregion List

        #region SortedSet 有序集合

        #region 同步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public virtual Boolean SortedSetAdd<TModel>(String key, TModel value, Double score)
        {

            return Execute(redis => redis.SortedSetAdd(key, ConvertJson(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual Boolean SortedSetRemove<TModel>(String key, TModel value)
        {

            return Execute(redis => redis.SortedSetRemove(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual List<TModel> SortedSetRangeByRank<TModel>(String key)
        {

            return Execute(redis =>
            {
                var values = redis.SortedSetRangeByRank(key);
                return ConvetList<TModel>(values).ToList();
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual Int64 SortedSetLength(String key)
        {

            return Execute(redis => redis.SortedSetLength(key));
        }

        #endregion 同步方法

        #region 异步方法

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public virtual async Task<Boolean> SortedSetAddAsync<TModel>(String key, TModel value, Double score)
        {

            return await Execute(redis => redis.SortedSetAddAsync(key, ConvertJson(value), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual async Task<Boolean> SortedSetRemoveAsync<TModel>(String key, TModel value)
        {

            return await Execute(redis => redis.SortedSetRemoveAsync(key, ConvertJson(value)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<List<TModel>> SortedSetRangeByRankAsync<TModel>(String key)
        {
            var values = await Execute(redis => redis.SortedSetRangeByRankAsync(key));
            return ConvetList<TModel>(values).ToList();
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<Int64> SortedSetLengthAsync(String key)
        {

            return await Execute(redis => redis.SortedSetLengthAsync(key));
        }

        #endregion 异步方法

        #endregion SortedSet 有序集合

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public virtual Boolean KeyDelete(String key)
        {

            return Execute(db => db.KeyDelete(key));
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public virtual Int64 KeyDelete(List<String> keys)
        {
            return Execute(db => db.KeyDelete(ConvertRedisKeys(keys)));
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public virtual Boolean KeyExists(String key)
        {

            return Execute(db => db.KeyExists(key));
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public virtual Boolean KeyRename(String key, String newKey)
        {

            return Execute(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public virtual Boolean KeyExpire(String key, TimeSpan? expiry = default(TimeSpan?))
        {

            return Execute(db => db.KeyExpire(key, expiry));
        }

        #endregion key

        #region 发布订阅

        /// <summary>
        /// Redis发布订阅  订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public virtual void Subscribe(String subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            var sub = _conn.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// Redis发布订阅  发布
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual Int64 Publish<TModel>(String channel, TModel msg)
        {
            var sub = _conn.GetSubscriber();
            return sub.Publish(channel, ConvertJson(msg));
        }

        /// <summary>
        /// Redis发布订阅  取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public virtual void Unsubscribe(String channel)
        {
            var sub = _conn.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// Redis发布订阅  取消全部订阅
        /// </summary>
        public virtual void UnsubscribeAll()
        {
            var sub = _conn.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion 发布订阅

        #region 其他

        public virtual ITransaction CreateTransaction()
        {
            return GetDatabase().CreateTransaction();
        }

        public virtual IDatabase GetDatabase()
        {
            return _conn.GetDatabase(DbNum);
        }

        public virtual IServer GetServer(String hostAndPort)
        {
            return _conn.GetServer(hostAndPort);
        }



        #endregion 其他

        #region 辅助方法

        protected virtual TModel Execute<TModel>(Func<IDatabase, TModel> func)
        {
            var database = _conn.GetDatabase(DbNum);

            return func(database);
        }



        private String ConvertJson<TModel>(TModel value)
        {
            var result = value is String ? value.ToString() : JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return result;
        }

        private TModel ConvertObj<TModel>(RedisValue value)
        {
            if (value.IsNull)
            {
                return default(TModel);
            }
            return JsonConvert.DeserializeObject<TModel>(value,new JsonSerializerSettings
            {
                ContractResolver=new PrivateSetterContractResolver()
            });
        }

        private IList<TModel> ConvetList<TModel>(RedisValue[] values)
        {
            var result = new List<TModel>();
            foreach (var item in values)
            {
                var model = ConvertObj<TModel>(item);
                result.Add(model);
            }
            return result;
        }
        private RedisKey[] ConvertRedisKeys(List<String> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }

        protected RedisType GetKeyType(String key)
        {


            var keyType = Execute(db => db.KeyType(key));

            return keyType;
        }



        #endregion 辅助方法


    }
}
