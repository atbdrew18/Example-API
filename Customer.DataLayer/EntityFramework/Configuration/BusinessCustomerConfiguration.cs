using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Customer.Api.DataLayer.EntityFramework.Entities;

namespace Customer.Api.DataLayer.EntityFramework.Configuration
{
    public class BusinessCustomerConfiguration : IEntityTypeConfiguration<Entities.BusinessCustomer>
    {
        public void Configure(EntityTypeBuilder<Entities.BusinessCustomer> builder)
        {
            builder.ToTable("BusinessCustomer");

            builder.HasKey(p => p.BusinessCustomerId);
            builder.Property(p => p.BusinessCustomerId).ValueGeneratedOnAdd();

            builder.HasOne(p => p.Customer).WithMany().HasForeignKey(p => p.CustomerId);
            builder.HasOne(p => p.Business).WithMany().HasForeignKey(p => p.BusinessId);
        }
    }
}
