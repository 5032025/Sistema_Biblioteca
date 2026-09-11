using Dominio_API.Clases;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dominio_API.Interfaces
{
    public interface IBase<TEntity> where TEntity : EntidadBase
    {
        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity?> FindFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] incluides);

        Task<bool> Delete(int id);

        Task<TEntity> UpdateAsync(int id, TEntity entity);

        Task<IQueryable<TEntity>> GetAll(Expression<Func<TEntity, bool>> predicate, int skip, int take, string search);

        
    }
}
