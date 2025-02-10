using System.Linq.Expressions;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Persistance.Repository;

public interface IAsyncRepository<TEntity> : IQuery<TEntity>
    where TEntity : IEntity
{
    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Paginate<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Paginate<TEntity>> GetListByDynamicAsync(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    /// <summary>
    /// createdAtPropertyName -> CreatedAt
    /// </summary>
    /// <summary>
    /// createdByPropertyName -> CreatedBy
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="createdAtPropertyName"></param>
    /// <param name="createdByPropertyName"></param>
    /// <returns></returns>
    Task<TEntity> AddAsync(TEntity entity, string createdAtPropertyName,
        string createdByPropertyName);
    
    /// <summary>
    /// createdAtPropertyName -> CreatedAt
    /// </summary>
    /// <summary>
    /// createdByPropertyName -> CreatedBy
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="createdAtPropertyName"></param>
    /// <param name="createdByPropertyName"></param>
    /// <returns></returns>
    Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities, string createdAtPropertyName,
        string createdByPropertyName);

    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="updatedAtPropertyName"></param>
    /// <param name="updatedByPropertyName"></param>
    /// <returns></returns>
    Task<TEntity> UpdateAsync(TEntity entity,string updatedAtPropertyName,
        string updatedByPropertyName);

    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities,string updatedAtPropertyName,
    string updatedByPropertyName);

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    Task<TEntity> DeleteAsync(TEntity entity, string deletedAtPropertyName,
    string deletedByPropertyName, string isActivePropertyName,bool permanent = false);

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, string deletedAtPropertyName,
        string deletedByPropertyName, string isActivePropertyName, bool permanent = false);
}