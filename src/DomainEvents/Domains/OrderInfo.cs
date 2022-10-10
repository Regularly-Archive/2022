using DomainEvents.Events;
using DomainEvents.Infrastructure;
using Mapster;

namespace DomainEvents.Domains
{
    public class OrderInfo : Entity<Guid>
    {
        public string Remark { get; private set; }
        public string Address { get; private set; }
        public string Telephone { get; private set; }
        public decimal Quantity { get; private set; }

        public OrderInfo(string address, string telephone, decimal quantity, string remark)
        {
            Remark = remark;
            Address = address;
            Quantity = quantity;
            Telephone = telephone;
            Id = Guid.NewGuid();
        }

        public void Confirm()
        {
            CreatedBy = "System";
            CreatedAt = DateTime.Now;
            AddDomainEvent(this.Adapt<OrderInfoCreatedEvent>());
        }

        public void ModifyAddress(string address)
        {
            Address = address;
            AddDomainEvent(this.Adapt<OrderInfoUpdatedEvent>());
        }
    }
}
