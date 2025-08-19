// Decompiled with JetBrains decompiler
// Type: AngleSharp.Css.Parser.CssTokenizer
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Text;
using System.Text;

#nullable disable
namespace AngleSharp.Css.Parser;

internal sealed class CssTokenizer
{
  private readonly StringSource _source;

  public CssTokenizer(StringSource source) => this._source = source;

  public CssSelectorToken Get() => this.Data(this._source.Current);

  private CssSelectorToken Data(char current)
  {
    switch (current)
    {
      case '\t':
      case '\n':
      case '\f':
      case '\r':
      case ' ':
        int num1 = (int) this._source.SkipSpaces();
        return new CssSelectorToken(CssTokenType.Whitespace, " ");
      case '!':
        current = this._source.Next();
        if (current != '=')
          return this.NewDelimiter('!');
        int num2 = (int) this._source.Next();
        return this.NewMatch(CombinatorSymbols.Unlike);
      case '"':
        return this.StringDQ();
      case '#':
        return this.HashStart();
      case '$':
        current = this._source.Next();
        if (current != '=')
          return this.NewDelimiter('$');
        int num3 = (int) this._source.Next();
        return this.NewMatch(CombinatorSymbols.Ends);
      case '\'':
        return this.StringSQ();
      case '(':
      case ';':
      case '{':
      case '}':
        int num4 = (int) this._source.Next();
        return this.NewInvalid();
      case ')':
        int num5 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.RoundBracketClose, ")");
      case '*':
        current = this._source.Next();
        if (current != '=')
          return this.NewDelimiter('*');
        int num6 = (int) this._source.Next();
        return this.NewMatch(CombinatorSymbols.InText);
      case '+':
        char c1 = this._source.Next();
        if (c1 != char.MaxValue)
        {
          char c2 = this._source.Next();
          int num7 = (int) this._source.Back();
          if (c1.IsDigit() || c1 == '.' && c2.IsDigit())
          {
            int num8 = (int) this._source.Back();
            return this.NumberStart(current);
          }
        }
        return this.NewDelimiter('+');
      case ',':
        int num9 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.Comma, ",");
      case '-':
        char c3 = this._source.Next();
        if (c3 != char.MaxValue)
        {
          char c4 = this._source.Next();
          int num10 = (int) this._source.Back(2);
          if (c3.IsDigit() || c3 == '.' && c4.IsDigit())
            return this.NumberStart(current);
          if (c3.IsNameStart() || c3 == '\\' && !c4.IsLineBreak() && c4 != char.MaxValue)
            return this.IdentStart(current);
          if (c3 == '-' && c4 == '>')
          {
            int num11 = (int) this._source.Next(2);
            return this.NewInvalid();
          }
          int num12 = (int) this._source.Next();
        }
        return this.NewDelimiter('-');
      case '.':
        current = this._source.Next();
        if (current.IsDigit())
          return this.NumberStart(this._source.Back());
        CssSelectorToken cssSelectorToken = this.Data(current);
        return cssSelectorToken.Type == CssTokenType.Ident ? new CssSelectorToken(CssTokenType.Class, cssSelectorToken.Data) : this.NewInvalid();
      case '/':
        current = this._source.Next();
        return current == '*' ? this.Data(this._source.SkipCssComment()) : this.NewDelimiter('/');
      case '0':
      case '1':
      case '2':
      case '3':
      case '4':
      case '5':
      case '6':
      case '7':
      case '8':
      case '9':
        return this.NumberStart(current);
      case ':':
        int num13 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.Colon, ":");
      case '<':
        current = this._source.Next();
        if (current == '!')
        {
          current = this._source.Next();
          if (current == '-')
          {
            current = this._source.Next();
            if (current == '-')
            {
              int num14 = (int) this._source.Next();
              return this.NewInvalid();
            }
            current = this._source.Back();
          }
          current = this._source.Back();
        }
        return this.NewDelimiter('<');
      case '>':
        current = this._source.Next();
        if (current != '>')
          return this.NewDelimiter('>');
        current = this._source.Next();
        if (current != '>')
          return new CssSelectorToken(CssTokenType.Descendent, ">>");
        int num15 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.Deep, ">>>");
      case '@':
        return this.AtKeywordStart();
      case 'U':
      case 'u':
        current = this._source.Next();
        if (current == '+')
        {
          current = this._source.Next();
          if (current.IsHex() || current == '?')
            return this.UnicodeRange(current);
          current = this._source.Back();
        }
        return this.IdentStart(this._source.Back());
      case '[':
        int num16 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.SquareBracketOpen, "[");
      case '\\':
        current = this._source.Next();
        return current.IsLineBreak() || current == char.MaxValue ? this.NewDelimiter('\\') : this.IdentStart(this._source.Back());
      case ']':
        int num17 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.SquareBracketClose, "]");
      case '^':
        current = this._source.Next();
        if (current != '=')
          return this.NewDelimiter('^');
        int num18 = (int) this._source.Next();
        return this.NewMatch(CombinatorSymbols.Begins);
      case '|':
        current = this._source.Next();
        if (current == '=')
        {
          int num19 = (int) this._source.Next();
          return this.NewMatch(CombinatorSymbols.InToken);
        }
        if (current != '|')
          return this.NewDelimiter('|');
        int num20 = (int) this._source.Next();
        return new CssSelectorToken(CssTokenType.Column, CombinatorSymbols.Column);
      case '~':
        current = this._source.Next();
        if (current != '=')
          return this.NewDelimiter('~');
        int num21 = (int) this._source.Next();
        return this.NewMatch(CombinatorSymbols.InList);
      case char.MaxValue:
        return new CssSelectorToken(CssTokenType.EndOfFile, string.Empty);
      default:
        if (current.IsNameStart())
          return this.IdentStart(current);
        int num22 = (int) this._source.Next();
        return this.NewDelimiter(current);
    }
  }

  private CssSelectorToken StringDQ()
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    while (true)
    {
      char ch = this._source.Next();
      switch (ch)
      {
        case '\n':
        case '\f':
          goto label_3;
        case '"':
        case char.MaxValue:
          goto label_2;
        case '\\':
          char c = this._source.Next();
          if (c.IsLineBreak())
          {
            sb.AppendLine();
            continue;
          }
          if (c != char.MaxValue)
          {
            int num = (int) this._source.Back();
            sb.Append(this._source.ConsumeEscape());
            continue;
          }
          goto label_8;
        default:
          sb.Append(ch);
          continue;
      }
    }
