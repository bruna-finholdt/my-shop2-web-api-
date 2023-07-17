using Microsoft.EntityFrameworkCore.ChangeTracking;
using Minishop.DAL;

public interface ICustomersDbContext : IMinishop2023Context
{
    EntityEntry Entry(object entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
