using System;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Helpers
{
    public static class Hasher
    {
        public static String MD5(String input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            Byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            Byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }


        public static String SHA1(String s)
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(s);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            Byte[] hashBytes = sha1.ComputeHash(bytes);
            return HexStringFromBytes(hashBytes);
        }

        public static String HexStringFromBytes(Byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

    }
}
