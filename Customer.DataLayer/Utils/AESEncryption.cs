using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Customer.Api.DataLayer.Utils
{
    /// <summary>
    /// AES Encryption Implementation
    /// </summary>
    /// <remarks>
    /// Implements a Standard AES Encrypt and Decrypt method call. The Encryption Vector is stored as the first 32 bytes of the string value.
    /// </remarks>
    public static class AESEncryption
    {
        /// <summary>
        /// Default key to use if they key is not specified in the web.config
        /// This needs to be a base 64 encoded string.
        /// </summary>
        private const string DEFAULT_KEY = "3ZtxpnEhH2Zmoh7CnxssD2y6O6Wj2LkQdKh8eCiDamw=";

        public static string KeyFromSettings { get; set; }

        /// <summary>
        /// Encrypts a data string.
        /// </summary>
        /// <param name="data">The source data string.</param>
        /// <returns>The encrypted string in base 64 encoded format.</returns>
        /// <remarks>
        /// Encrypt the string data with the standard key.
        /// </remarks>
        public static string Encrypt(string data)
        {
            try
            {
                if (data == null)
                {
                    return data;
                }
                if (data.Trim() == "")
                {
                    return data;
                }

                string key32 = GetKey();

                return AESEncrypt(data, key32);
            }
            catch (Exception e)
            {
                throw new Exception("Encryption Error", e);
            }
        }

        /// <summary>
        /// Decrypts a data string.
        /// </summary>
        /// <param name="data">The base 64 encoded string to decrypt.</param>
        /// <returns>The decrypted string.</returns>
        /// <remarks>
        /// Decrypts the string data with the standard key
        /// </remarks>
        public static string Decrypt(string data)
        {
            try
            {
                if (data == null)
                {
                    return data;
                }
                if (data.Trim() == "")
                {
                    return data;
                }

                string key32 = GetKey();

                return AESDecrypt(data, key32);
            }
            catch (Exception e)
            {
                throw new Exception($"Encryption Error - Cannot decrypt {data}."
                +"\n1. Is the value encrypted?"
                +"\n2. Does the decryption key match the key that encrypted the value?"
                , e);
            }
        }

        /// <summary>
        /// Encrypts the string data with a Specific Key
        /// </summary>
        /// <param name="strText">The data to be encrypted.</param>
        /// <param name="sKey">A key value.</param>
        /// <returns>The data string encrypted.</returns>
        /// <remarks>
        /// Encrypts the passed in data string using the passed in key and vector and the Rijndael algorithm.
        /// The key and vector must be of a fixed size: 128, 192, or 256 bits (16, 24, or 32 characters).
        /// This implementation requires 32 character keys and vectors.
        /// </remarks>
        public static string AESEncrypt(string strText, string sKey)
        {
            ASCIIEncoding textConverter = new ASCIIEncoding();
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] encrypted;
            byte[] toEncrypt;
            byte[] key;
            byte[] IV;

            //Create a new key and initialization vector.
            myRijndael.KeySize = 256;
            myRijndael.BlockSize = 128;
            myRijndael.GenerateIV();

            //Get the key and IV.
            key = Convert.FromBase64String(sKey == "" ? DEFAULT_KEY : sKey);
            IV = myRijndael.IV;

            //Get an encryptor.
            ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, IV);

            //Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            //Convert the data to a byte array.
            toEncrypt = textConverter.GetBytes(strText);

            //Write all data to the crypto stream and flush it.
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            //Get encrypted array of bytes.
            encrypted = msEncrypt.ToArray();

            // combine our initialization vector and ancrypted data into one string.
            byte[] outputArray = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, outputArray, IV.Length);
            Array.Copy(encrypted, 0, outputArray, IV.Length, encrypted.Length);

            return Convert.ToBase64String(outputArray);
        }

        /// <summary>
        /// Encrypts the string data with a Specific Key and Vector
        /// </summary>
        /// <param name="strText">The data to be encrypted.</param>
        /// <param name="sKey">A key value.</param>
        /// <param name="vector">Vector</param>
        /// <returns>The data string encrypted.</returns>
        /// <remarks>
        /// Encrypts the passed in data string using the passed in key and vector and the Rijndael algorithm.
        /// The key and vector must be of a fixed size: 128, 192, or 256 bits (16, 24, or 32 characters).
        /// This implementation requires 32 character keys and vectors.
        /// </remarks>
        public static string AESEncrypt(string strText, string sKey, string vector)
        {
            ASCIIEncoding textConverter = new ASCIIEncoding();
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] encrypted;
            byte[] toEncrypt;
            byte[] key;
            byte[] IV;

            myRijndael.KeySize = 256;
            myRijndael.BlockSize = 256;

            //Get the key and IV.
            key = Convert.FromBase64String(sKey == "" ? DEFAULT_KEY : sKey);

            if (String.IsNullOrEmpty(vector))
            {
                myRijndael.GenerateIV();
                IV = myRijndael.IV;
            }
            else
            {
                IV = Convert.FromBase64String(vector);
            }

            //Get an encryptor.
            ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, IV);

            //Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            //Convert the data to a byte array.
            toEncrypt = textConverter.GetBytes(strText);

            //Write all data to the crypto stream and flush it.
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            //Get encrypted array of bytes.
            encrypted = msEncrypt.ToArray();

            // combine our initialization vector and ancrypted data into one string.
            byte[] outputArray = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, outputArray, IV.Length);
            Array.Copy(encrypted, 0, outputArray, IV.Length, encrypted.Length);

            return Convert.ToBase64String(outputArray);
        }

        /// <summary>
        /// Decrypts the string data with a Specific Key
        /// </summary>
        /// <param name="strText">The data to be encrypted.</param>
        /// <param name="sKey">A key value.</param>
        /// <returns>The data string decrypted.</returns>
        /// <remarks>
        /// Decrypts the passed in data string using the passed in key and vector and the Rijndael algorithm.
        /// The key and vector must be of a fixed size: 128, 192, or 256 bits (16, 24, or 32 characters).
        /// This implementation requires 32 character keys.
        /// </remarks>
        public static string AESDecrypt(string strText, string sKey)
        {
            string roundtrip;
            ASCIIEncoding textConverter = new ASCIIEncoding();
            RijndaelManaged myRijndael = new RijndaelManaged();
            byte[] fromEncrypt;
            byte[] encrypted;
            byte[] key;
            byte[] IV = new byte[16];
            byte[] convertedText;

            myRijndael.KeySize = 256;
            myRijndael.BlockSize = 128;

            // convert the base 64 encoded string into a byte array
            convertedText = Convert.FromBase64String(strText);

            // get the vector out of the byte array
            Array.Copy(convertedText, IV, 16);

            // get the data out of the byte array
            int length = convertedText.Length - 16;
            encrypted = new byte[length];
            Array.Copy(convertedText, 16, encrypted, 0, length);

            // get the key
            key = Convert.FromBase64String(sKey);

            //Get a decryptor that uses the same key and IV as the encryptor.
            ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, IV);

            //Now decrypt the previously encrypted message using the decryptor
            // obtained in the above step.
            MemoryStream msDecrypt = new MemoryStream(encrypted);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            fromEncrypt = new byte[encrypted.Length + 1];

            //Read the data out of the crypto stream.
            //dotnet 6 introduced a new breaking change where .read is not guaranteed to return all the btytes any longer.
            //See https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/partial-byte-reads-in-streams
            int totalBytesRead = 0;
            while (totalBytesRead < fromEncrypt.Length)
            {
                //The byte offset in buffer at which to begin storing the data read from the current stream.
                var offset = totalBytesRead;
                //The maximum number of bytes to be read from the current stream.
                var bytesLeftToRead = fromEncrypt.Length - totalBytesRead;
                var bytesRead = csDecrypt.Read(fromEncrypt, offset, bytesLeftToRead);
                if (bytesRead == 0)
                {
                    break;
                }
                totalBytesRead += bytesRead;
            }

            //Convert the byte array back into a string.
            //roundtrip = textConverter.GetString(fromEncrypt, 0, fromEncrypt.Length)
            roundtrip = textConverter.GetString(fromEncrypt).TrimEnd(Convert.ToChar(0));
            return (roundtrip);
        }

        /// <summary>
        /// GetKey
        /// </summary>
        /// <returns></returns>
        private static string GetKey()
        {
            return KeyFromSettings ?? DEFAULT_KEY;
        }
    }
}