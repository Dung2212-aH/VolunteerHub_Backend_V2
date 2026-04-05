namespace VolunteerHub.Application.Abstractions;

/// <summary>
/// Unit of Work contract — flushes all tracked changes to the database.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
