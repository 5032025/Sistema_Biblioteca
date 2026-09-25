using Dominio_API.Clases;
using Dominio_API.Interfaces;
using Microsoft.AspNetCore.Http; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims; 
using System.Text;

namespace Infraestructura_API.Persistencia.Repositorios
{
    public abstract class RepositorioBase<TEntity> : IBase<TEntity> where TEntity : EntidadBase
    {
        protected readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor; // <-- Inyectar el accessor

        public RepositorioBase(AppDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        // Create
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            // Extraer el usuario autenticado desde las claims del token
            // Usamos ClaimTypes.Name que almacena el Email según tu JWTService, o puedes usar NameIdentifier para el ID
            var usuarioCreacion = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

            // Asignación automática de auditoría
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;
            entity.CreatedBy = !string.IsNullOrEmpty(usuarioCreacion) ? usuarioCreacion : "Sistema";

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

        // Update
        public virtual async Task<TEntity> UpdateAsync(int id, TEntity entity)
        {
            // 1. Asignamos el ID por seguridad al objeto que entra
            var primaryKeyProperty = _context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey().Properties.FirstOrDefault();
            if (primaryKeyProperty != null)
            {
                var propertyInfo = typeof(TEntity).GetProperty(primaryKeyProperty.Name);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(entity, id);
                }
            }

            // 2. Indicamos explícitamente a Entity Framework que el objeto está modificado
            _context.Entry(entity).State = EntityState.Modified;

            // 3. Evitamos que EF intente modificar los campos de auditoría de creación si existen
            var entry = _context.Entry(entity);
            if (entry.Metadata.FindProperty("CreatedAt") != null)
                entry.Property("CreatedAt").IsModified = false;

            if (entry.Metadata.FindProperty("CreatedBy") != null)
                entry.Property("CreatedBy").IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Set<TEntity>().AnyAsync(e => EF.Property<int>(e, primaryKeyProperty.Name) == id))
                {
                    return null;
                }
                throw;
            }

            return entity;
        }

    }
}
