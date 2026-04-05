using VolunteerHub.Application.Abstractions;

namespace VolunteerHub.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation wrapping AppDbContext.SaveChangesAsync.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
