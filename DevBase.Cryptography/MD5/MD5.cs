using System.Security.Cryptography;
using System.Text;

namespace DevBase.Cryptography.MD5;

public class MD5
{
    public static byte[] ToMD5Binary(string data)
    {
        MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        byte[] compute = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(data));
        return compute;
    }
    
    public static string ToMD5String(string data)
    {
        MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        byte[] compute = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(data));
        
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < compute.Length; i++)
        {
            strBuilder.Append(compute[i].ToString("x2"));
        }
        
        return strBuilder.ToString();
    }
    
     public static string ToMD5(byte[] data)
     {
         MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
         byte[] compute = md5CryptoServiceProvider.ComputeHash(data);
            
         StringBuilder strBuilder = new StringBuilder();
         for (int i = 0; i < compute.Length; i++)
         {
             strBuilder.Append(compute[i].ToString("x2"));
         }
            
         return strBuilder.ToString();
     }
}