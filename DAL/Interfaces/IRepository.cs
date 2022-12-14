using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    /// <summary>
    /// Generic repository intervace.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity</typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Gets all entities from context DBSet
        /// </summary>
        /// <returns><see cref="IQueryable"/> of all Entities in DBSet.</returns>
        IQueryable<TEntity> FindAll();

        /// <summary>
        /// Gets Entity by its Id.
        /// </summary>
        /// <param name="id">Id of Entity to be found.</param>
        /// <returns><see cref="Task"/> objects containing found object.</returns>
        Task<TEntity> GetByIdAsync(int id);

        /// <summary>
        /// Adds entity to context BDSet.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <returns><see cref="Task"/> object.</returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Updates Entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes Entity from context DBSet.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Asyncronously deletes Entity with specified Id.
        /// </summary>
        /// <param name="id">Id of Entity to delete.</param>
        /// <returns><see cref="Task"/> object.</returns>
        Task DeleteByIdAsync(int id);
    }
}
