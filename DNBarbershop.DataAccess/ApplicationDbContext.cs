using Microsoft.EntityFrameworkCore;

namespace DNBarbershop.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
    }
}
