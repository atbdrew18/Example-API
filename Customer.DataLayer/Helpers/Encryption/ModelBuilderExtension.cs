using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Customer.Api.DataLayer.Helpers.Encryption
{
    public static class ModelBuilderExtension
    {
        public static void UseEncryption(this ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder), "There is not ModelBuilder object.");
            }

            var encryptionConverter = new EncryptionConverter();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        object[] attributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptAttribute), false);
                        if (attributes.Any())
                        {
                            property.SetValueConverter(encryptionConverter);
                        }
                    }
                }
            }

        }
    }
}