// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonLocale
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonLocale
{
  public const string __doc__ = "Provides access for querying and manipulating the current locale settings";
  private static readonly object _localeKey = new object();
  public const int CHAR_MAX = 127 /*0x7F*/;
  public const int LC_ALL = 0;
  public const int LC_COLLATE = 1;
  public const int LC_CTYPE = 2;
  public const int LC_MONETARY = 3;
  public const int LC_NUMERIC = 4;
  public const int LC_TIME = 5;

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    PythonLocale.EnsureLocaleInitialized(context);
    context.EnsureModuleException((object) "_localeerror", dict, "Error", "_locale");
  }

  internal static void EnsureLocaleInitialized(PythonContext context)
  {
    if (context.HasModuleState(PythonLocale._localeKey))
      return;
    context.SetModuleState(PythonLocale._localeKey, (object) new PythonLocale.LocaleInfo(context));
  }

  internal static string PreferredEncoding
  {
    get => "cp" + CultureInfo.CurrentCulture.TextInfo.ANSICodePage.ToString();
  }

  [Documentation("gets the default locale tuple")]
  public static object _getdefaultlocale()
  {
    return (object) PythonTuple.MakeTuple((object) PythonLocale.GetDefaultLocale(), (object) PythonLocale.PreferredEncoding);
  }

  [Documentation("gets the locale's conventions table.  \r\n\r\nThe conventions table is a dictionary that contains information on how to use \r\nthe locale for numeric and monetary formatting")]
  public static object localeconv(CodeContext context)
  {
    return (object) PythonLocale.GetLocaleInfo(context).GetConventionsTable();
  }

  [Documentation("Sets the current locale for the given category.\r\n\r\nLC_ALL:       sets locale for all options below\r\nLC_COLLATE:   sets locale for collation (strcoll and strxfrm) only\r\nLC_CTYPE:     sets locale for CType [unused]\r\nLC_MONETARY:  sets locale for the monetary functions (localeconv())\r\nLC_NUMERIC:   sets the locale for numeric functions (slocaleconv())\r\nLC_TIME:      sets the locale for time functions [unused]\r\n\r\nIf locale is None then the current setting is returned.\r\n")]
  public static object setlocale(CodeContext context, int category, string locale = null)
  {
    PythonLocale.LocaleInfo localeInfo = PythonLocale.GetLocaleInfo(context);
    switch (locale)
    {
      case null:
        return (object) localeInfo.GetLocale(context, category);
      case "":
        locale = PythonLocale.GetDefaultLocale();
        break;
    }
    return (object) localeInfo.SetLocale(context, category, locale);
  }

  [Documentation("compares two strings using the current locale")]
  public static int strcoll(CodeContext context, string string1, string string2)
  {
    return PythonLocale.GetLocaleInfo(context).Collate.CompareInfo.Compare(string1, string2, CompareOptions.None);
  }

  [Documentation("returns a System.Globalization.SortKey that can be compared using the built-in cmp.\r\n\r\nNote: Return value differs from CPython - it is not a string.")]
  public static object strxfrm(CodeContext context, string @string)
  {
    return (object) PythonLocale.GetLocaleInfo(context).Collate.CompareInfo.GetSortKey(@string);
  }

  private static string GetDefaultLocale()
  {
    return CultureInfo.CurrentCulture.Name.Replace('-', '_').Replace(' ', '_');
  }

  internal static PythonLocale.LocaleInfo GetLocaleInfo(CodeContext context)
  {
    PythonLocale.EnsureLocaleInitialized(context.LanguageContext);
    return (PythonLocale.LocaleInfo) context.LanguageContext.GetModuleState(PythonLocale._localeKey);
  }

  private static PythonType _localeerror(CodeContext context)
  {
    return (PythonType) context.LanguageContext.GetModuleState((object) nameof (_localeerror));
  }

  private enum LocaleCategories
  {
    All,
    Collate,
    CType,
    Monetary,
    Numeric,
    Time,
  }

  internal class LocaleInfo
  {
    private readonly PythonContext _context;
    private PythonDictionary conv;

    public LocaleInfo(PythonContext context) => this._context = context;

    public CultureInfo Collate
    {
      get => this._context.CollateCulture;
      set => this._context.CollateCulture = value;
    }

    public CultureInfo CType
    {
      get => this._context.CTypeCulture;
      set => this._context.CTypeCulture = value;
    }

    public CultureInfo Time
    {
      get => this._context.TimeCulture;
      set => this._context.TimeCulture = value;
    }

    public CultureInfo Monetary
    {
      get => this._context.MonetaryCulture;
      set => this._context.MonetaryCulture = value;
    }

    public CultureInfo Numeric
    {
      get => this._context.NumericCulture;
      set => this._context.NumericCulture = value;
    }

    public override string ToString() => base.ToString();

    public PythonDictionary GetConventionsTable()
    {
      this.CreateConventionsDict();
      return this.conv;
    }

    public string SetLocale(CodeContext context, int category, string locale)
    {
      switch (category)
      {
        case 0:
          this.SetLocale(context, 1, locale);
          this.SetLocale(context, 2, locale);
          this.SetLocale(context, 3, locale);
          this.SetLocale(context, 4, locale);
          return this.SetLocale(context, 5, locale);
        case 1:
          return this.CultureToName(this.Collate = this.LocaleToCulture(context, locale));
        case 2:
          return this.CultureToName(this.CType = this.LocaleToCulture(context, locale));
        case 3:
          this.Monetary = this.LocaleToCulture(context, locale);
          this.conv = (PythonDictionary) null;
          return this.CultureToName(this.Monetary);
        case 4:
          this.Numeric = this.LocaleToCulture(context, locale);
          this.conv = (PythonDictionary) null;
          return this.CultureToName(this.Numeric);
        case 5:
          return this.CultureToName(this.Time = this.LocaleToCulture(context, locale));
        default:
          throw PythonExceptions.CreateThrowable(PythonLocale._localeerror(context), (object) "unknown locale category");
      }
    }

    public string GetLocale(CodeContext context, int category)
    {
      switch (category)
      {
        case 0:
          if (this.Collate != this.CType || this.Collate != this.Time || this.Collate != this.Monetary || this.Collate != this.Numeric)
            return $"LC_COLLATE={this.GetLocale(context, 1)};LC_CTYPE={this.GetLocale(context, 2)};LC_MONETARY={this.GetLocale(context, 3)};LC_NUMERIC={this.GetLocale(context, 4)};LC_TIME={this.GetLocale(context, 5)}";
          goto case 1;
        case 1:
          return this.CultureToName(this.Collate);
        case 2:
          return this.CultureToName(this.CType);
        case 3:
          return this.CultureToName(this.Monetary);
        case 4:
          return this.CultureToName(this.Numeric);
        case 5:
          return this.CultureToName(this.Time);
        default:
          throw PythonExceptions.CreateThrowable(PythonLocale._localeerror(context), (object) "unknown locale category");
      }
    }

    public string CultureToName(CultureInfo culture)
    {
      return culture == PythonContext.CCulture ? "C" : culture.Name.Replace('-', '_');
    }

    private CultureInfo LocaleToCulture(CodeContext context, string locale)
    {
      if (locale == "C")
        return PythonContext.CCulture;
      locale = locale.Replace('_', '-');
      try
      {
        return StringUtils.GetCultureInfo(locale);
      }
      catch (ArgumentException ex)
      {
        throw PythonExceptions.CreateThrowable(PythonLocale._localeerror(context), (object) $"unknown locale: {locale}");
      }
    }

    private void CreateConventionsDict()
    {
      this.conv = new PythonDictionary();
      this.conv[(object) "decimal_point"] = (object) this.Numeric.NumberFormat.NumberDecimalSeparator;
      this.conv[(object) "grouping"] = (object) PythonLocale.LocaleInfo.GroupsToList(this.Numeric.NumberFormat.NumberGroupSizes);
      this.conv[(object) "thousands_sep"] = (object) this.Numeric.NumberFormat.NumberGroupSeparator;
      this.conv[(object) "mon_decimal_point"] = (object) this.Monetary.NumberFormat.CurrencyDecimalSeparator;
      this.conv[(object) "mon_thousands_sep"] = (object) this.Monetary.NumberFormat.CurrencyGroupSeparator;
      this.conv[(object) "mon_grouping"] = (object) PythonLocale.LocaleInfo.GroupsToList(this.Monetary.NumberFormat.CurrencyGroupSizes);
      this.conv[(object) "int_curr_symbol"] = (object) this.Monetary.NumberFormat.CurrencySymbol;
      this.conv[(object) "currency_symbol"] = (object) this.Monetary.NumberFormat.CurrencySymbol;
      this.conv[(object) "frac_digits"] = (object) this.Monetary.NumberFormat.CurrencyDecimalDigits;
      this.conv[(object) "int_frac_digits"] = (object) this.Monetary.NumberFormat.CurrencyDecimalDigits;
      this.conv[(object) "positive_sign"] = (object) this.Monetary.NumberFormat.PositiveSign;
      this.conv[(object) "negative_sign"] = (object) this.Monetary.NumberFormat.NegativeSign;
      this.conv[(object) "p_sign_posn"] = (object) this.Monetary.NumberFormat.CurrencyPositivePattern;
      this.conv[(object) "n_sign_posn"] = (object) this.Monetary.NumberFormat.CurrencyNegativePattern;
    }

    private static IronPython.Runtime.List GroupsToList(int[] groups)
    {
      IronPython.Runtime.List list = new IronPython.Runtime.List((ICollection) groups);
      if (groups.Length != 0 && groups[groups.Length - 1] == 0)
        list[list.__len__() - 1] = (object) (int) sbyte.MaxValue;
      else
        list.AddNoLock((object) 0);
      return list;
    }
  }
}
