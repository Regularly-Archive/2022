using DomainEvents.Infrastructure;

namespace DomainEvents.Events
{
    public class OrderInfoUpdatedEvent : DomainEvent
    {
        public string Remark { get; set; }
        public string Address { get; private set; }
        public string Telephone { get; private set; }
        public decimal Quantity { get; private set; }
    }
}
