using System.Linq.Expressions;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Persistance.Repository;

public interface IRepository<TEntity> : IQuery<TEntity>
    where TEntity : IEntity
{
    TEntity? Get(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true
    );

    Paginate<TEntity> GetList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true
    );

    Paginate<TEntity> GetListByDynamic(
        DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true
    );

    bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true);
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
    TEntity Add(TEntity entity,string createdAtPropertyName,
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
    ICollection<TEntity> AddRange(ICollection<TEntity> entities, string createdAtPropertyName,
        string createdByPropertyName);
    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    TEntity Update(TEntity entity, string updatedAtPropertyName,
        string updatedByPropertyName);
    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    ICollection<TEntity> UpdateRange(ICollection<TEntity> entities, string updatedAtPropertyName,
        string updatedByPropertyName);
    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    TEntity Delete(TEntity entity,string deletedAtPropertyName,
        string deletedByPropertyName, string isActivePropertyName,bool permanent = false);
    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    ICollection<TEntity> DeleteRange(ICollection<TEntity> entity, string deletedAtPropertyName,
        string deletedByPropertyName, string isActivePropertyName,bool permanent = false);
}