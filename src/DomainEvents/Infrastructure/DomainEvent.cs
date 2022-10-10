namespace DomainEvents.Infrastructure
{
    public class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
    }
}
