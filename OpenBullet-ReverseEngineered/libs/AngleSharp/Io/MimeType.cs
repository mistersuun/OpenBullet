// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.MimeType
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AngleSharp.Io;

public class MimeType : IEquatable<MimeType>
{
  private readonly string _general;
  private readonly string _media;
  private readonly string _suffix;
  private readonly string _params;

  public MimeType(string value)
  {
    int num1 = 0;
    while (num1 < value.Length && value[num1] != '/')
      ++num1;
    int num2 = num1;
    while (num2 < value.Length && value[num2] != '+')
      ++num2;
    int num3 = num2 < value.Length ? num2 : num1;
    while (num3 < value.Length && value[num3] != ';')
      ++num3;
    this._general = value.Substring(0, num1);
    this._media = num1 < value.Length ? value.Substring(num1 + 1, Math.Min(num2, num3) - num1 - 1) : string.Empty;
    this._suffix = num2 < value.Length ? value.Substring(num2 + 1, num3 - num2 - 1) : string.Empty;
    this._params = num3 < value.Length ? value.Substring(num3 + 1).StripLeadingTrailingSpaces() : string.Empty;
  }

  public string Content
  {
    get
    {
      return this._media.Length != 0 || this._suffix.Length != 0 ? $"{this._general}/{this._media}" + (this._suffix.Length > 0 ? "+" + this._suffix : string.Empty) : this._general;
    }
  }

  public string GeneralType => this._general;

  public string MediaType => this._media;

  public string Suffix => this._suffix;

  public IEnumerable<string> Keys
  {
    get
    {
      return ((IEnumerable<string>) this._params.Split(';')).Where<string>((Func<string, bool>) (m => !string.IsNullOrEmpty(m))).Select<string, string>((Func<string, string>) (m => m.IndexOf('=') < 0 ? m : m.Substring(0, m.IndexOf('='))));
    }
  }

  public string GetParameter(string key)
  {
    return ((IEnumerable<string>) this._params.Split(';')).Where<string>((Func<string, bool>) (m => m.StartsWith(key + "="))).Select<string, string>((Func<string, string>) (m => m.Substring(m.IndexOf('=') + 1))).FirstOrDefault<string>();
  }

  public override string ToString()
  {
    if (this._media.Length == 0 && this._suffix.Length == 0 && this._params.Length == 0)
      return this._general;
    string str1 = $"{this._general}/{this._media}";
    string str2 = this._suffix.Length > 0 ? "+" + this._suffix : string.Empty;
    string str3 = this._params.Length > 0 ? ";" + this._params : string.Empty;
    string str4 = str2;
    string str5 = str3;
    return str1 + str4 + str5;
  }

  public bool Equals(MimeType other)
  {
    return this._general.Isi(other._general) && this._media.Isi(other._media) && this._suffix.Isi(other._suffix);
  }

  public override bool Equals(object obj)
  {
    if ((object) this == obj)
      return true;
    MimeType other = obj as MimeType;
    return other != (MimeType) null && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return this._general.GetHashCode() << 2 ^ this._media.GetHashCode() << 1 ^ this._suffix.GetHashCode();
  }

  public static bool operator ==(MimeType a, MimeType b) => a.Equals(b);

  public static bool operator !=(MimeType a, MimeType b) => !a.Equals(b);
}
