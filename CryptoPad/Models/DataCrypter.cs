using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoPad.Models
{
    class DataCrypter
    {
        private const int formatLength = 4;
        private const string prefixString = "AI297 CryptoPad encrypted file";

        private HashAlgorithm hash = SHA256.Create();
        private byte[] secureKey;


        public DataCrypter(string secureKey)
        {
            if (secureKey == null) throw new ArgumentNullException();
            this.secureKey = Encoding.Unicode.GetBytes(secureKey);
        }

        /// <summary>
        /// Encrypt data with security key
        /// </summary>
        /// <param name="data">data for encript</param>
        /// <param name="format">file format (extension without '.'), using first 4 chars only</param>
        /// <returns>bytes array with encripted data</returns>
        public byte[] Encrypt(byte[] data, string format = "txt")
        {
            if (data == null || string.IsNullOrEmpty(format)) throw new ArgumentNullException();
            
            format = format.Trim().ToLower().PadRight(formatLength).Substring(0, formatLength);
            var formatData = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(format));

            var prefix = GetPrefix();

            var ciper = GetCiper();
            ciper.GenerateIV(); // generate new vector bytes

            // encrypt data
            var encryptor = ciper.CreateEncryptor();
            var encryptedData = encryptor.TransformFinalBlock(data, 0, data.Length);

            // create result bytes array with all data
            var result = new byte[prefix.Length + formatLength + ciper.IV.Length + encryptedData.Length];
            Array.Copy(prefix, 0, result, 0, prefix.Length);
            Array.Copy(formatData, 0, result, prefix.Length, formatLength);
            Array.Copy(ciper.IV, 0, result, formatData.Length + prefix.Length, ciper.IV.Length);
            Array.Copy(encryptedData, 0, result, result.Length - encryptedData.Length, encryptedData.Length);

            return result;
        }


        public byte[] Decrypt(byte[] data, out string format)
        {
            if (data == null) throw new ArgumentNullException();

            var vector = new byte[16];
            var prefix = GetPrefix();
            var minLength = prefix.Length + formatLength + vector.Length;

            if (data.Length < minLength) throw new ArgumentException($"data.Length cannot be less than {minLength}");

            // check prefix
            byte[] dataPrefix = new byte[prefix.Length];
            Array.Copy(data, 0, dataPrefix, 0, prefix.Length);
            if (!Convert.ToBase64String(prefix).Equals(Convert.ToBase64String(dataPrefix))) throw new FileFormatException();

            // get file format string
            byte[] formatData = new byte[formatLength];
            Array.Copy(data, prefix.Length, formatData, 0, formatLength);
            format = Encoding.UTF8.GetString(formatData).Trim();
            
            // get vector
            Array.Copy(data, formatLength + prefix.Length, vector, 0, vector.Length);

            // get crypted data
            byte[] buffer = new byte[data.Length - minLength];
            Array.Copy(data, minLength, buffer, 0, buffer.Length);

            // decrypt data and return result bytes
            var ciper = GetCiper();
            ciper.IV = vector;
            var decryptor = ciper.CreateDecryptor();
            var result = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);

            return result;
        }


        private byte[] GetPrefix()
        {
            return hash.ComputeHash(Encoding.Unicode.GetBytes(prefixString));
        }


        private RijndaelManaged GetCiper()
        { 
            var cryptKey = hash.ComputeHash(this.secureKey);

            var ciper = new RijndaelManaged();
            ciper.Key = cryptKey;
            ciper.BlockSize = 128;
            ciper.Padding = PaddingMode.ISO10126;
            ciper.Mode = CipherMode.CBC;

            return ciper;
        }
    }
}
