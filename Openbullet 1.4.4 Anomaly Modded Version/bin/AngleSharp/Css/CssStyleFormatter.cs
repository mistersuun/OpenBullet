// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.CssStyleFormatter
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace AngleSharp.Css;

public sealed class CssStyleFormatter : IStyleFormatter
{
  public static readonly IStyleFormatter Instance = (IStyleFormatter) new CssStyleFormatter();

  string IStyleFormatter.Sheet(IEnumerable<IStyleFormattable> rules)
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    string newLine = Environment.NewLine;
    using (StringWriter writer = new StringWriter(sb))
    {
      foreach (IStyleFormattable rule in rules)
      {
        rule.ToCss((TextWriter) writer, (IStyleFormatter) this);
        writer.Write(newLine);
      }
      if (sb.Length > 0)
        sb.Remove(sb.Length - newLine.Length, newLine.Length);
    }
    return sb.ToPool();
  }

  string IStyleFormatter.BlockRules(IEnumerable<IStyleFormattable> rules)
  {
    StringBuilder sb = StringBuilderPool.Obtain().Append('{');
    using (StringWriter stringWriter = new StringWriter(sb))
    {
      foreach (IStyleFormattable rule in rules)
      {
        stringWriter.Write(' ');
        StringWriter writer = stringWriter;
        rule.ToCss((TextWriter) writer, (IStyleFormatter) this);
      }
    }
    return sb.Append(' ').Append('}').ToPool();
  }

  string IStyleFormatter.Declaration(string name, string value, bool important)
  {
    return $"{name}: {value + (important ? " !important" : string.Empty)}";
  }

  string IStyleFormatter.BlockDeclarations(IEnumerable<IStyleFormattable> declarations)
  {
    StringBuilder sb = StringBuilderPool.Obtain().Append('{');
    using (StringWriter stringWriter = new StringWriter(sb))
    {
      foreach (IStyleFormattable declaration in declarations)
      {
        stringWriter.Write(' ');
        StringWriter writer = stringWriter;
        declaration.ToCss((TextWriter) writer, (IStyleFormatter) this);
        stringWriter.Write(';');
      }
      if (sb.Length > 1)
        sb.Remove(sb.Length - 1, 1);
    }
    return sb.Append(' ').Append('}').ToPool();
  }

  string IStyleFormatter.Rule(string name, string value) => $"{name} {value};";

  string IStyleFormatter.Rule(string name, string prelude, string rules)
  {
    return $"{name} {(string.IsNullOrEmpty(prelude) ? string.Empty : prelude + " ")}{rules}";
  }

  string IStyleFormatter.Comment(string data) => string.Join("/*", data, "*/");
}
