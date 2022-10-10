using DomainEvents.Infrastructure;

namespace DomainEvents.Events
{
    public class OrderInfoCreatedEvent : DomainEvent
    {
        public string Remark { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public decimal Quantity { get; set; }
    }
}
