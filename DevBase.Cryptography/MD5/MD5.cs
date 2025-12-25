using System.Security.Cryptography;
using System.Text;

namespace DevBase.Cryptography.MD5;

/// <summary>
/// Provides methods for calculating MD5 hashes.
/// </summary>
public class MD5
{
    /// <summary>
    /// Computes the MD5 hash of the given string and returns it as a byte array.
    /// </summary>
    /// <param name="data">The input string to hash.</param>
    /// <returns>The MD5 hash as a byte array.</returns>
    public static byte[] ToMD5Binary(string data)
    {
        MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        byte[] compute = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(data));
        return compute;
    }
    
    /// <summary>
    /// Computes the MD5 hash of the given string and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="data">The input string to hash.</param>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
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
    
    /// <summary>
    /// Computes the MD5 hash of the given byte array and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="data">The input byte array to hash.</param>
    /// <returns>The MD5 hash as a hexadecimal string.</returns>
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