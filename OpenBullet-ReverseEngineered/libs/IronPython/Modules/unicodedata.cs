// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.unicodedata
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class unicodedata
{
  private static unicodedata.UCD ucd_5_2_0;
  private static unicodedata.UCD _ucd_3_2_0;

  public static unicodedata.UCD ucd_3_2_0
  {
    get
    {
      if (unicodedata._ucd_3_2_0 == null)
        Interlocked.CompareExchange<unicodedata.UCD>(ref unicodedata._ucd_3_2_0, new unicodedata.UCD("3.2.0"), (unicodedata.UCD) null);
      return unicodedata._ucd_3_2_0;
    }
  }

  public static string unidata_version => unicodedata.ucd_5_2_0.unidata_version;

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, IDictionary dict)
  {
    if (unicodedata.ucd_5_2_0 != null)
      return;
    Interlocked.CompareExchange<unicodedata.UCD>(ref unicodedata.ucd_5_2_0, new unicodedata.UCD("5.2.0"), (unicodedata.UCD) null);
  }

  public static string lookup(string name) => unicodedata.ucd_5_2_0.lookup(name);

  public static string name(char unichr, string @default = null)
  {
    return unicodedata.ucd_5_2_0.name(unichr, @default);
  }

  public static int @decimal(char unichr, int @default)
  {
    return unicodedata.ucd_5_2_0.@decimal(unichr, @default);
  }

  public static int @decimal(char unichr) => unicodedata.ucd_5_2_0.@decimal(unichr);

  public static object @decimal(char unichr, object @default)
  {
    return unicodedata.ucd_5_2_0.@decimal(unichr, @default);
  }

  public static int digit(char unichr, int @default)
  {
    return unicodedata.ucd_5_2_0.digit(unichr, @default);
  }

  public static object digit(char unichr, object @default)
  {
    return unicodedata.ucd_5_2_0.digit(unichr, @default);
  }

  public static int digit(char unichr) => unicodedata.ucd_5_2_0.digit(unichr);

  public static double numeric(char unichr, double @default)
  {
    return unicodedata.ucd_5_2_0.numeric(unichr, @default);
  }

  public static double numeric(char unichr) => unicodedata.ucd_5_2_0.numeric(unichr);

  public static object numeric(char unichr, object @default)
  {
    return unicodedata.ucd_5_2_0.numeric(unichr, @default);
  }

  public static string category(char unichr) => unicodedata.ucd_5_2_0.category(unichr);

  public static string bidirectional(char unichr) => unicodedata.ucd_5_2_0.bidirectional(unichr);

  public static int combining(char unichr) => unicodedata.ucd_5_2_0.combining(unichr);

  public static string east_asian_width(char unichr)
  {
    return unicodedata.ucd_5_2_0.east_asian_width(unichr);
  }

  public static int mirrored(char unichr) => unicodedata.ucd_5_2_0.mirrored(unichr);

  public static string decomposition(char unichr) => unicodedata.ucd_5_2_0.decomposition(unichr);

  public static string normalize(string form, string unistr)
  {
    return unicodedata.ucd_5_2_0.normalize(form, unistr);
  }

  [PythonType("unicodedata.UCD")]
  public class UCD
  {
    private const string UnicodedataResourceName = "IronPython.Modules.unicodedata.IPyUnicodeData.txt.gz";
    private const string OtherNotAssigned = "Cn";
    private Dictionary<int, CharInfo> database;
    private List<RangeInfo> ranges;
    private Dictionary<string, int> nameLookup;

    public UCD(string version)
    {
      this.unidata_version = version;
      this.EnsureLoaded();
    }

    public string unidata_version { get; private set; }

    public string lookup(string name) => char.ConvertFromUtf32(this.nameLookup[name]);

    public string name(char unichr, string @default)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.Name : @default;
    }

    public string name(char unichr)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
        return charInfo.Name;
      throw PythonOps.ValueError("no such name");
    }

    public int @decimal(char unichr, int @default)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        int? numericValueDecimal = charInfo.Numeric_Value_Decimal;
        if (numericValueDecimal.HasValue)
          return numericValueDecimal.Value;
      }
      return @default;
    }

    public int @decimal(char unichr)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        int? numericValueDecimal = charInfo.Numeric_Value_Decimal;
        if (numericValueDecimal.HasValue)
          return numericValueDecimal.Value;
      }
      throw PythonOps.ValueError("not a decimal");
    }

    public object @decimal(char unichr, object @default)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        int? numericValueDecimal = charInfo.Numeric_Value_Decimal;
        if (numericValueDecimal.HasValue)
          return (object) numericValueDecimal.Value;
      }
      return @default;
    }

    public int digit(char unichr, int @default)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        int? numericValueDigit = charInfo.Numeric_Value_Digit;
        if (numericValueDigit.HasValue)
          return numericValueDigit.Value;
      }
      return @default;
    }

    public object digit(char unichr, object @default)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        int? numericValueDigit = charInfo.Numeric_Value_Digit;
        if (numericValueDigit.HasValue)
          return (object) numericValueDigit.Value;
      }
      return @default;
    }

    public int digit(char unichr)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        int? numericValueDigit = charInfo.Numeric_Value_Digit;
        if (numericValueDigit.HasValue)
          return numericValueDigit.Value;
      }
      throw PythonOps.ValueError("not a digit");
    }

    public double numeric(char unichr, double @default)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        double? numericValueNumeric = charInfo.Numeric_Value_Numeric;
        if (numericValueNumeric.HasValue)
          return numericValueNumeric.Value;
      }
      return @default;
    }

    public double numeric(char unichr)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        double? numericValueNumeric = charInfo.Numeric_Value_Numeric;
        if (numericValueNumeric.HasValue)
          return numericValueNumeric.Value;
      }
      throw PythonOps.ValueError("not a numeric character");
    }

    public object numeric(char unichr, object @default)
    {
      CharInfo charInfo;
      if (this.TryGetInfo(unichr, out charInfo))
      {
        double? numericValueNumeric = charInfo.Numeric_Value_Numeric;
        if (numericValueNumeric.HasValue)
          return (object) numericValueNumeric.Value;
      }
      return @default;
    }

    public string category(char unichr)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.General_Category : "Cn";
    }

    public string bidirectional(char unichr)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.Bidi_Class : string.Empty;
    }

    public int combining(char unichr)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.Canonical_Combining_Class : 0;
    }

    public string east_asian_width(char unichr)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.East_Asian_Width : string.Empty;
    }

    public int mirrored(char unichr)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.Bidi_Mirrored : 0;
    }

    public string decomposition(char unichr)
    {
      CharInfo charInfo;
      return this.TryGetInfo(unichr, out charInfo) ? charInfo.Decomposition_Type : string.Empty;
    }

    public string normalize(string form, string unistr)
    {
      NormalizationForm normalizationForm;
      switch (form)
      {
        case "NFC":
          normalizationForm = NormalizationForm.FormC;
          break;
        case "NFD":
          normalizationForm = NormalizationForm.FormD;
          break;
        case "NFKC":
          normalizationForm = NormalizationForm.FormKC;
          break;
        case "NFKD":
          normalizationForm = NormalizationForm.FormKD;
          break;
        default:
          throw new ArgumentException("Invalid normalization form " + form, nameof (form));
      }
      return unistr.Normalize(normalizationForm);
    }

    private void BuildDatabase(StreamReader data)
    {
      char[] separator = new char[1]{ ';' };
      this.database = new Dictionary<int, CharInfo>();
      this.ranges = new List<RangeInfo>();
      foreach (string readLine in data.ReadLines())
      {
        int num = readLine.IndexOf('#');
        string str = num == -1 ? readLine : readLine.Substring(readLine.Length - num).Trim();
        if (!string.IsNullOrEmpty(str))
        {
          string[] strArray = str.Split(separator, 2);
          System.Text.RegularExpressions.Match match = Regex.Match(strArray[0], "([0-9a-fA-F]{4})\\.\\.([0-9a-fA-F]{4})");
          if (match.Success)
            this.ranges.Add(new RangeInfo(Convert.ToInt32(match.Groups[1].Value, 16 /*0x10*/), Convert.ToInt32(match.Groups[2].Value, 16 /*0x10*/), strArray[1].Split(separator)));
          else
            this.database[Convert.ToInt32(strArray[0], 16 /*0x10*/)] = new CharInfo(strArray[1].Split(separator));
        }
      }
    }

    private void BuildNameLookup()
    {
      this.nameLookup = this.database.Where<KeyValuePair<int, CharInfo>>((Func<KeyValuePair<int, CharInfo>, bool>) (c => !c.Value.Name.StartsWith("<"))).ToDictionary<KeyValuePair<int, CharInfo>, string, int>((Func<KeyValuePair<int, CharInfo>, string>) (c => c.Value.Name), (Func<KeyValuePair<int, CharInfo>, int>) (c => c.Key));
    }

    private bool TryGetInfo(char unichr, out CharInfo charInfo)
    {
      if (this.database.TryGetValue((int) unichr, out charInfo))
        return true;
      foreach (RangeInfo range in this.ranges)
      {
        if (range.First <= (int) unichr && (int) unichr <= range.Last)
        {
          charInfo = (CharInfo) range;
          return true;
        }
      }
      return false;
    }

    private void EnsureLoaded()
    {
      if (this.database != null && this.nameLookup != null)
        return;
      this.BuildDatabase(new StreamReader((Stream) new GZipStream(typeof (unicodedata).Assembly.GetManifestResourceStream("IronPython.Modules.unicodedata.IPyUnicodeData.txt.gz"), CompressionMode.Decompress), Encoding.UTF8));
      this.BuildNameLookup();
    }
  }
}
