using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HashVerifier
{
    class HashProvider
    {
        private static readonly SHA256CryptoServiceProvider Sha256 = new SHA256CryptoServiceProvider();

        public static string GetHashSha256(FileStream fileStream)
        {
            var hashByte = Sha256.ComputeHash(fileStream);

            StringBuilder hashStringBuilder = new StringBuilder();

            //to hex string
            for (int i = 0; i < hashByte.Length; i++)
            {
                hashStringBuilder.Append(hashByte[i].ToString("x2"));
            }


            return hashStringBuilder.ToString();
        }
    }
}
