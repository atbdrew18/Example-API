using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Customer.Api.DataLayer.EntityFramework.Entities;

namespace Customer.Api.DataLayer.EntityFramework.Configuration
{
    public class BusinessConfiguration : IEntityTypeConfiguration<Entities.Business>
    {
        public void Configure(EntityTypeBuilder<Entities.Business> builder)
        {
            builder.ToTable("Business");

            builder.HasKey(p => p.BusinessId);
            builder.Property(p => p.BusinessId).ValueGeneratedOnAdd();
        }
    }
}
