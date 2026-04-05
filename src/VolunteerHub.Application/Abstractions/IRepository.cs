using System.Linq.Expressions;
using VolunteerHub.Domain.Common;

namespace VolunteerHub.Application.Abstractions;

/// <summary>
/// Generic repository contract. Implemented by Infrastructure layer.
/// Add, Update, Remove are synchronous — they only track changes.
/// Call IUnitOfWork.SaveChangesAsync to persist.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
