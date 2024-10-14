namespace ChatClient.Domain.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }
}