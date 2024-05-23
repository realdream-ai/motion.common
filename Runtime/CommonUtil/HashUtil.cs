using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using RealDream.Network;

namespace RealDream.AI
{
    public class HashUtil
    {
        public static string CalcHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(fileStream);
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        builder.Append(hashBytes[i].ToString("x2"));
                    }

                    return builder.ToString();
                }
            }
        }
    }
}