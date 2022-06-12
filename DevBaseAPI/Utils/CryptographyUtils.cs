using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseAPI.Utils
{
    public class CryptographyUtils
    {
        private static string HmacSignature(string input, string key)
        {
            return Convert.ToBase64String(new HMACSHA1(Encoding.ASCII.GetBytes(key)).ComputeHash(Encoding.ASCII.GetBytes(input)));
        } 
    }
}
