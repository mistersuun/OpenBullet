// Decompiled with JetBrains decompiler
// Type: RuriLib.Functions.Crypto.Crypto
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace RuriLib.Functions.Crypto;

public static class Crypto
{
  public static string MD4(string input) => RuriLib.Functions.Crypto.Crypto.MD4Digest(Encoding.UTF8.GetBytes(input)).ToHex();

  public static string MD5(string input)
  {
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
      return md5.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex();
  }

  public static string HMACMD5(string input, string key, bool base64)
  {
    byte[] hash = new System.Security.Cryptography.HMACMD5(Encoding.ASCII.GetBytes(key)).ComputeHash(Encoding.ASCII.GetBytes(input));
    return base64 ? Convert.ToBase64String(hash) : hash.ToHex();
  }

  public static string SHA1(string input)
  {
    using (SHA1Managed shA1Managed = new SHA1Managed())
      return shA1Managed.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex();
  }

  public static string HMACSHA1(string input, string key, bool base64)
  {
    byte[] hash = new System.Security.Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key)).ComputeHash(Encoding.ASCII.GetBytes(input));
    return base64 ? Convert.ToBase64String(hash) : hash.ToHex();
  }

  public static string SHA256(string input)
  {
    using (SHA256Managed shA256Managed = new SHA256Managed())
      return shA256Managed.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex();
  }

  public static string HMACSHA256(string input, string key, bool base64)
  {
    byte[] hash = new System.Security.Cryptography.HMACSHA256(Encoding.ASCII.GetBytes(key)).ComputeHash(Encoding.ASCII.GetBytes(input));
    return base64 ? Convert.ToBase64String(hash) : hash.ToHex();
  }

  public static string SHA384(string input)
  {
    using (SHA384Managed shA384Managed = new SHA384Managed())
      return shA384Managed.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex();
  }

  public static string HMACSHA384(string input, string key, bool base64)
  {
    byte[] hash = new System.Security.Cryptography.HMACSHA384(Encoding.ASCII.GetBytes(key)).ComputeHash(Encoding.ASCII.GetBytes(input));
    return base64 ? Convert.ToBase64String(hash) : hash.ToHex();
  }

  public static string SHA512(string input)
  {
    using (SHA512Managed shA512Managed = new SHA512Managed())
      return shA512Managed.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex();
  }

  public static string HMACSHA512(string input, string key, bool base64)
  {
    byte[] hash = new System.Security.Cryptography.HMACSHA512(Encoding.ASCII.GetBytes(key)).ComputeHash(Encoding.ASCII.GetBytes(input));
    return base64 ? Convert.ToBase64String(hash) : hash.ToHex();
  }

  public static string ToHex(this byte[] bytes)
  {
    StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
    foreach (byte num in bytes)
      stringBuilder.Append(num.ToString("X2"));
    return stringBuilder.ToString();
  }

  public static byte[] FromHex(this string input)
  {
    byte[] numArray = new byte[input.Length / 2];
    for (int index = 0; index < numArray.Length; ++index)
      numArray[index] = Convert.ToByte(input.Substring(index * 2, 2), 16 /*0x10*/);
    return numArray;
  }

  public static HashAlgorithmName ToHashAlgorithmName(this Hash type)
  {
    switch (type)
    {
      case Hash.MD5:
        return HashAlgorithmName.MD5;
      case Hash.SHA1:
        return HashAlgorithmName.SHA1;
      case Hash.SHA256:
        return HashAlgorithmName.SHA256;
      case Hash.SHA384:
        return HashAlgorithmName.SHA384;
      case Hash.SHA512:
        return HashAlgorithmName.SHA512;
      default:
        throw new NotSupportedException("No such algorithm name");
    }
  }

  private static string RSAEncrypt(
    string dataToEncrypt,
    RSAParameters RSAKeyInfo,
    bool doOAEPPadding)
  {
    RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
    cryptoServiceProvider.ImportParameters(RSAKeyInfo);
    return Convert.ToBase64String(cryptoServiceProvider.Encrypt(Convert.FromBase64String(dataToEncrypt), doOAEPPadding));
  }

  private static string RSADecrypt(
    string dataToDecrypt,
    RSAParameters RSAKeyInfo,
    bool doOAEPPadding)
  {
    RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider();
    cryptoServiceProvider.ImportParameters(RSAKeyInfo);
    return Convert.ToBase64String(cryptoServiceProvider.Decrypt(Convert.FromBase64String(dataToDecrypt), doOAEPPadding));
  }

  public static string RSAEncrypt(
    string data,
    string password,
    string modulus,
    string exponent,
    bool oaep)
  {
    return RuriLib.Functions.Crypto.Crypto.RSAEncrypt(data, new RSAParameters()
    {
      D = Encoding.UTF8.GetBytes(password),
      Modulus = Encoding.UTF8.GetBytes(modulus),
      Exponent = Encoding.UTF8.GetBytes(exponent)
    }, (oaep ? 1 : 0) != 0);
  }

  public static string RSADecrypt(
    string data,
    string password,
    string modulus,
    string exponent,
    bool oaep)
  {
    return RuriLib.Functions.Crypto.Crypto.RSADecrypt(data, new RSAParameters()
    {
      D = Encoding.UTF8.GetBytes(password),
      Modulus = Encoding.UTF8.GetBytes(modulus),
      Exponent = Encoding.UTF8.GetBytes(exponent)
    }, (oaep ? 1 : 0) != 0);
  }

  public static string PBKDF2PKCS5(
    string password,
    string salt,
    int saltSize = 8,
    int iterations = 1,
    int keyLength = 16 /*0x10*/,
    Hash type = Hash.SHA1)
  {
    if (salt != "")
    {
      using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations, type.ToHashAlgorithmName()))
        return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(keyLength));
    }
    using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltSize, iterations, type.ToHashAlgorithmName()))
      return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(keyLength));
  }

  public static string AESEncrypt(
    string data,
    string key,
    string iv = "",
    CipherMode mode = CipherMode.CBC,
    PaddingMode padding = PaddingMode.None)
  {
    string str = (string) null;
    byte[][] hashKeys = RuriLib.Functions.Crypto.Crypto.GetHashKeys(key, iv);
    try
    {
      str = RuriLib.Functions.Crypto.Crypto.EncryptStringToBytes_Aes(data, hashKeys[0], hashKeys[1], mode, padding);
    }
    catch (CryptographicException ex)
    {
    }
    catch (ArgumentNullException ex)
    {
    }
    return str;
  }

  public static string AESDecrypt(
    string data,
    string key,
    string iv = "",
    CipherMode mode = CipherMode.CBC,
    PaddingMode padding = PaddingMode.None)
  {
    string str = (string) null;
    byte[][] hashKeys = RuriLib.Functions.Crypto.Crypto.GetHashKeys(key, iv);
    try
    {
      str = RuriLib.Functions.Crypto.Crypto.DecryptStringFromBytes_Aes(data, hashKeys[0], hashKeys[1], mode, padding);
    }
    catch (CryptographicException ex)
    {
    }
    catch (ArgumentNullException ex)
    {
    }
    return str;
  }

  private static byte[][] GetHashKeys(string key, string iv)
  {
    byte[][] hashKeys = new byte[2][];
    SHA256CryptoServiceProvider cryptoServiceProvider = new SHA256CryptoServiceProvider();
    byte[] buffer1 = Convert.FromBase64String(key);
    byte[] buffer2 = iv != "" ? Convert.FromBase64String(iv) : Convert.FromBase64String(key);
    byte[] hash1 = cryptoServiceProvider.ComputeHash(buffer1);
    byte[] hash2 = cryptoServiceProvider.ComputeHash(buffer2);
    Array.Resize<byte>(ref hash2, 16 /*0x10*/);
    hashKeys[0] = hash1;
    hashKeys[1] = hash2;
    return hashKeys;
  }

  private static string EncryptStringToBytes_Aes(
    string plainText,
    byte[] Key,
    byte[] IV,
    CipherMode mode,
    PaddingMode padding)
  {
    if (plainText == null || plainText.Length <= 0)
      throw new ArgumentNullException(nameof (plainText));
    if (Key == null || Key.Length == 0)
      throw new ArgumentNullException(nameof (Key));
    if (IV == null || IV.Length == 0)
      throw new ArgumentNullException(nameof (IV));
    byte[] array;
    using (AesManaged aesManaged = new AesManaged())
    {
      aesManaged.Key = Key;
      aesManaged.IV = IV;
      aesManaged.Mode = mode;
      aesManaged.Padding = padding;
      ICryptoTransform encryptor = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
        {
          using (StreamWriter streamWriter = new StreamWriter((Stream) cryptoStream))
            streamWriter.Write(plainText);
          array = memoryStream.ToArray();
        }
      }
    }
    return Convert.ToBase64String(array);
  }

  private static string DecryptStringFromBytes_Aes(
    string cipherTextString,
    byte[] Key,
    byte[] IV,
    CipherMode mode,
    PaddingMode padding)
  {
    byte[] buffer = Convert.FromBase64String(cipherTextString);
    if (buffer == null || buffer.Length == 0)
      throw new ArgumentNullException("cipherText");
    if (Key == null || Key.Length == 0)
      throw new ArgumentNullException(nameof (Key));
    if (IV == null || IV.Length == 0)
      throw new ArgumentNullException(nameof (IV));
    using (Aes aes = Aes.Create())
    {
      aes.Key = Key;
      aes.IV = IV;
      aes.Mode = mode;
      aes.Padding = padding;
      ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
      using (MemoryStream memoryStream = new MemoryStream(buffer))
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
        {
          using (StreamReader streamReader = new StreamReader((Stream) cryptoStream))
            return streamReader.ReadToEnd();
        }
      }
    }
  }

  private static byte[] MD4Digest(byte[] input)
  {
    List<byte> list = ((IEnumerable<byte>) input).ToList<byte>();
    uint num1 = (uint) (list.Count * 8);
    list.Add((byte) 128 /*0x80*/);
    while (list.Count % 64 /*0x40*/ != 56)
      list.Add((byte) 0);
    List<uint> uintList = new List<uint>();
    for (int index = 0; index + 3 < list.Count; index += 4)
      uintList.Add((uint) ((int) list[index] | (int) list[index + 1] << 8 | (int) list[index + 2] << 16 /*0x10*/ | (int) list[index + 3] << 24));
    uintList.Add(num1);
    uintList.Add(0U);
    uint a = 1732584193;
    uint b = 4023233417;
    uint c = 2562383102;
    uint d = 271733878;
    Func<uint, uint, uint> rol = (Func<uint, uint, uint>) ((x, y) => x << (int) y | x >> 32 /*0x20*/ - (int) y);
    for (int index = 0; index + 15 < uintList.Count; index += 16 /*0x10*/)
    {
      List<uint> chunk = uintList.GetRange(index, 16 /*0x10*/);
      uint num2 = a;
      uint num3 = b;
      uint num4 = c;
      uint num5 = d;
      Action<Func<uint, uint, uint, uint>, uint[]> action = (Action<Func<uint, uint, uint, uint>, uint[]>) ((f, y) =>
      {
        uint[] numArray = new uint[4]
        {
          y[0],
          y[1],
          y[2],
          y[3]
        };
        foreach (uint num6 in numArray)
        {
          a = rol(a + f(b, c, d) + chunk[(int) num6 + (int) y[4]] + y[12], y[8]);
          d = rol(d + f(a, b, c) + chunk[(int) num6 + (int) y[5]] + y[12], y[9]);
          c = rol(c + f(d, a, b) + chunk[(int) num6 + (int) y[6]] + y[12], y[10]);
          b = rol(b + f(c, d, a) + chunk[(int) num6 + (int) y[7]] + y[12], y[11]);
        }
      });
      action((Func<uint, uint, uint, uint>) ((x, y, z) => (uint) ((int) x & (int) y | ~(int) x & (int) z)), new uint[13]
      {
        0U,
        4U,
        8U,
        12U,
        0U,
        1U,
        2U,
        3U,
        3U,
        7U,
        11U,
        19U,
        0U
      });
      action((Func<uint, uint, uint, uint>) ((x, y, z) => (uint) ((int) x & (int) y | (int) x & (int) z | (int) y & (int) z)), new uint[13]
      {
        0U,
        1U,
        2U,
        3U,
        0U,
        4U,
        8U,
        12U,
        3U,
        5U,
        9U,
        13U,
        1518500249U
      });
      action((Func<uint, uint, uint, uint>) ((x, y, z) => x ^ y ^ z), new uint[13]
      {
        0U,
        2U,
        1U,
        3U,
        0U,
        8U,
        4U,
        12U,
        3U,
        9U,
        11U,
        15U,
        1859775393U
      });
      a += num2;
      b += num3;
      c += num4;
      d += num5;
    }
    return ((IEnumerable<uint>) new uint[4]{ a, b, c, d }).SelectMany<uint, byte>(new Func<uint, IEnumerable<byte>>(BitConverter.GetBytes)).ToArray<byte>();
  }
}
