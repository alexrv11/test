using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MS.N.Log.DAO
{
    public class MongoRepository
    {
        private readonly IMongoDatabase _db;

        public MongoRepository(IMongoDatabase db)
        {
            _db = db;
        }

        public void Delete<T>(System.Linq.Expressions.Expression<Func<T, bool>> expression) where T : class, new()
        {
            _db.GetCollection<T>(typeof(T).Name).DeleteManyAsync(expression);
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
        public void Add<T>(T item) where T : class, new()
        {
            _db.GetCollection<T>(typeof(T).Name).InsertOneAsync(item);
        }
        public void Add<T>(IEnumerable<T> items) where T : class, new()
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }
    }
}
