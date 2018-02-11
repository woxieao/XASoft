using System;
using System.Security.Cryptography;
using System.Text;

namespace XASoft.Extensions
{
    public static class ToolsExtensions
    {
        private const string Salt = "ThisIsIt";

        /// <summary>
        /// 摘要MD5 hash
        /// </summary>
        /// <param name="input">需要签名的字符串</param>
        /// <param name="salt">加料,不加料请传空字符串</param>
        /// <returns>md5 string to upper</returns>
        public static string CalculateMd5WithSalt(this string input, string salt = Salt)
        {
            try
            {
                var md5 = MD5.Create();
                var inputBytes = Encoding.UTF8.GetBytes(input + salt);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                throw new Exception("MD5签名时出错", ex);
            }
        }
    }
}
