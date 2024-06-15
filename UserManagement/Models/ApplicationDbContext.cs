using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserModeClass> UserDetails { get; set; }
        public DbSet<LoginModelClass> LoginDetails { get; set; }


    }
}
