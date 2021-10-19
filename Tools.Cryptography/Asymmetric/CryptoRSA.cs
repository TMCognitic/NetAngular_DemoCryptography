using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Cryptography.Asymmetric
{
    public class CryptoRSA : ICryptoRSA
    {
        private readonly RSACryptoServiceProvider _cryptoServiceProvider;
        private readonly Encoding _encoding;

        public byte[] BothBinaryKeys
        {
            get
            {
                if (_cryptoServiceProvider.PublicOnly)
                    throw new InvalidOperationException();

                return _cryptoServiceProvider.ExportCspBlob(true);
            }
        }

        public byte[] PublicBinaryKey
        {
            get
            {
                return _cryptoServiceProvider.ExportCspBlob(false);
            }
        }

        public string BothXmlKeys
        {
            get
            {
                if (_cryptoServiceProvider.PublicOnly)
                    throw new InvalidOperationException();

                return _cryptoServiceProvider.ToXmlString(true);
            }
        }

        public string PublicXmlKey
        {
            get
            {
                return _cryptoServiceProvider.ToXmlString(false);
            }
        }

        public CryptoRSA() : this(2048)
        {

        }

        public CryptoRSA(int keySize) : this(keySize, Encoding.Unicode)
        {
        }

        public CryptoRSA(int keySize, Encoding encoding)
        {
            _cryptoServiceProvider = new RSACryptoServiceProvider(keySize);
            _encoding = encoding;
        }

        public CryptoRSA(byte[] binaryKeys) : this()
        {
            _cryptoServiceProvider.ImportCspBlob(binaryKeys);
        }

        public CryptoRSA(string xmlKeys) : this()
        {
            _cryptoServiceProvider.FromXmlString(xmlKeys);
        }

        public byte[] Encrypt(string data)
        {
            byte[] binaries = _encoding.GetBytes(data);
            return Encrypt(binaries);
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data.Length > (_cryptoServiceProvider.KeySize / 8) - 42)
                throw new InvalidOperationException("data is too long...");

            return _cryptoServiceProvider.Encrypt(data, true);
        }

        public string DecryptAsString(byte[] binaries)
        {
            binaries = Decrypt(binaries);
            return _encoding.GetString(binaries);
        }

        public byte[] Decrypt(byte[] binaries)
        {
            return _cryptoServiceProvider.Decrypt(binaries, true);
        }
    }
}
