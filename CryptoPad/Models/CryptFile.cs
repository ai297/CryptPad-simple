using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoPad.Models
{
    public class CryptFile
    {
        private DataCrypter dataCrypter;
        private Func<string> getSecurityKey;

        public const string CryptExtension = ".crpd";
        public TextDocument Document { get; }

        public string FileName { get; private set; }


        public CryptFile(Func<string> securityKeyGetter) {
            getSecurityKey = securityKeyGetter;
            Document = new TextDocument();
        }

        public CryptFile(string fileName, Func<string> securityKeyGetter)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException();
            getSecurityKey = securityKeyGetter;
            
            var fileProvider = new FileProvider(fileName);
            var buffer = fileProvider.LoadData();

            if (Path.GetExtension(fileName).Equals(CryptExtension))
            {
                var key = getSecurityKey.Invoke();
                dataCrypter = new DataCrypter(key);
                buffer = dataCrypter.Decrypt(buffer, out _);
            }

            FileName = fileName;
            Document = new TextDocument(buffer);
        }


        public void Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));

            var fileProvider = new FileProvider(fileName);
            byte[] buffer;

            if (Path.GetExtension(fileName).Equals(CryptExtension))
            {
                if (dataCrypter == null)
                {
                    var key = getSecurityKey.Invoke();
                    dataCrypter = new DataCrypter(key);
                }

                buffer = dataCrypter.Encrypt(Document.GetBytes(), TextDocument.FormatPrefix);
            }
            else buffer = Document.GetBytes();

            fileProvider.SaveData(buffer);
            FileName = fileName;
        }
    }
}
