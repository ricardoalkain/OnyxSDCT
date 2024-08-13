using Onyx.ProductService.Entities;

namespace Onyx.ProductService.Repositories.Base;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected abstract Dictionary<int, T> Data { get; }

    public virtual T? Get(int id)
    {
        if (Data.TryGetValue(id, out var entity))
        {
            return entity;
        }

        return default;
    }

    public virtual IEnumerable<T> GetAll() => Data.Values;

    public virtual int Add(T entity)
    {
        var id = (Data.Count > 0 ? Data.Keys.Max() : 0) + 1;
        entity.Id = id;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = null;
        Data.Add(id, entity);
        return id;
    }

    public virtual bool Update(T entity)
    {
        if (Data.TryGetValue(entity.Id, out var _))
        {
            entity.UpdatedAt = DateTime.UtcNow;
            Data[entity.Id] = entity;
            return true;
        }

        return false;
    }

    public virtual bool Delete(int id)
    {
        if (!Data.ContainsKey(id))
        {
            return false;
        }

        Data.Remove(id);
        return true;
    }
}
