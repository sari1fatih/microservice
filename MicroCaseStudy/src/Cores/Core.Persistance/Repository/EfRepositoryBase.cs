using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Core.Persistance.Dynamic;
using Core.Persistance.Paging;
using Core.WebAPI.Appsettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.Persistance.Repository;

public class EfRepositoryBase<TEntity, TContext, TUserSession> :
    IAsyncRepository<TEntity>,
    IQuery<TEntity>,
    IRepository<TEntity>
    where TEntity : Entity
    where TContext : DbContext
{
    protected readonly TContext Context;
    private readonly IUserSession<TUserSession> _userSession;

    public EfRepositoryBase(TContext context, IUserSession<TUserSession> userSession)
    {
        Context = context;
        _userSession = userSession;
    }

    public IQueryable<TEntity> Query() => Context.Set<TEntity>();

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0,
        int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0,
        int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query().ToDynamic(dynamic);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();

        if (predicate != null)
            queryable = queryable.Where(predicate);

        return await queryable.AnyAsync(cancellationToken);
    }

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
    public async Task<TEntity> AddAsync(TEntity entity, string createdAtPropertyName,
        string createdByPropertyName)
    {
        // CreatedAt özelliğini bul ve ata
        var createdAtProperty = typeof(TEntity).GetProperty(createdAtPropertyName);
        if (createdAtProperty != null && createdAtProperty.CanWrite)
        {
            createdAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // CreatedBy özelliğini bul ve ata
        var createdByProperty = typeof(TEntity).GetProperty(createdByPropertyName);
        if (createdByProperty != null && createdByProperty.CanWrite)
        {
            var existingValue = createdByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                createdByProperty.SetValue(entity, _userSession.UserId);
            }
        }

        await Context.AddAsync(entity);
        await Context.SaveChangesAsync();

        return entity;
    }


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
    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities, string createdAtPropertyName,
        string createdByPropertyName)
    {
        foreach (TEntity entity in entities)
        {
            // CreatedAt özelliğini bul ve ayarla
            var createdAtProperty = typeof(TEntity).GetProperty(createdAtPropertyName);
            if (createdAtProperty != null && createdAtProperty.CanWrite)
            {
                createdAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            // CreatedBy özelliğini bul ve ayarla
            var createdByProperty = typeof(TEntity).GetProperty(createdByPropertyName);
            if (createdByProperty != null && createdByProperty.CanWrite)
            {
                var existingValue = createdByProperty.GetValue(entity);

                // Eğer mevcut değer null ise, yeni değeri ayarla
                if (existingValue == null || (existingValue is int intValue && intValue == 0))
                {
                    createdByProperty.SetValue(entity, _userSession.UserId);
                }
            }
        }

        await Context.AddRangeAsync(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

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
    public async Task<TEntity> UpdateAsync(TEntity entity, string updatedAtPropertyName,
        string updatedByPropertyName)
    {
        var updatedAtProperty = typeof(TEntity).GetProperty(updatedAtPropertyName);
        if (updatedAtProperty != null && updatedAtProperty.CanWrite)
        {
            updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // UpdatedBy özelliğini bul ve ata
        var updatedByProperty = typeof(TEntity).GetProperty(updatedByPropertyName);
        if (updatedByProperty != null && updatedByProperty.CanWrite)
        {
            var existingValue = updatedByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                updatedByProperty.SetValue(entity, _userSession.UserId);
            }
        }

        Context.Update(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities,
        string updatedAtPropertyName,
        string updatedByPropertyName)
    {
        foreach (TEntity entity in entities)
        {
            // UpdatedAt özelliğini bul ve ayarla
            var updatedAtProperty = typeof(TEntity).GetProperty(updatedAtPropertyName);
            if (updatedAtProperty != null && updatedAtProperty.CanWrite)
            {
                updatedAtProperty.SetValue(entity, DateTime.UtcNow);
            }

            // UpdatedBy özelliğini bul ve ayarla
            var updatedByProperty = typeof(TEntity).GetProperty(updatedByPropertyName);
            if (updatedByProperty != null && updatedByProperty.CanWrite)
            {
                var existingValue = updatedByProperty.GetValue(entity);

                // Eğer mevcut değer null ise, yeni değeri ayarla
                if (existingValue == null || (existingValue is int intValue && intValue == 0))
                {
                    updatedByProperty.SetValue(entity, _userSession.UserId);
                }
            }
        }

        Context.UpdateRange(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    public async Task<TEntity> DeleteAsync(TEntity entity, string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entity, deletedAtPropertyName, deletedByPropertyName, isDeletedPropertyName,
            permanent);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities,
        string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entities, deletedAtPropertyName, deletedByPropertyName, isDeletedPropertyName,
            permanent);
        await Context.SaveChangesAsync();
        return entities;
    }

    public TEntity? Get(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true
    )
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        return queryable.FirstOrDefault(predicate);
    }

    public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10,
        bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> source = this.Query();
        if (!enableTracking)
            source = source.AsNoTracking<TEntity>();
        if (include != null)
            source = (IQueryable<TEntity>)include(source);
        if (withDeleted)
            source = source.IgnoreQueryFilters<TEntity>();
        if (predicate != null)
            source = source.Where<TEntity>(predicate);
        return orderBy != null
            ? orderBy(source).ToPaginate<TEntity>(index, size)
            : source.ToPaginate<TEntity>(index, size);
    }

    public Paginate<TEntity> GetListByDynamic(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0,
        int size = 10, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> source = this.Query().ToDynamic<TEntity>(dynamic);
        if (!enableTracking)
            source = source.AsNoTracking<TEntity>();
        if (include != null)
            source = (IQueryable<TEntity>)include(source);
        if (withDeleted)
            source = source.IgnoreQueryFilters<TEntity>();
        if (predicate != null)
            source = source.Where<TEntity>(predicate);
        return source.ToPaginate<TEntity>(index, size);
    }

    public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false,
        bool enableTracking = true)
    {
        IQueryable<TEntity> source = this.Query();
        if (withDeleted)
            source = source.IgnoreQueryFilters<TEntity>();
        if (predicate != null)
            source = source.Where<TEntity>(predicate);
        return source.Any<TEntity>();
    }

    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="updatedAtPropertyName"></param>
    /// <param name="updatedByPropertyName"></param>
    /// <param name="???"></param>
    protected virtual void EditEntityProperties(TEntity entity, string updatedAtPropertyName,
        string updatedByPropertyName)
    {
        // UpdatedAt özelliğini bul ve ayarla
        var updatedAtProperty = typeof(TEntity).GetProperty(updatedAtPropertyName);
        if (updatedAtProperty != null && updatedAtProperty.CanWrite)
        {
            updatedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // UpdatedBy özelliğini bul ve ayarla
        var updatedByProperty = typeof(TEntity).GetProperty(updatedByPropertyName);
        if (updatedByProperty != null && updatedByProperty.CanWrite)
        {
            var existingValue = updatedByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                updatedByProperty.SetValue(entity, _userSession.UserId);
            }
        }
    }

    /// <summary>
    /// createdAtPropertyName -> CreatedAt
    /// </summary>
    /// <summary>
    /// createdByPropertyName -> CreatedBy
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="createdAtPropertyName"></param>
    /// <param name="createdByPropertyName"></param>
    protected virtual void AddEntityProperties(TEntity entity, string createdAtPropertyName,
        string createdByPropertyName)
    {
        // CreatedAt özelliğini bul ve ayarla
        var createdAtProperty = typeof(TEntity).GetProperty(createdAtPropertyName);
        if (createdAtProperty != null && createdAtProperty.CanWrite)
        {
            createdAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // CreatedBy özelliğini bul ve ayarla
        var createdByProperty = typeof(TEntity).GetProperty(createdByPropertyName);
        if (createdByProperty != null && createdByProperty.CanWrite)
        {
            var existingValue = createdByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                createdByProperty.SetValue(entity, _userSession.UserId);
            }
        }
    }

    private bool IsNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null || !type.IsValueType;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <returns></returns>
    protected virtual bool IsSoftDeleted(IEntity entity, string deletedAtPropertyName = "DeletedAt")
    {
        var deletedAtProperty = typeof(TEntity).GetProperty(deletedAtPropertyName);

        // Eğer özellik yoksa veya nullable bir tür değilse false döndür
        if (deletedAtProperty == null || !IsNullable(deletedAtProperty.PropertyType))
        {
            return false;
        }

        // DeletedAt özelliğinin değerini al
        var deletedAtValue = deletedAtProperty.GetValue(entity);

        // Eğer değer null değilse true döndür
        return deletedAtValue != null;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    protected virtual void EditEntityPropertiesToDelete(TEntity entity, string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName)
    {
        // DeletedAt özelliğini bul ve ayarla
        var deletedAtProperty = typeof(TEntity).GetProperty(deletedAtPropertyName);
        if (deletedAtProperty != null && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // IsDeleted özelliğini bul ve ayarla
        var isDeletedProperty = typeof(TEntity).GetProperty(isDeletedPropertyName);
        if (isDeletedProperty != null && isDeletedProperty.CanWrite)
        {
            isDeletedProperty.SetValue(entity, true);
        }

        // DeletedBy özelliğini bul ve ayarla
        var deletedByProperty = typeof(TEntity).GetProperty(deletedByPropertyName);
        if (deletedByProperty != null && deletedByProperty.CanWrite)
        {
            var existingValue = deletedByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                deletedByProperty.SetValue(entity, _userSession.UserId);
            }
        }
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    protected virtual void EditRelationEntityPropertiesToCascadeSoftDelete(IEntity entity,
        string deletedAtPropertyName,
        string deletedByPropertyName,
        string isDeletedPropertyName)
    {
        // DeletedAt özelliğini bul ve ayarla
        var deletedAtProperty = typeof(TEntity).GetProperty(deletedAtPropertyName);
        if (deletedAtProperty != null && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // isDeleted özelliğini bul ve ayarla
        var isDeletedProperty = typeof(TEntity).GetProperty(isDeletedPropertyName);
        if (isDeletedProperty != null && isDeletedProperty.CanWrite)
        {
            isDeletedProperty.SetValue(entity, true);
        }

        // DeletedBy özelliğini bul ve ayarla
        var deletedByProperty = typeof(TEntity).GetProperty(deletedByPropertyName);
        if (deletedByProperty != null && deletedByProperty.CanWrite)
        {
            var existingValue = deletedByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                deletedByProperty.SetValue(entity, _userSession.UserId);
            }
        }
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsActive 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="isAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="isRoot"></param>
    private async Task setEntityAsSoftDeleted(
        IEntity entity,
        string deletedAtPropertyName,
        string deletedByPropertyName,
        string isDeletedPropertyName,
        bool isAsync = true,
        CancellationToken cancellationToken = default(CancellationToken),
        bool isRoot = true)
    {
        if (this.IsSoftDeleted(entity))
            return;
        if (isRoot)
            this.EditEntityPropertiesToDelete((TEntity)entity, deletedAtPropertyName, deletedByPropertyName,
                isDeletedPropertyName);
        else
            this.EditRelationEntityPropertiesToCascadeSoftDelete(entity, deletedAtPropertyName, deletedByPropertyName,
                isDeletedPropertyName);
        foreach (INavigation navigation in this.Context.Entry<IEntity>(entity).Metadata.GetNavigations()
                     .Where<INavigation>((Func<INavigation, bool>)(x =>
                     {
                         bool flag = false;
                         if (x != null && !x.IsOnDependent)
                         {
                             IForeignKey foreignKey = x.ForeignKey;
                             if (foreignKey != null)
                             {
                                 switch (foreignKey.DeleteBehavior)
                                 {
                                     case DeleteBehavior.Cascade:
                                     case DeleteBehavior.ClientCascade:
                                         flag = true;
                                         break;
                                 }
                             }
                         }

                         return flag;
                     })).ToList<INavigation>())
        {
            if (!navigation.TargetEntityType.IsOwned() && !(navigation.PropertyInfo == (PropertyInfo)null))
            {
                object entity1 = navigation.PropertyInfo.GetValue((object)entity);
                if (navigation.IsCollection)
                {
                    if (entity1 == null)
                    {
                        IQueryable query = this.Context.Entry<IEntity>(entity)
                            .Collection(navigation.PropertyInfo.Name).Query();
                        if (isAsync)
                        {
                            IQueryable<object> relationLoaderQuery =
                                this.GetRelationLoaderQuery(query, navigation.PropertyInfo.GetType());
                            if (relationLoaderQuery != null)
                                entity1 = (object)await relationLoaderQuery.ToListAsync<object>(cancellationToken);
                        }
                        else
                        {
                            IQueryable<object> relationLoaderQuery =
                                this.GetRelationLoaderQuery(query, navigation.PropertyInfo.GetType());
                            entity1 = relationLoaderQuery != null
                                ? (object)relationLoaderQuery.ToList<object>()
                                : (object)(List<object>)null;
                        }

                        if (entity1 == null)
                            continue;
                    }

                    foreach (IEntity entity2 in (IEnumerable)entity1)
                        await this.setEntityAsSoftDeleted(entity2, deletedAtPropertyName, deletedByPropertyName,
                            isDeletedPropertyName,
                            isAsync,
                            cancellationToken, false);
                }
                else
                {
                    if (entity1 == null)
                    {
                        IQueryable query = this.Context.Entry<IEntity>(entity)
                            .Reference(navigation.PropertyInfo.Name).Query();
                        if (isAsync)
                        {
                            IQueryable<object> relationLoaderQuery =
                                this.GetRelationLoaderQuery(query, navigation.PropertyInfo.GetType());
                            if (relationLoaderQuery != null)
                                entity1 = await relationLoaderQuery.FirstOrDefaultAsync<object>(cancellationToken);
                        }
                        else
                        {
                            IQueryable<object> relationLoaderQuery =
                                this.GetRelationLoaderQuery(query, navigation.PropertyInfo.GetType());
                            entity1 = relationLoaderQuery != null
                                ? relationLoaderQuery.FirstOrDefault<object>()
                                : (object)null;
                        }

                        if (entity1 == null)
                            continue;
                    }

                    await this.setEntityAsSoftDeleted((IEntity)entity1, deletedAtPropertyName,
                        deletedByPropertyName, isDeletedPropertyName, isAsync, cancellationToken, false);
                }
            }
        }

        this.Context.Update<IEntity>(entity);
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsActive 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="permanent"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="isAsync"></param>
    /// <param name="cancellationToken"></param>
    protected async Task SetEntityAsDeleted(
        TEntity entity,
        string deletedAtPropertyName,
        string deletedByPropertyName,
        string isDeletedPropertyName,
        bool permanent,
        bool isAsync = true,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!permanent)
        {
            this.CheckHasEntityHaveOneToOneRelation(entity);
            if (isAsync)
                await this.setEntityAsSoftDeleted((IEntity)entity, deletedAtPropertyName,
                    deletedByPropertyName, isDeletedPropertyName, isAsync, cancellationToken);
            else
                this.setEntityAsSoftDeleted((IEntity)entity, deletedAtPropertyName, deletedByPropertyName,
                    isDeletedPropertyName,
                    isAsync).Wait();
        }
        else
            this.Context.Remove<TEntity>(entity);
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted 
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="permanent"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="isAsync"></param>
    /// <param name="cancellationToken"></param>
    protected async Task SetEntityAsDeleted(
        IEnumerable<TEntity> entities,
        string deletedAtPropertyName,
        string deletedByPropertyName,
        string isDeletedPropertyName,
        bool permanent,
        bool isAsync = true,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        foreach (TEntity entity in entities)
            await this.SetEntityAsDeleted(entity, deletedAtPropertyName, deletedByPropertyName, isDeletedPropertyName,
                permanent, isAsync,
                cancellationToken);
    }

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
    public TEntity Add(TEntity entity, string createdAtPropertyName,
        string createdByPropertyName)
    {
        this.AddEntityProperties(entity, createdAtPropertyName, createdByPropertyName);
        this.Context.Add<TEntity>(entity);
        this.Context.SaveChanges();
        return entity;
    }

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
    public ICollection<TEntity> AddRange(ICollection<TEntity> entities, string createdAtPropertyName,
        string createdByPropertyName)
    {
        foreach (TEntity entity in (IEnumerable<TEntity>)entities)
            this.AddEntityProperties(entity, createdAtPropertyName, createdByPropertyName);
        this.Context.AddRange((IEnumerable<object>)entities);
        this.Context.SaveChanges();
        return entities;
    }

    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public TEntity Update(TEntity entity, string updatedAtPropertyName,
        string updatedByPropertyName)
    {
        this.EditEntityProperties(entity, updatedAtPropertyName, updatedByPropertyName);
        this.Context.Update<TEntity>(entity);
        this.Context.SaveChanges();
        return entity;
    }

    /// <summary>
    /// updatedAtPropertyName -> UpdatedAt
    /// </summary>
    /// <summary>
    /// updatedByPropertyName -> UpdatedBy
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities, string updatedAtPropertyName,
        string updatedByPropertyName)
    {
        foreach (TEntity entity in (IEnumerable<TEntity>)entities)
            this.EditEntityProperties(entity, updatedAtPropertyName, updatedByPropertyName);
        this.Context.UpdateRange((IEnumerable<object>)entities);
        this.Context.SaveChanges();
        return entities;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    public TEntity Delete(TEntity entity, string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName, bool permanent = false)
    {
        this.SetEntityAsDeleted(entity, deletedAtPropertyName, deletedByPropertyName, isDeletedPropertyName, permanent,
            false).Wait();
        this.Context.SaveChanges();
        return entity;
    }


    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="permanent"></param>
    /// <returns></returns>
    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName, bool permanent = false)
    {
        this.SetEntityAsDeleted((IEnumerable<TEntity>)entities, deletedAtPropertyName, deletedByPropertyName,
            isDeletedPropertyName, permanent,
            false).Wait();
        this.Context.SaveChanges();
        return entities;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsActive 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="permanent"></param>
    protected async Task SetEntityAsDeletedAsync(TEntity entity, string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            await setEntityAsSoftDeletedAsync(entity, deletedAtPropertyName, deletedByPropertyName,
                isDeletedPropertyName);
        }
        else
        {
            Context.Remove(entity);
        }
    }

    protected void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {
        bool hasEntityHaveOneToOneRelation =
            Context
                .Entry(entity)
                .Metadata.GetForeignKeys()
                .All(
                    x =>
                        x.DependentToPrincipal?.IsCollection == true
                        || x.PrincipalToDependent?.IsCollection == true
                        || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
                ) == false;
        if (hasEntityHaveOneToOneRelation)
            throw new InvalidOperationException(
                "Entity has one-to-one relationship. Soft Delete causes problems if you try to create entry again by same foreign key."
            );
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    ///  /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsDeleted 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="???"></param>
    private async Task setEntityAsSoftDeletedAsync(IEntity entity,
        string deletedAtPropertyName,
        string deletedByPropertyName,
        string isDeletedPropertyName)
    {
        // DeletedAt özelliğini bul ve ayarla
        var deletedAtProperty = typeof(TEntity).GetProperty(deletedAtPropertyName);
        if (deletedAtProperty != null && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(entity, DateTime.UtcNow);
        }

        // isDeleted özelliğini bul ve ayarla
        var isDeletedProperty = typeof(TEntity).GetProperty(isDeletedPropertyName);
        if (isDeletedProperty != null && isDeletedProperty.CanWrite)
        {
            isDeletedProperty.SetValue(entity, true);
        }

        // DeletedBy özelliğini bul ve ayarla
        var deletedByProperty = typeof(TEntity).GetProperty(deletedByPropertyName);
        if (deletedByProperty != null && deletedByProperty.CanWrite)
        {
            var existingValue = deletedByProperty.GetValue(entity);

            // Eğer mevcut değer null ise, yeni değeri ayarla
            if (existingValue == null || (existingValue is int intValue && intValue == 0))
            {
                deletedByProperty.SetValue(entity, _userSession.UserId);
            }
        }

        var navigations = Context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is
            {
                IsOnDependent: false,
                ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade
            })
            .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query,
                        navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();
                    if (navValue == null)
                        continue;
                }

                foreach (IEntity navValueItem in (IEnumerable)navValue)
                    await setEntityAsSoftDeletedAsync(navValueItem, deletedAtPropertyName, deletedByPropertyName,
                        isDeletedPropertyName);
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query,
                            navigationPropertyType: navigation.PropertyInfo.GetType())
                        .FirstOrDefaultAsync();
                    if (navValue == null)
                        continue;
                }

                await setEntityAsSoftDeletedAsync((IEntity)navValue, deletedAtPropertyName,
                    deletedByPropertyName, isDeletedPropertyName);
            }
        }

        Context.Update(entity);
    }

    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider,
                parameters: new object[] { query.Expression })!;

        var parameter = Expression.Parameter(typeof(object), "x");

        var property = navigationPropertyType.GetProperty("DeletedAt");
        if (property != null)
        {
            var propertyAccess =
                Expression.MakeMemberAccess(Expression.Convert(parameter, navigationPropertyType), property);
            var isNotNullExpression = Expression.NotEqual(propertyAccess, Expression.Constant(null));

            var lambda = Expression.Lambda<Func<object, bool>>(isNotNullExpression, parameter);
            return queryProviderQuery.Where(lambda);
        }

        return queryProviderQuery;
    }

    /// <summary>
    /// deletedAtPropertyName -> DeletedAt 
    /// </summary>
    /// <summary>
    /// deletedByPropertyName -> DeletedBy 
    /// </summary>
    /// <summary>
    /// isDeletedPropertyName -> IsActive 
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="deletedAtPropertyName"></param>
    /// <param name="deletedByPropertyName"></param>
    /// <param name="isDeletedPropertyName"></param>
    /// <param name="permanent"></param>
    protected async Task SetEntityAsDeletedAsync(IEnumerable<TEntity> entities, string deletedAtPropertyName,
        string deletedByPropertyName, string isDeletedPropertyName, bool permanent)
    {
        foreach (TEntity entity in entities)
            await SetEntityAsDeletedAsync(entity, deletedAtPropertyName, deletedByPropertyName, isDeletedPropertyName,
                permanent);
    }
}