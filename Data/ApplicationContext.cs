using Microsoft.EntityFrameworkCore;
using drv_next_api.Models;

namespace drv_next_api.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            var customer = b.Entity<Customer>();
            customer.HasKey(c => c.Id);
            customer.HasIndex(c => c.PhoneNumber).IsUnique();

            var trip = b.Entity<Trip>();
            trip.HasKey(t => t.Id);
            trip.HasOne(t => t.Customer);
        }
    }
}