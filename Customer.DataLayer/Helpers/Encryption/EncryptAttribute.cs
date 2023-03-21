using System;

namespace Customer.Api.DataLayer.Helpers.Encryption
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EncryptAttribute : Attribute
    {
    }
}