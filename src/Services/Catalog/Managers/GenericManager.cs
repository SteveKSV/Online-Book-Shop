using System;
using System.Reflection;
using System.Threading.Tasks;
using Catalog.Managers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Managers
{
    public class GenericManager<T> : IGenericManager<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericManager(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task CreateEntity(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteEntity(Guid id)
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("The entity does not have an 'Id' property.");

            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEntity(T entity)
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("The entity does not have an 'Id' property.");

            var entityId = idProperty.GetValue(entity);
            if (entityId == null)
                throw new InvalidOperationException("The 'Id' property value is null.");

            var existing = await _dbSet.FindAsync(entityId);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
