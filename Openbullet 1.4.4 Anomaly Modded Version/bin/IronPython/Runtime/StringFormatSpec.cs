// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.StringFormatSpec
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;

#nullable disable
namespace IronPython.Runtime;

internal class StringFormatSpec
{
  internal readonly char? Fill;
  internal readonly char? Alignment;
  internal readonly char? Sign;
  internal readonly char? Type;
  internal readonly int? Width;
  internal readonly int? Precision;
  internal readonly bool IncludeType;
  internal readonly bool ThousandsComma;
  internal readonly bool IsEmpty;

  private StringFormatSpec(
    char? fill,
    char? alignment,
    char? sign,
    int? width,
    bool thousandsComma,
    int? precision,
    char? type,
    bool includeType,
    bool isEmpty)
  {
    this.Fill = fill;
    this.Alignment = alignment;
    this.Sign = sign;
    this.Width = width;
    this.ThousandsComma = thousandsComma;
    this.Precision = precision;
    this.Type = type;
    this.IncludeType = includeType;
    this.IsEmpty = isEmpty;
  }

  internal static StringFormatSpec FromString(string formatSpec)
  {
    char? fill = new char?();
    char? sign = new char?();
    char? alignment = new char?();
    char? type = new char?();
    int? width = new int?();
    int? precision = new int?();
    bool includeType = false;
    bool thousandsComma = false;
    bool isEmpty = formatSpec.Length == 0;
    int curOffset = 0;
    if (formatSpec.Length >= 2)
    {
      switch (formatSpec[1])
      {
        case '<':
        case '=':
        case '>':
        case '^':
          fill = new char?(formatSpec[0]);
          char? nullable1 = fill;
          int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
          int num = 0;
          if (nullable2.GetValueOrDefault() == num & nullable2.HasValue)
            fill = new char?(' ');
          alignment = new char?(formatSpec[1]);
          curOffset = 2;
          break;
        default:
          switch (formatSpec[0])
          {
            case '<':
            case '=':
            case '>':
            case '^':
              alignment = new char?(formatSpec[0]);
              curOffset = 1;
              break;
          }
          break;
      }
    }
    if (curOffset != formatSpec.Length && (formatSpec[curOffset] == '+' || formatSpec[curOffset] == '-' || formatSpec[curOffset] == ' '))
      sign = new char?(formatSpec[curOffset++]);
    if (curOffset != formatSpec.Length && formatSpec[curOffset] == '#')
    {
      includeType = true;
      ++curOffset;
    }
    if (curOffset != formatSpec.Length && formatSpec[curOffset] == '0')
    {
      if (!alignment.HasValue)
        alignment = new char?('=');
      if (!fill.HasValue)
        fill = new char?('0');
      ++curOffset;
    }
    if (curOffset != formatSpec.Length && char.IsDigit(formatSpec[curOffset]))
      width = StringFormatSpec.ParseInt(formatSpec, ref curOffset);
    if (curOffset != formatSpec.Length && formatSpec[curOffset] == ',')
    {
      ++curOffset;
      thousandsComma = true;
    }
    if (curOffset != formatSpec.Length && formatSpec[curOffset] == '.')
    {
      ++curOffset;
      if (curOffset == formatSpec.Length || !char.IsDigit(formatSpec[curOffset]))
        throw PythonOps.ValueError("Format specifier missing precision");
      precision = StringFormatSpec.ParseInt(formatSpec, ref curOffset);
    }
    if (curOffset != formatSpec.Length)
    {
      type = new char?(formatSpec[curOffset++]);
      if (thousandsComma && type.HasValue)
      {
        switch (type.GetValueOrDefault())
        {
          case 'X':
          case 'b':
          case 'c':
          case 'n':
          case 'o':
          case 'x':
            throw PythonOps.ValueError("Cannot specify ',' with '{0}'", (object) type);
        }
      }
    }
    return new StringFormatSpec(fill, alignment, sign, width, thousandsComma, precision, type, includeType, isEmpty);
  }

