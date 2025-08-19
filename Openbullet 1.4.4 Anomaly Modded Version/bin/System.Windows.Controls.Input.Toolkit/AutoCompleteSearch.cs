// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.AutoCompleteSearch
// Assembly: System.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 14A20646-D206-4805-A36B-30DDEEBAF814
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Windows.Controls.Input.Toolkit.dll

#nullable disable
namespace System.Windows.Controls;

internal static class AutoCompleteSearch
{
  public static AutoCompleteFilterPredicate<string> GetFilter(AutoCompleteFilterMode FilterMode)
  {
    switch (FilterMode)
    {
      case AutoCompleteFilterMode.StartsWith:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.StartsWith);
      case AutoCompleteFilterMode.StartsWithCaseSensitive:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.StartsWithCaseSensitive);
      case AutoCompleteFilterMode.StartsWithOrdinal:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.StartsWithOrdinal);
      case AutoCompleteFilterMode.StartsWithOrdinalCaseSensitive:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.StartsWithOrdinalCaseSensitive);
      case AutoCompleteFilterMode.Contains:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.Contains);
      case AutoCompleteFilterMode.ContainsCaseSensitive:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.ContainsCaseSensitive);
      case AutoCompleteFilterMode.ContainsOrdinal:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.ContainsOrdinal);
      case AutoCompleteFilterMode.ContainsOrdinalCaseSensitive:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.ContainsOrdinalCaseSensitive);
      case AutoCompleteFilterMode.Equals:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.Equals);
      case AutoCompleteFilterMode.EqualsCaseSensitive:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.EqualsCaseSensitive);
      case AutoCompleteFilterMode.EqualsOrdinal:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.EqualsOrdinal);
      case AutoCompleteFilterMode.EqualsOrdinalCaseSensitive:
        return new AutoCompleteFilterPredicate<string>(AutoCompleteSearch.EqualsOrdinalCaseSensitive);
      default:
        return (AutoCompleteFilterPredicate<string>) null;
    }
  }

  public static bool StartsWith(string text, string value)
  {
    return value.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
  }

  public static bool StartsWithCaseSensitive(string text, string value)
  {
    return value.StartsWith(text, StringComparison.CurrentCulture);
  }

  public static bool StartsWithOrdinal(string text, string value)
  {
    return value.StartsWith(text, StringComparison.OrdinalIgnoreCase);
  }

  public static bool StartsWithOrdinalCaseSensitive(string text, string value)
  {
    return value.StartsWith(text, StringComparison.Ordinal);
  }

  public static bool Contains(string text, string value)
  {
    return value.Contains(text, StringComparison.CurrentCultureIgnoreCase);
  }

  public static bool ContainsCaseSensitive(string text, string value)
  {
    return value.Contains(text, StringComparison.CurrentCulture);
  }

  public static bool ContainsOrdinal(string text, string value)
  {
    return value.Contains(text, StringComparison.OrdinalIgnoreCase);
  }

  public static bool ContainsOrdinalCaseSensitive(string text, string value)
  {
    return value.Contains(text, StringComparison.Ordinal);
  }

  public static bool Equals(string text, string value)
  {
    return value.Equals(text, StringComparison.CurrentCultureIgnoreCase);
  }

  public static bool EqualsCaseSensitive(string text, string value)
  {
    return value.Equals(text, StringComparison.CurrentCulture);
  }

  public static bool EqualsOrdinal(string text, string value)
  {
    return value.Equals(text, StringComparison.OrdinalIgnoreCase);
  }

  public static bool EqualsOrdinalCaseSensitive(string text, string value)
  {
    return value.Equals(text, StringComparison.Ordinal);
  }
}
