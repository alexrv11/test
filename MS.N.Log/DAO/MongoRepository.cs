using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BGBA.MS.N.Log.DAO
{
    public class MongoRepository
    {
        private readonly IMongoDatabase _db;

        public MongoRepository(IConfiguration configuration)
        {
            _db = new MongoClient(configuration["MongoDB:ConnectionString"]).GetDatabase(configuration["MongoDB:DatabaseName"]);
        }

        public async Task Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            await _db.GetCollection<T>(typeof(T).Name).DeleteManyAsync(expression);
        }

        public void DeleteAll<T>() where T : class, new()
        {
            _db.DropCollection(typeof(T).Name);
        }
        public T Single<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            return All<T>().Where(expression).SingleOrDefault();
        }
        public IQueryable<T> All<T>() where T : class, new()
        {
            return _db.GetCollection<T>(typeof(T).Name).AsQueryable();
        }
        public async Task Add<T>(T item) where T : class, new()
        {
            await _db.GetCollection<T>(typeof(T).Name).InsertOneAsync(item);
        }
        public async Task Add<T>(IEnumerable<T> items) where T : class, new()
        {
            foreach (T item in items)
            {
                await Add(item);
            }
        }

        public async Task<List<T>> Find<T>(FilterDefinition<T> filterDefinition)
        {
            return await _db.GetCollection<T>(typeof(T).Name).Find(filterDefinition).ToListAsync();
        }
    }
}
