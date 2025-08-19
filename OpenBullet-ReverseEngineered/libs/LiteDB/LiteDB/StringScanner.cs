// Decompiled with JetBrains decompiler
// Type: LiteDB.StringScanner
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System.Text.RegularExpressions;

#nullable disable
namespace LiteDB;

public class StringScanner
{
  public string Source { get; private set; }

  public int Index { get; set; }

  public StringScanner(string source)
  {
    this.Source = source;
    this.Index = 0;
  }

  public override string ToString()
  {
    return !this.HasTerminated ? this.Source.Substring(this.Index) : "<EOF>";
  }

  public void Reset() => this.Index = 0;

  public void Seek(int length) => this.Index += length;

  public bool HasTerminated => this.Index >= this.Source.Length;

  public string Scan(string pattern)
  {
    return this.Scan(new Regex((pattern.StartsWith("^") ? "" : "^") + pattern, RegexOptions.IgnorePatternWhitespace));
  }

  public string Scan(Regex regex)
  {
    System.Text.RegularExpressions.Match match = regex.Match(this.Source, this.Index, this.Source.Length - this.Index);
    if (!match.Success)
      return string.Empty;
    this.Index += match.Length;
    return match.Value;
  }

  public string Scan(string pattern, int group)
  {
    return this.Scan(new Regex((pattern.StartsWith("^") ? "" : "^") + pattern, RegexOptions.IgnorePatternWhitespace), group);
  }

  public string Scan(Regex regex, int group)
  {
    System.Text.RegularExpressions.Match match = regex.Match(this.Source, this.Index, this.Source.Length - this.Index);
    if (!match.Success)
      return string.Empty;
    this.Index += match.Length;
    return group < match.Groups.Count ? match.Groups[group].Value : "";
  }

  public bool Match(string pattern)
  {
    return this.Match(new Regex((pattern.StartsWith("^") ? "" : "^") + pattern, RegexOptions.IgnorePatternWhitespace));
  }

  public bool Match(Regex regex)
  {
    return regex.Match(this.Source, this.Index, this.Source.Length - this.Index).Success;
  }

  public void ThrowIfNotFinish()
  {
    this.Scan("\\s*");
    if (!this.HasTerminated)
      throw LiteException.SyntaxError(this);
  }
}
