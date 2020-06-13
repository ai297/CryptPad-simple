using System.Text;

namespace CryptoPad.Models
{
    public class TextDocument
    {   
        public const string FormatPrefix = "txt";

        public string TextContent { get; set; } = "";
        public Encoding Encoding { get; private set; } = Encoding.UTF8;

        public TextDocument() { }

        public TextDocument(byte[] data)
        {
            create(data);
        }
        public TextDocument(byte[] data, Encoding encoding)
        {
            Encoding = encoding;
            create(data);
        }

        public void ChangeEncodingTo(Encoding encoding)
        {
            var buffer = GetBytes();
            Encoding = encoding;
            create(buffer);
        }

        private void create(byte[] data)
        {
            var buffer = Encoding.Convert(Encoding, Encoding.Unicode, data);
            TextContent = Encoding.Unicode.GetString(buffer);
        }

        public byte[] GetBytes()
        {
            var buffer = Encoding.Unicode.GetBytes(TextContent);
            return Encoding.Convert(Encoding.Unicode, this.Encoding, buffer);
        }
    }
}