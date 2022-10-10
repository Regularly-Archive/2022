using Newtonsoft.Json;

namespace DomainEvents.Infrastructure
{
    public class InMemoryDomainEventDispatcher : IDomainEventDispatcher
    {
        public InMemoryDomainEventDispatcher()
        {

        }

        public Task DispatchDomainEvent<TDomainEvent>(IEnumerable<TDomainEvent> domainEvents, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent
        {
            foreach(var domainEvent in domainEvents)
            {
                Console.WriteLine("Send DomainEvent: {0}", JsonConvert.SerializeObject(domainEvent));
            }

            return Task.CompletedTask;
        }
    }
}
