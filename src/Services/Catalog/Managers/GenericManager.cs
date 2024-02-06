using System;
using System.Reflection;
using System.Threading.Tasks;
using Catalog.Managers.Interfaces;
using MongoDB.Driver;

namespace Catalog.Managers
{
    public class GenericManager<T> : IGenericManager<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly MongoDbContext _context;
        public GenericManager(MongoDbContext context)
        {
            _collection = context.Database.GetCollection<T>(typeof(T).Name.ToLower());
            _context = context;
        }

        public async Task CreateEntity(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<bool> DeleteEntity(string id)
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("The entity does not have an 'Id' property.");
            }

            var filter = Builders<T>.Filter.Eq("Id", id);
            var deleteResult = await _collection.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> UpdateEntity(T entity)
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("The entity does not have an 'Id' property.");
            }

            var entityId = idProperty.GetValue(entity);
            if (entityId == null)
            {
                throw new InvalidOperationException("The 'Id' property value is null.");
            }

            var filter = Builders<T>.Filter.Eq("Id", entityId);
            var updateResult = await _collection.ReplaceOneAsync(filter, entity);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

    }
}
