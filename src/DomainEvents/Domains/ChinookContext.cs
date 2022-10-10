using DomainEvents.Domains.Configurations;
using DomainEvents.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DomainEvents.Domains
{
    public class ChinookContext : DbContext
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        public ChinookContext(IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher;
            this.SavedChanges += async (s, e) =>
            {
                var context = s as ChinookContext;

                var entities = context.ChangeTracker.Entries()
                    .Where(x => x.Entity is Entity && ((Entity)x.Entity).DomainEvents.Any())
                    .Select(x => (Entity)x.Entity)
                    .ToList();

                foreach (var entity in entities)
                {
                    await _domainEventDispatcher.DispatchDomainEvent(entity.DomainEvents);
                    entity.ClearDomainEvents();
                }
            };
            //this.SavingChanges += ChinookContext_SavingChanges;
        }

        //private void ChinookContext_SavingChanges(object? sender, SavingChangesEventArgs e)
        //{
        //    var context = sender as ChinookContext;

        //    var entities = context.ChangeTracker.Entries()
        //        .Where(x => x.Entity is Entity && ((Entity)x.Entity).DomainEvents.Any())
        //        .Select(x => (Entity)x.Entity)
        //        .ToList();

        //    foreach (var entity in entities)
        //    {
        //        _domainEventDispatcher.DispatchDomainEvent(entity.DomainEvents).GetAwaiter().GetResult();
        //        entity.ClearDomainEvents();
        //    }
        //}

        public DbSet<OrderInfo> OrderInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=Chinook.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new OrderInfoMap());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //var entities = ChangeTracker.Entries()
            //    .Where(x => x.Entity is Entity && ((Entity)x.Entity).DomainEvents.Any())
            //    .Select(x => (Entity)x.Entity)
            //    .ToList();

            //foreach (var entity in entities)
            //{
            //    await _domainEventDispatcher.DispatchDomainEvent(entity.DomainEvents, cancellationToken);
            //    entity.ClearDomainEvents();
            //}

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
