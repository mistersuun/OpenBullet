// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.Utility
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class Utility
{
  public const string Empty = "empty";
  public const string Null = "null";

  public static byte[] CloneByteArray(this byte[] src) => (byte[]) src.Clone();

  internal static string SerializeAsSingleCommaDelimitedString(IEnumerable<string> strings)
  {
    if (strings == null)
      return "null";
    StringBuilder stringBuilder = new StringBuilder();
    bool flag = true;
    foreach (string str in strings)
    {
      if (flag)
      {
        stringBuilder.AppendFormat("{0}", (object) (str ?? "null"));
        flag = false;
      }
      else
        stringBuilder.AppendFormat(", {0}", (object) (str ?? "null"));
    }
    return flag ? "empty" : stringBuilder.ToString();
  }

  public static bool IsHttps(string address)
  {
    if (string.IsNullOrEmpty(address))
      return false;
    try
    {
      Uri uri = new Uri(address);
      return Utility.IsHttps(new Uri(address));
    }
    catch (UriFormatException ex)
    {
      return false;
    }
  }

  public static bool IsHttps(Uri uri)
  {
    return !(uri == (Uri) null) && uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
  }

  [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
  public static bool AreEqual(byte[] a, byte[] b)
  {
    byte[] numArray1 = new byte[32 /*0x20*/]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 9,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      (byte) 16 /*0x10*/,
      (byte) 17,
      (byte) 18,
      (byte) 19,
      (byte) 20,
      (byte) 21,
      (byte) 22,
      (byte) 23,
      (byte) 24,
      (byte) 25,
      (byte) 26,
      (byte) 27,
      (byte) 28,
      (byte) 29,
      (byte) 30,
      (byte) 31 /*0x1F*/
    };
    byte[] numArray2 = new byte[32 /*0x20*/]
    {
      (byte) 31 /*0x1F*/,
      (byte) 30,
      (byte) 29,
      (byte) 28,
      (byte) 27,
      (byte) 26,
      (byte) 25,
      (byte) 24,
      (byte) 23,
      (byte) 22,
      (byte) 21,
      (byte) 20,
      (byte) 19,
      (byte) 18,
      (byte) 17,
      (byte) 16 /*0x10*/,
      (byte) 15,
      (byte) 14,
      (byte) 13,
      (byte) 12,
      (byte) 11,
      (byte) 10,
      (byte) 9,
      (byte) 8,
      (byte) 7,
      (byte) 6,
      (byte) 5,
      (byte) 4,
      (byte) 3,
      (byte) 2,
      (byte) 1,
      (byte) 0
    };
    int num = 0;
    byte[] numArray3;
    byte[] numArray4;
    if (a == null || b == null || a.Length != b.Length)
    {
      numArray3 = numArray1;
      numArray4 = numArray2;
    }
    else
    {
      numArray3 = a;
      numArray4 = b;
    }
    for (int index = 0; index < numArray3.Length; ++index)
      num |= (int) numArray3[index] ^ (int) numArray4[index];
    return num == 0;
  }

  [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
  internal static bool AreEqual(byte[] a, byte[] b, int length)
  {
    byte[] numArray1 = new byte[32 /*0x20*/]
    {
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 9,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      (byte) 16 /*0x10*/,
      (byte) 17,
      (byte) 18,
      (byte) 19,
      (byte) 20,
      (byte) 21,
      (byte) 22,
      (byte) 23,
      (byte) 24,
      (byte) 25,
      (byte) 26,
      (byte) 27,
      (byte) 28,
      (byte) 29,
      (byte) 30,
      (byte) 31 /*0x1F*/
    };
    byte[] numArray2 = new byte[32 /*0x20*/]
    {
      (byte) 31 /*0x1F*/,
      (byte) 30,
      (byte) 29,
      (byte) 28,
      (byte) 27,
      (byte) 26,
      (byte) 25,
      (byte) 24,
      (byte) 23,
      (byte) 22,
      (byte) 21,
      (byte) 20,
      (byte) 19,
      (byte) 18,
      (byte) 17,
      (byte) 16 /*0x10*/,
      (byte) 15,
      (byte) 14,
      (byte) 13,
      (byte) 12,
      (byte) 11,
      (byte) 10,
      (byte) 9,
      (byte) 8,
      (byte) 7,
      (byte) 6,
      (byte) 5,
      (byte) 4,
      (byte) 3,
      (byte) 2,
      (byte) 1,
      (byte) 0
    };
    int num1 = 0;
    byte[] numArray3;
    byte[] numArray4;
    int num2;
    if (a == null || b == null || a.Length < length || b.Length < length)
    {
      numArray3 = numArray1;
      numArray4 = numArray2;
      num2 = numArray3.Length;
    }
    else
    {
      numArray3 = a;
      numArray4 = b;
      num2 = length;
    }
    for (int index = 0; index < num2; ++index)
      num1 |= (int) numArray3[index] ^ (int) numArray4[index];
    return num1 == 0;
  }

  internal static byte[] ConvertToBigEndian(long i)
  {
    byte[] bytes = BitConverter.GetBytes(i);
    if (BitConverter.IsLittleEndian)
      Array.Reverse((Array) bytes);
    return bytes;
  }

  internal static byte[] Xor(byte[] a, byte[] b, int offset, bool inPlace)
  {
    if (inPlace)
    {
      for (int index = 0; index < a.Length; ++index)
        a[index] = (byte) ((uint) a[index] ^ (uint) b[offset + index]);
      return a;
    }
    byte[] numArray = new byte[a.Length];
    for (int index = 0; index < a.Length; ++index)
      numArray[index] = (byte) ((uint) a[index] ^ (uint) b[offset + index]);
    return numArray;
  }

  internal static void Zero(byte[] byteArray)
  {
    for (int index = 0; index < byteArray.Length; ++index)
      byteArray[index] = (byte) 0;
  }
}
