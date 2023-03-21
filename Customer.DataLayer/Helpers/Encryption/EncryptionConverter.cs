using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Customer.Api.DataLayer.Utils;

namespace Customer.Api.DataLayer.Helpers.Encryption
{
    internal sealed class EncryptionConverter : ValueConverter<string, string>
    {
        public EncryptionConverter(ConverterMappingHints mappingHints = null)
        : base(x => AESEncryption.Encrypt(x), x => AESEncryption.Decrypt(x), mappingHints)
        {
        }
    }
}