label_2:
    int num1 = (int) this._source.Next();
    return this.NewString(sb.ToPool());
label_3:
    return this.NewString(sb.ToPool());
label_8:
    return this.NewString(sb.ToPool());
  }

  private CssSelectorToken StringSQ()
  {
    StringBuilder sb = StringBuilderPool.Obtain();
    while (true)
    {
      char ch = this._source.Next();
      switch (ch)
      {
        case '\n':
        case '\f':
          goto label_3;
        case '\'':
        case char.MaxValue:
          goto label_2;
        case '\\':
          char c = this._source.Next();
          if (c.IsLineBreak())
          {
            sb.AppendLine();
            continue;
          }
          if (c != char.MaxValue)
          {
            int num = (int) this._source.Back();
            sb.Append(this._source.ConsumeEscape());
            continue;
          }
          goto label_8;
        default:
          sb.Append(ch);
          continue;
      }
    }
label_2:
    int num1 = (int) this._source.Next();
    return this.NewString(sb.ToPool());
label_3:
    return this.NewString(sb.ToPool());
label_8:
    return this.NewString(sb.ToPool());
  }

  private CssSelectorToken HashStart()
  {
    char c = this._source.Next();
    if (c.IsNameStart() || c == '-')
    {
      StringBuilder buffer = StringBuilderPool.Obtain();
      buffer.Append(c);
      return this.HashRest(buffer);
    }
    if (!this._source.IsValidEscape())
      return this.NewDelimiter('#');
    StringBuilder buffer1 = StringBuilderPool.Obtain();
    buffer1.Append(this._source.ConsumeEscape());
    return this.HashRest(buffer1);
  }

  private CssSelectorToken HashRest(StringBuilder buffer)
  {
    while (true)
    {
      char c = this._source.Next();
      if (c.IsName())
        buffer.Append(c);
      else if (this._source.IsValidEscape())
        buffer.Append(this._source.ConsumeEscape());
      else
        break;
    }
    return new CssSelectorToken(CssTokenType.Hash, buffer.ToPool());
  }

  private CssSelectorToken AtKeywordStart()
  {
    char c = this._source.Next();
    if (c == '-')
    {
      char ch = this._source.Next();
      if (ch.IsNameStart() || this._source.IsValidEscape())
        return this.AtKeywordRest(ch);
      int num = (int) this._source.Back();
      return this.NewDelimiter('@');
    }
    if (c.IsNameStart())
      return this.AtKeywordRest(this._source.Next());
    if (!this._source.IsValidEscape())
      return this.NewDelimiter('@');
    this._source.ConsumeEscape();
    return this.AtKeywordRest(this._source.Next());
  }

  private CssSelectorToken AtKeywordRest(char current)
  {
    while (true)
    {
      if (!current.IsName())
      {
        if (this._source.IsValidEscape())
          this._source.ConsumeEscape();
        else
          break;
      }
      current = this._source.Next();
    }
    return this.NewInvalid();
  }

  private CssSelectorToken IdentStart(char current)
  {
    if (current == '-')
    {
      current = this._source.Next();
      if (!current.IsNameStart() && !this._source.IsValidEscape())
        return this.NewDelimiter('-');
      StringBuilder buffer = StringBuilderPool.Obtain();
      buffer.Append('-');
      return this.IdentRest(current, buffer);
    }
    if (current.IsNameStart())
    {
      StringBuilder buffer = StringBuilderPool.Obtain();
      buffer.Append(current);
      return this.IdentRest(this._source.Next(), buffer);
    }
    if (current != '\\' || !this._source.IsValidEscape())
      return this.Data(current);
    StringBuilder buffer1 = StringBuilderPool.Obtain();
    buffer1.Append(this._source.ConsumeEscape());
    return this.IdentRest(this._source.Next(), buffer1);
  }

  private CssSelectorToken IdentRest(char current, StringBuilder buffer)
  {
    while (true)
    {
      if (current.IsName())
        buffer.Append(current);
      else if (this._source.IsValidEscape())
        buffer.Append(this._source.ConsumeEscape());
      else
        break;
      current = this._source.Next();
    }
    if (current != '(')
      return new CssSelectorToken(CssTokenType.Ident, buffer.ToPool());
    string pool = buffer.ToPool();
    if (pool.Isi(Keywords.Url))
      return this.UrlStart();
    int num = (int) this._source.Next();
    return new CssSelectorToken(CssTokenType.Function, pool);
  }

  private CssSelectorToken NumberStart(char current)
  {
    for (; !current.IsOneOf('+', '-'); current = this._source.Next())
    {
      if (current == '.')
      {
        StringBuilder buffer = StringBuilderPool.Obtain();
        buffer.Append(current).Append(this._source.Next());
        return this.NumberFraction(buffer);
      }
      if (current.IsDigit())
      {
        StringBuilder buffer = StringBuilderPool.Obtain();
        buffer.Append(current);
        return this.NumberRest(buffer);
      }
    }
    StringBuilder buffer1 = StringBuilderPool.Obtain();
    buffer1.Append(current);
    current = this._source.Next();
    if (current == '.')
    {
      buffer1.Append(current).Append(this._source.Next());
      return this.NumberFraction(buffer1);
    }
    buffer1.Append(current);
    return this.NumberRest(buffer1);
  }

  private CssSelectorToken NumberRest(StringBuilder buffer)
  {
    char ch;
    for (ch = this._source.Next(); ch.IsDigit(); ch = this._source.Next())
      buffer.Append(ch);
    if (ch.IsNameStart())
    {
      buffer.Append(ch);
      return this.Dimension(buffer);
    }
    if (this._source.IsValidEscape())
    {
      buffer.Append(this._source.ConsumeEscape());
      return this.Dimension(buffer);
    }
    switch (ch)
    {
      case '%':
        int num = (int) this._source.Next();
        return this.NewDimension(buffer.Append('%').ToPool());
      case '-':
        return this.NumberDash(buffer);
      case '.':
        char c = this._source.Next();
        if (!c.IsDigit())
          return this.NewNumber(buffer.ToPool());
        buffer.Append('.').Append(c);
        return this.NumberFraction(buffer);
      case 'E':
      case 'e':
        return this.NumberExponential(ch, buffer);
      default:
        return this.NewNumber(buffer.ToPool());
    }
  }

  private CssSelectorToken NumberFraction(StringBuilder buffer)
  {
    char ch;
    for (ch = this._source.Next(); ch.IsDigit(); ch = this._source.Next())
      buffer.Append(ch);
    if (ch.IsNameStart())
    {
      buffer.Append(ch);
      return this.Dimension(buffer);
    }
    if (this._source.IsValidEscape())
    {
      buffer.Append(this._source.ConsumeEscape());
      return this.Dimension(buffer);
    }
    switch (ch)
    {
      case '%':
        int num = (int) this._source.Next();
        return this.NewDimension(buffer.Append('%').ToPool());
      case '-':
        return this.NumberDash(buffer);
      case 'E':
      case 'e':
        return this.NumberExponential(ch, buffer);
      default:
        return this.NewNumber(buffer.ToPool());
    }
  }

  private CssSelectorToken Dimension(StringBuilder buffer)
  {
    while (true)
    {
      char c = this._source.Next();
      if (c.IsLetter())
        buffer.Append(c);
      else if (this._source.IsValidEscape())
        buffer.Append(this._source.ConsumeEscape());
      else
        break;
    }
    return this.NewDimension(buffer.ToPool());
  }

  private CssSelectorToken SciNotation(StringBuilder buffer)
  {
    while (true)
    {
      char c = this._source.Next();
      if (c.IsDigit())
        buffer.Append(c);
      else
        break;
    }
    return this.NewNumber(buffer.ToPool());
  }

  private CssSelectorToken UrlStart()
  {
    char current = this._source.SkipSpaces();
    switch (current)
    {
      case '"':
        return this.UrlDQ();
      case '\'':
        return this.UrlSQ();
      case ')':
        int num = (int) this._source.Next();
        return this.NewInvalid();
      case char.MaxValue:
        return this.NewInvalid();
      default:
        return this.UrlUQ(current);
    }
  }

  private CssSelectorToken UrlDQ()
  {
    while (true)
    {
      char c1;
      do
      {
        char c2 = this._source.Next();
        if (c2.IsLineBreak())
          return this.UrlBad();
        if (char.MaxValue == c2)
          return this.NewInvalid();
        switch (c2)
        {
          case '"':
            return this.UrlEnd();
          case '\\':
            c1 = this._source.Next();
            if (c1 == char.MaxValue)
              return this.NewInvalid();
            continue;
          default:
            continue;
        }
      }
      while (c1.IsLineBreak());
      int num = (int) this._source.Back();
      this._source.ConsumeEscape();
    }
  }

  private CssSelectorToken UrlSQ()
  {
    while (true)
    {
      char c1;
      do
      {
        char c2 = this._source.Next();
        if (c2.IsLineBreak())
          return this.UrlBad();
        switch (c2)
        {
          case '\'':
            return this.UrlEnd();
          case '\\':
            c1 = this._source.Next();
            if (c1 == char.MaxValue)
              return this.NewInvalid();
            continue;
          case char.MaxValue:
            return this.NewInvalid();
          default:
            continue;
        }
      }
      while (c1.IsLineBreak());
      int num = (int) this._source.Back();
      this._source.ConsumeEscape();
    }
  }

  private CssSelectorToken UrlUQ(char current)
  {
    for (; !current.IsSpaceCharacter(); current = this._source.Next())
    {
      if (current.IsOneOf(')', char.MaxValue))
      {
        int num = (int) this._source.Next();
        return this.NewInvalid();
      }
      if (current.IsOneOf('"', '\'', '(') || current.IsNonPrintable() || current != '\\' || !this._source.IsValidEscape())
        return this.UrlBad();
      this._source.ConsumeEscape();
    }
    return this.UrlEnd();
  }

  private CssSelectorToken UrlEnd()
  {
    char c;
    do
    {
      c = this._source.Next();
      if (c == ')')
      {
        int num = (int) this._source.Next();
        return this.NewInvalid();
      }
    }
    while (c.IsSpaceCharacter());
    int num1 = (int) this._source.Back();
    return this.UrlBad();
  }

  private CssSelectorToken UrlBad()
  {
    char ch = this._source.Current;
    int num = 0;
    for (int index = 1; ch != char.MaxValue && ch != ';' && (ch != '}' || --num != -1) && (ch != ')' || --index != 0); ch = this._source.Next())
    {
      if (this._source.IsValidEscape())
        this._source.ConsumeEscape();
      else if (ch == '(')
        ++index;
      else if (num == 123)
        ++num;
    }
    return this.NewInvalid();
  }

  private CssSelectorToken UnicodeRange(char current)
  {
    int num1 = 0;
    for (int index = 0; index < 6 && current.IsHex(); ++index)
    {
      ++num1;
      current = this._source.Next();
    }
    if (num1 != 6)
    {
      for (int index = 0; index < 6 - num1; ++index)
      {
        if (current != '?')
        {
          current = this._source.Back();
          break;
        }
        current = this._source.Next();
      }
      return this.NewInvalid();
    }
    if (current != '-')
      return this.NewInvalid();
    current = this._source.Next();
    if (current.IsHex())
    {
      for (int index = 0; index < 6; ++index)
      {
        if (!current.IsHex())
        {
          current = this._source.Back();
          break;
        }
        current = this._source.Next();
      }
      return new CssSelectorToken(CssTokenType.Invalid, string.Empty);
    }
    int num2 = (int) this._source.Back();
    return this.NewInvalid();
  }

  private CssSelectorToken NewMatch(string match)
  {
    return new CssSelectorToken(CssTokenType.Match, match);
  }

  private CssSelectorToken NewInvalid() => new CssSelectorToken(CssTokenType.Invalid, string.Empty);

  private CssSelectorToken NewString(string value)
  {
    return new CssSelectorToken(CssTokenType.String, value);
  }

  private CssSelectorToken NewDimension(string value)
  {
    return new CssSelectorToken(CssTokenType.Dimension, value);
  }

  private CssSelectorToken NewNumber(string number)
  {
    return new CssSelectorToken(CssTokenType.Number, number);
  }

  private CssSelectorToken NewDelimiter(char c)
  {
    return new CssSelectorToken(CssTokenType.Delim, c.ToString());
  }

  private CssSelectorToken NumberExponential(char letter, StringBuilder buffer)
  {
    char c1 = this._source.Next();
    if (c1.IsDigit())
    {
      buffer.Append(letter).Append(c1);
      return this.SciNotation(buffer);
    }
    if (c1 == '+' || c1 == '-')
    {
      char ch = c1;
      char c2 = this._source.Next();
      if (c2.IsDigit())
      {
        buffer.Append(letter).Append(ch).Append(c2);
        return this.SciNotation(buffer);
      }
      int num = (int) this._source.Back();
    }
    buffer.Append(letter);
    int num1 = (int) this._source.Back();
    return this.Dimension(buffer);
  }

  private CssSelectorToken NumberDash(StringBuilder buffer)
  {
    char c = this._source.Next();
    if (c.IsNameStart())
    {
      buffer.Append('-').Append(c);
      return this.Dimension(buffer);
    }
    if (this._source.IsValidEscape())
    {
      buffer.Append('-').Append(this._source.ConsumeEscape());
      return this.Dimension(buffer);
    }
    int num = (int) this._source.Back();
    return this.NewNumber(buffer.ToPool());
  }
}
