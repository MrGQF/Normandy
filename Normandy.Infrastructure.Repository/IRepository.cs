namespace Normandy.Infrastructure.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interfaces for generic repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {

        /// <summary>
        /// Changes the table name. This require the tables in the same database.
        /// </summary>
        /// <param name="table"></param>
        /// <remarks>
        /// This only been used for supporting multiple tables in the same model. This require the tables in the same database.
        /// </remarks>
        void ChangeTable(string table);

        /// <summary>
        /// Uses raw SQL queries to fetch the specified <typeparamref name="TEntity" /> data.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IQueryable{TEntity}" /> that contains elements that satisfy the condition specified by raw SQL.</returns>
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The found entity or null.</returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        ValueTask<TEntity> FindAsync(params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If found, is attached to the context and returned. If no entity is found, then null is returned.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task{TEntity}"/> that represents the asynchronous find operation. The task result contains the found entity or null.</returns>
        ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Gets all entities. This method is not recommended
        /// </summary>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        Task<IList<TEntity>> GetAllAsync();


        /// <summary>
        /// Gets the count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets async the count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets the long count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        long LongCount(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets async the long count based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets the max based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the async max based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the min based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns>decimal</returns>
        T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the async min based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

        /// <summary>
        /// Gets the average based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the async average based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the sum based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the async sum based on a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        ///  /// <param name="selector"></param>
        /// <returns>decimal</returns>
        Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

        /// <summary>
        /// Gets the Exists record based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        bool Exists(Expression<Func<TEntity, bool>> selector = null);
        /// <summary>
        /// Gets the Async Exists record based on a predicate.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null);

        /// <summary>
        /// Inserts a new entity synchronously.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void Insert(params TEntity[] entities);

        /// <summary>
        /// Inserts a range of entities synchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(params TEntity[] entities);

        /// <summary>
        /// Inserts a range of entities asynchronously.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous insert operation.</returns>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Update(params TEntity[] entities);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Delete(params TEntity[] entities);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// 返回仓储实体集合
        /// </summary>
        /// <returns>实体集合</returns>
        IQueryable<TEntity> FindAll();
        /// <summary>
        /// 依据条件返回仓储实体集合
        /// </summary>
        /// <param name="where">条件lambda</param>
        /// <returns>数量</returns>
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> where = null);
        /// <summary>
        /// 返回仓储数量,长整型表示
        /// </summary>
        /// <returns>数量</returns>
        int Count();

        /// <summary>
        /// 返回仓储数量,长整型表示
        /// </summary>
        /// <returns>数量</returns>
        long LongCount();

        /// <summary>
        /// 根据条件查找实体,无结果则抛出异常
        /// </summary>
        /// <param name="where">条件lambda</param>
        /// <returns>实体</returns>
        /// <exception cref="InvalidOperationException">未找到实体</exception>
        TEntity Find(Expression<Func<TEntity, bool>> where = null);
        /// <summary>
        /// 根据条件异步查找实体,无结果则抛出异常
        /// </summary>
        /// <param name="where">条件</param>
        /// <exception cref="InvalidOperationException">未找到实体</exception>
        /// <returns>实体</returns>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> where = null);
        /// <summary>
        /// 根据标识寻找实体,如无则返回默认值
        /// </summary>
        /// <param name="identify">标识</param>
        /// <returns>实体</returns>
        TEntity FindOrDefaultById(object identify);

        /// <summary>
        /// 根据条件查找实体,如无结果,则返回default(TEntity)
        /// </summary>
        /// <param name="where">条件lambda</param>
        /// <returns>实体</returns>
        TEntity FindOrDefault(Expression<Func<TEntity, bool>> where = null);
        /// <summary>
        /// 根据条件异步查找实体,如无结果,则返回default(TEntity)
        /// </summary>
        /// <param name="where">条件lambda</param>
        /// <returns>实体</returns>
        Task<TEntity> FindOrDefaultAsync(Expression<Func<TEntity, bool>> where = null);
        /// <summary>
        /// 查找实体是否在仓储中
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>存在标识</returns>
        bool Exist(TEntity entity);
        /// <summary>
        /// 根据条件查找实体是否在仓储中
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns>存在标识</returns>
        bool Exist(Expression<Func<TEntity, bool>> where);
    }
}