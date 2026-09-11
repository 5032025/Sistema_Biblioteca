using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public abstract class RepositorioBase<TEntity> : IBase<TEntity> where TEntity : EntidadBase
    {
        protected readonly AppDbContext _context;
        public RepositorioBase(AppDbContext applicationDbContext)
        {

            _context = applicationDbContext;

        }

        //Create
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);


            var rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                return entity;
            }


            return null;
        }

        //Delete

        public virtual async Task<bool> Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null) return false;

            entity.IsDeleted = true;

            return await _context.SaveChangesAsync() >= 1;
        }

        //Get 
        public virtual async Task<TEntity?> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] incluides)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in incluides)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }


        // Get all 
        public virtual async Task<IQueryable<TEntity>> GetAll(Expression<Func<TEntity, bool>> predicate, int page = 1, int take = 10, string search = "")
        {
            // El repositorio base calcula el skip automáticamente usando la página
            int skip = (page - 1) * take;

            var query = _context.Set<TEntity>().Where(predicate);

            return query.Skip(skip).Take(take).AsQueryable();
        }


        //Update
        public virtual async Task<TEntity> UpdateAsync(int id, TEntity entity)
        {
            var existingEntity = await _context.Set<TEntity>().FirstOrDefaultAsync(c => c.Id == id);

            if (existingEntity == null)
            {
                return null; 
            }

            
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

           
            existingEntity.Id = id;

            var rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected > 0)
            {
                return existingEntity;
            }

            return null;
        }

       
    }
}
