using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Jacmazon_ECommerce.Data;
using System.Reflection;

namespace Jacmazon_ECommerce.Repositories
{
    public class CRUDRepository<TEntity, TContext> : ICRUDRepository<TEntity> 
        where TEntity : class
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public CRUDRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<int> CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            PropertyInfo keyProperty = typeof(TEntity).GetProperty("Id");
            return (int)keyProperty.GetValue(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            TEntity dbEntity = await GetDbEntity(entity);
            _context.Entry(dbEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            TEntity entity = await GetDbEntity(id);
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity?> FindByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        /// <summary>
        /// 驗證Entity並回傳DB Entity
        /// </summary>
        /// <param name="entity">實體物件</param>
        /// <returns>Db Entity</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<TEntity> GetDbEntity(TEntity entity)
        {
            //
            PropertyInfo? keyProperty = typeof(TEntity).GetProperty("Id");
            if (keyProperty == null)
            {
                throw new InvalidOperationException("Entity must have an Id property.");
            }

            int? key = (int?)keyProperty.GetValue(entity);
            if (key == null)
            {
                throw new InvalidOperationException("Id value cannot be null.");
            }

            TEntity? dbEntity = await _dbSet.FindAsync(key);
            if (dbEntity == null)
            {
                throw new InvalidOperationException($"Entity with Id {key} not found.");
            }

            return dbEntity;
        }

        /// <summary>
        /// 查詢Entity id並回傳DB Entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Db Entity</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<TEntity> GetDbEntity(int id)
        {
            TEntity? dbEntity = await _dbSet.FindAsync(id);
            if (dbEntity == null)
            {
                throw new InvalidOperationException($"Entity with Id {id} not found.");
            }

            return dbEntity;
        }
    }
}
