using System;
using System.IO;

namespace CryptoPad.Models
{
    public class FileProvider
    {
        private string fileName;

        public FileProvider(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));
            this.fileName = fileName;
        }

        public byte[] LoadData()
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException();
            var buffer = File.ReadAllBytes(fileName);
            return buffer;
        }


        public void SaveData(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));   
            File.WriteAllBytes(fileName, data);
        }
    }
}
