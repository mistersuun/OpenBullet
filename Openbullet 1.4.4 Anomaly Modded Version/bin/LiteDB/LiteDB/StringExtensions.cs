// Decompiled with JetBrains decompiler
// Type: LiteDB.StringExtensions
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

#nullable disable
namespace LiteDB;

internal static class StringExtensions
{
  public static bool IsNullOrWhiteSpace(this string str) => str == null || str.Trim().Length == 0;

  public static string ThrowIfEmpty(this string str, string message, StringScanner s)
  {
    return !string.IsNullOrEmpty(str) && str.Trim().Length != 0 ? str : throw LiteException.SyntaxError(s, message);
  }

  public static string TrimToNull(this string str)
  {
    string str1 = str.Trim();
    return str1.Length != 0 ? str1 : (string) null;
  }
}
