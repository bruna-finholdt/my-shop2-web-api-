using Microsoft.EntityFrameworkCore;

namespace Minishop.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
        }
    }

}
