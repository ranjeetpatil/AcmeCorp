using AcmeCorp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcmeCorp.Core.Data
{
    public class AcmeCorpDbContext : DbContext
    {
        public AcmeCorpDbContext(DbContextOptions<AcmeCorpDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.ContactInfo)
                .WithOne(ci => ci.Customer)
                .HasForeignKey<ContactInfo>(ci => ci.Customer.Id);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
