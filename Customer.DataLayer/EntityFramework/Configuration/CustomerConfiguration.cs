using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Customer.Api.DataLayer.EntityFramework.Entities;

namespace Customer.Api.DataLayer.EntityFramework.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Entities.Customer>
    {
        public void Configure(EntityTypeBuilder<Entities.Customer> builder)
        {
            builder.ToTable("Customer");

            builder.HasKey(p => p.CustomerId);
            builder.Property(p => p.CustomerId).ValueGeneratedOnAdd();
        }
    }
}
