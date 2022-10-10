namespace DomainEvents.Infrastructure
{
	public abstract class Entity : IEntity
	{		
		private List<IDomainEvent> _domainEvents = null;
		public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

		public void AddDomainEvent(IDomainEvent eventItem)
		{
			_domainEvents = _domainEvents ?? new List<IDomainEvent>();
			_domainEvents.Add(eventItem);
		}

		public void RemoveDomainEvent(IDomainEvent eventItem)
		{
			_domainEvents?.Remove(eventItem);
		}

		public void ClearDomainEvents()
		{
			_domainEvents?.Clear();
		}

		public abstract object[] GetKeys();
		public virtual DateTime CreatedAt { get; set; }
		public virtual string CreatedBy { get; set; }
	}

    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        public TKey Id { get; set; }

		public override object[] GetKeys() => new object[] { Id };
    }
}
