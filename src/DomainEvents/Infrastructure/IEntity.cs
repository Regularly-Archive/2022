namespace DomainEvents.Infrastructure
{
    public interface IEntity
    {
        object[] GetKeys();
    }

    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}