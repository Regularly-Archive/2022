namespace DomainEvents.Infrastructure
{
    public interface IDomainEventDispatcher
    {
        public Task DispatchDomainEvent<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent;
    }
}
