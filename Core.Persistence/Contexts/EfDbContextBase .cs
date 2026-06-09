using Core.Abstractions.ContextExecutions;
using Core.Abstractions.Domain;
using Core.Abstractions.Events.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Core.Persistence.Contexts
{
    public class EfDbContextBase : DbContext, IUnitOfWork
    {
        private IDbContextTransaction _transaction;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        protected EfDbContextBase(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher
                                                          ) : base(options)
        {
            System.Diagnostics.Debug.WriteLine($"{GetType().Name}::ctor");
            _domainEventDispatcher = domainEventDispatcher;
        }



        public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            _transaction ??= await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                await _transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            var strategy = Database.CreateExecutionStrategy();

            return strategy.ExecuteAsync(async () =>
            {

                await BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
                try
                {
                    await action();
                    await CommitTransactionAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await RollbackTransactionAsync(cancellationToken);
                    throw;
                }

            });
        }

        public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            var strategy = Database.CreateExecutionStrategy();

            return strategy.ExecuteAsync(async () =>
            {

                await BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
                try
                {
                    var result = await action();
                    await CommitTransactionAsync(cancellationToken);
                    return result;
                }
                catch (Exception)
                {
                    await RollbackTransactionAsync(cancellationToken);
                    throw;
                }

            });
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _transaction?.RollbackAsync(cancellationToken);
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = GatherDomainEvents();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (_domainEventDispatcher is not null && domainEvents.Length > 0)
                await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            return result;
        }

        private IDomainEvent[] GatherDomainEvents()
        {
            var aggregates = ChangeTracker.Entries<IAggregateRoot>().Where(e => e.Entity.DomainEvents.Any()).Select(e => e.Entity).ToList();

            var events = aggregates.SelectMany(a => a.DomainEvents).ToArray();

            aggregates.ForEach(a => a.ClearDomainEvents());
            return events;
        }
    }
}