  internal string AlignText(string text)
  {
    if (this.Width.HasValue)
    {
      int num1 = this.Width.Value;
      if (text.Length < num1)
      {
        char? nullable = this.Fill;
        char c = (char) ((int) nullable ?? 32 /*0x20*/);
        int count1 = 0;
        int count2 = 0;
        int num2 = num1 - text.Length;
        nullable = this.Alignment;
        if (nullable.HasValue)
        {
          switch (nullable.GetValueOrDefault())
          {
            case '<':
              break;
            case '=':
            case '>':
              count1 = num2;
              goto label_8;
            case '^':
              count1 = count2 = num2 / 2;
              if ((num2 & 1) != 0)
              {
                ++count2;
                goto label_8;
              }
              goto label_8;
            default:
              goto label_8;
          }
        }
        count2 = num2;
label_8:
        if (count1 != 0)
          text = new string(c, count1) + text;
        if (count2 != 0)
          text += new string(c, count2);
      }
    }
    return text;
  }

  internal string AlignNumericText(string text, bool isZero, bool isPos)
  {
    char? sign = this.GetSign(isZero, isPos);
    string type = this.GetTypeString();
    if (this.Width.HasValue)
    {
      int num1 = this.Width.Value;
      if (text.Length < num1)
      {
        char? nullable1 = this.Fill;
        char c = (char) ((int) nullable1 ?? 32 /*0x20*/);
        int count1 = 0;
        int count2 = 0;
        int num2 = num1 - text.Length;
        if (sign.HasValue)
          --num2;
        if (type != null)
          num2 -= type.Length;
        nullable1 = this.Alignment;
        int? nullable2 = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        int num3 = 61;
        if (!(nullable2.GetValueOrDefault() == num3 & nullable2.HasValue) && sign.HasValue)
        {
          text = sign.Value.ToString() + type + text;
          sign = new char?();
          type = (string) null;
        }
        nullable1 = this.Alignment;
        if (nullable1.HasValue)
        {
          switch (nullable1.GetValueOrDefault())
          {
            case '<':
              count2 = num2;
              goto label_15;
            case '=':
              count1 = num2;
              goto label_15;
            case '>':
              break;
            case '^':
              count1 = count2 = num2 / 2;
              if ((num2 & 1) != 0)
              {
                ++count2;
                goto label_15;
              }
              goto label_15;
            default:
              goto label_15;
          }
        }
        count1 = num2;
label_15:
        if (count1 != 0)
        {
          text = (sign.HasValue ? sign.Value.ToString() : "") + type + new string(c, count1) + text;
        }
        else
        {
          if (sign.HasValue)
            text = sign.Value.ToString() + text;
          if (type != null)
            text = type + text;
        }
        if (count2 != 0)
          text += new string(c, count2);
      }
      else
        text = StringFormatSpec.FinishText(text, sign, type);
    }
    else
      text = StringFormatSpec.FinishText(text, sign, type);
    return text;
  }

  private static string FinishText(string text, char? sign, string type)
  {
    if (sign.HasValue)
      text = sign.Value.ToString() + type + text;
    else if (type != null)
      text = type + text;
    return text;
  }

  private string GetTypeString()
  {
    string typeString = (string) null;
    if (this.IncludeType)
    {
      char? type = this.Type;
      if (type.HasValue)
      {
        switch (type.GetValueOrDefault())
        {
          case 'X':
            typeString = "0X";
            break;
          case 'b':
            typeString = "0b";
            break;
          case 'o':
            typeString = "0o";
            break;
          case 'x':
            typeString = "0x";
            break;
        }
      }
    }
    return typeString;
  }

  private char? GetSign(bool isZero, bool isPos)
  {
    char? sign = this.Sign;
    if (sign.HasValue)
    {
      switch (sign.GetValueOrDefault())
      {
        case ' ':
          return isPos | isZero ? new char?(' ') : new char?('-');
        case '+':
          return isPos | isZero ? new char?('+') : new char?('-');
      }
    }
    return !isPos && !isZero ? new char?('-') : new char?();
  }

  private static int? ParseInt(string formatSpec, ref int curOffset)
  {
    int? nullable = new int?();
    int startIndex = curOffset;
    do
    {
      ++curOffset;
    }
    while (curOffset < formatSpec.Length && char.IsDigit(formatSpec[curOffset]));
    if (startIndex != curOffset)
      nullable = new int?(int.Parse(formatSpec.Substring(startIndex, curOffset - startIndex)));
    return nullable;
  }
}
