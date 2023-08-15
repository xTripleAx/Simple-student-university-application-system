using Microsoft.EntityFrameworkCore;

namespace TheTask.Data
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options) 
        {

        }  
        
        public DbSet<Application> Application { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Faculty> faculty { get; set; }
        public DbSet<Major> major { get; set; }
        public DbSet<User> User { get; set; }

    }
}