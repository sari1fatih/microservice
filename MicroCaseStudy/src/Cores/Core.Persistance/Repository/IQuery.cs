namespace Core.Persistance.Repository;

public interface IQuery<T>
{
    IQueryable<T> Query();
}