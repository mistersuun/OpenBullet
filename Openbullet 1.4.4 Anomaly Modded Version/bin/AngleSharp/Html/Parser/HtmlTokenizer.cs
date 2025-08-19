// Decompiled with JetBrains decompiler
// Type: AngleSharp.Html.Parser.HtmlTokenizer
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Dom.Events;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.Text;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace AngleSharp.Html.Parser;

internal sealed class HtmlTokenizer : BaseTokenizer
{
  private readonly IEntityProvider _resolver;
  private string _lastStartTag;
  private TextPosition _position;

  public event EventHandler<HtmlErrorEvent> Error;

  public HtmlTokenizer(TextSource source, IEntityProvider resolver)
    : base(source)
  {
    this.State = HtmlParseMode.PCData;
    this.IsAcceptingCharacterData = false;
    this.IsNotConsumingCharacterReferences = false;
    this.IsStrictMode = false;
    this._lastStartTag = string.Empty;
    this._resolver = resolver;
  }

  public bool IsAcceptingCharacterData { get; set; }

  public bool IsNotConsumingCharacterReferences { get; set; }

  public HtmlParseMode State { get; set; }

  public bool IsStrictMode { get; set; }

  public bool IsSupportingProcessingInstructions { get; set; }

  public HtmlToken Get()
  {
    char next = this.GetNext();
    this._position = this.GetCurrentPosition();
    if (next != char.MaxValue)
    {
      switch (this.State)
      {
        case HtmlParseMode.PCData:
          return this.Data(next);
        case HtmlParseMode.RCData:
          return this.RCData(next);
        case HtmlParseMode.Plaintext:
          return this.Plaintext(next);
        case HtmlParseMode.Rawtext:
          return this.Rawtext(next);
        case HtmlParseMode.Script:
          return this.ScriptData(next);
      }
    }
    return this.NewEof(true);
  }

  internal void RaiseErrorOccurred(HtmlParseError code, TextPosition position)
  {
    EventHandler<HtmlErrorEvent> error = this.Error;
    if (this.IsStrictMode)
    {
      string message = "Error while parsing the provided HTML document.";
      throw new HtmlParseException(code.GetCode(), message, position);
    }
    if (error == null)
      return;
    HtmlErrorEvent e = new HtmlErrorEvent(code, position);
    error((object) this, e);
  }

  private HtmlToken Data(char c) => c != '<' ? this.DataText(c) : this.TagOpen(this.GetNext());

  private HtmlToken DataText(char c)
  {
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          this.RaiseErrorOccurred(HtmlParseError.Null);
          break;
        case '&':
          this.AppendCharacterReference(this.GetNext());
          break;
        case '<':
        case char.MaxValue:
          goto label_1;
        default:
          this.StringBuffer.Append(c);
          break;
      }
      c = this.GetNext();
    }
label_1:
    this.Back();
    return this.NewCharacter();
  }

  private HtmlToken Plaintext(char c)
  {
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          this.AppendReplacement();
          break;
        case char.MaxValue:
          goto label_2;
        default:
          this.StringBuffer.Append(c);
          break;
      }
      c = this.GetNext();
    }
label_2:
    this.Back();
    return this.NewCharacter();
  }

  private HtmlToken RCData(char c) => c != '<' ? this.RCDataText(c) : this.RCDataLt(this.GetNext());

  private HtmlToken RCDataText(char c)
  {
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          this.AppendReplacement();
          break;
        case '&':
          this.AppendCharacterReference(this.GetNext());
          break;
        case '<':
        case char.MaxValue:
          goto label_2;
        default:
          this.StringBuffer.Append(c);
          break;
      }
      c = this.GetNext();
    }
label_2:
    this.Back();
    return this.NewCharacter();
  }

  private HtmlToken RCDataLt(char c)
  {
    if (c == '/')
    {
      c = this.GetNext();
      if (c.IsUppercaseAscii())
      {
        this.StringBuffer.Append(char.ToLowerInvariant(c));
        return this.RCDataNameEndTag(this.GetNext());
      }
      if (c.IsLowercaseAscii())
      {
        this.StringBuffer.Append(c);
        return this.RCDataNameEndTag(this.GetNext());
      }
      this.StringBuffer.Append('<').Append('/');
      return this.RCDataText(c);
    }
    this.StringBuffer.Append('<');
    return this.RCDataText(c);
  }

  private HtmlToken RCDataNameEndTag(char c)
  {
    HtmlToken ifAppropriate;
    while (true)
    {
      ifAppropriate = this.CreateIfAppropriate(c);
      if (ifAppropriate == null)
      {
        if (c.IsUppercaseAscii())
          this.StringBuffer.Append(char.ToLowerInvariant(c));
        else if (c.IsLowercaseAscii())
          this.StringBuffer.Append(c);
        else
          goto label_6;
        c = this.GetNext();
      }
      else
        break;
    }
    return ifAppropriate;
label_6:
    this.StringBuffer.Insert(0, '<').Insert(1, '/');
    return this.RCDataText(c);
  }

  private HtmlToken Rawtext(char c)
  {
    return c != '<' ? this.RawtextText(c) : this.RawtextLT(this.GetNext());
  }

  private HtmlToken RawtextText(char c)
  {
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          this.AppendReplacement();
          break;
        case '<':
        case char.MaxValue:
          goto label_1;
        default:
          this.StringBuffer.Append(c);
          break;
      }
      c = this.GetNext();
    }
label_1:
    this.Back();
    return this.NewCharacter();
  }

  private HtmlToken RawtextLT(char c)
  {
    if (c == '/')
    {
      c = this.GetNext();
      if (c.IsUppercaseAscii())
      {
        this.StringBuffer.Append(char.ToLowerInvariant(c));
        return this.RawtextNameEndTag(this.GetNext());
      }
      if (c.IsLowercaseAscii())
      {
        this.StringBuffer.Append(c);
        return this.RawtextNameEndTag(this.GetNext());
      }
      this.StringBuffer.Append('<').Append('/');
      return this.RawtextText(c);
    }
    this.StringBuffer.Append('<');
    return this.RawtextText(c);
  }

  private HtmlToken RawtextNameEndTag(char c)
  {
    HtmlToken ifAppropriate;
    while (true)
    {
      ifAppropriate = this.CreateIfAppropriate(c);
      if (ifAppropriate == null)
      {
        if (c.IsUppercaseAscii())
          this.StringBuffer.Append(char.ToLowerInvariant(c));
        else if (c.IsLowercaseAscii())
          this.StringBuffer.Append(c);
        else
          goto label_6;
        c = this.GetNext();
      }
      else
        break;
    }
    return ifAppropriate;
label_6:
    this.StringBuffer.Insert(0, '<').Insert(1, '/');
    return this.RawtextText(c);
  }

  private HtmlToken CharacterData(char c)
  {
    while (true)
    {
      switch (c)
      {
        case ']':
          if (!this.ContinuesWithSensitive("]]>"))
            break;
          goto label_3;
        case char.MaxValue:
          goto label_1;
      }
      this.StringBuffer.Append(c);
      c = this.GetNext();
    }
label_1:
    this.Back();
    goto label_5;
label_3:
    this.Advance(2);
label_5:
    return this.NewCharacter();
  }

  private void AppendCharacterReference(char c, char allowedCharacter = '\0')
  {
    if (this.IsNotConsumingCharacterReferences || c.IsSpaceCharacter() || c == '<' || c == char.MaxValue || c == '&' || (int) c == (int) allowedCharacter)
    {
      this.Back();
      this.StringBuffer.Append('&');
    }
    else
    {
      string str = c != '#' ? this.GetLookupCharacterReference(c, allowedCharacter) : this.GetNumericCharacterReference(this.GetNext());
      if (str == null)
        this.StringBuffer.Append('&');
      else
        this.StringBuffer.Append(str);
    }
  }

  private string GetNumericCharacterReference(char c)
  {
    int num1 = 10;
    int num2 = 1;
    int num3 = 0;
    List<int> intList = new List<int>();
    bool flag = c == 'x' || c == 'X';
    if (flag)
    {
      num1 = 16 /*0x10*/;
      while ((c = this.GetNext()).IsHex())
        intList.Add(c.FromHex());
    }
    else
    {
      for (; c.IsDigit(); c = this.GetNext())
        intList.Add(c.FromHex());
    }
    for (int index = intList.Count - 1; index >= 0; --index)
    {
      num3 += intList[index] * num2;
      num2 *= num1;
    }
    if (intList.Count == 0)
    {
      this.Back(2);
      if (flag)
        this.Back();
      this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceWrongNumber);
      return (string) null;
    }
    if (c != ';')
    {
      this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceSemicolonMissing);
      this.Back();
    }
    if (HtmlEntityProvider.IsInCharacterTable(num3))
    {
      this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceInvalidCode);
      return HtmlEntityProvider.GetSymbolFromTable(num3);
    }
    if (HtmlEntityProvider.IsInvalidNumber(num3))
    {
      this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceInvalidNumber);
      return '�'.ToString();
    }
    if (HtmlEntityProvider.IsInInvalidRange(num3))
      this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceInvalidRange);
    return char.ConvertFromUtf32(num3);
  }

  private string GetLookupCharacterReference(char c, char allowedCharacter)
  {
    string characterReference = (string) null;
    int num = this.InsertionPoint - 1;
    char[] chArray = new char[32 /*0x20*/];
    int index = 0;
    char c1 = this.Current;
    while (c1 != ';' && c1.IsName())
    {
      chArray[index++] = c1;
      c1 = this.GetNext();
      if (c1 == char.MaxValue || index >= 31 /*0x1F*/)
        break;
    }
    if (c1 == ';')
    {
      chArray[index] = ';';
      characterReference = this._resolver.GetSymbol(new string(chArray, 0, index + 1));
    }
    while (characterReference == null && index > 0)
    {
      characterReference = this._resolver.GetSymbol(new string(chArray, 0, index--));
      if (characterReference == null)
        this.Back();
    }
    char current = this.Current;
    if (current != ';')
    {
      if (allowedCharacter != char.MinValue && (current == '=' || current.IsAlphanumericAscii()))
      {
        if (current == '=')
          this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceAttributeEqualsFound);
        this.InsertionPoint = num;
        return (string) null;
      }
      this.Back();
      this.RaiseErrorOccurred(HtmlParseError.CharacterReferenceNotTerminated);
    }
    return characterReference;
  }

  private HtmlToken TagOpen(char c)
  {
    if (c == '/')
      return this.TagEnd(this.GetNext());
    if (c.IsLowercaseAscii())
    {
      this.StringBuffer.Append(c);
      return this.TagName(this.NewTagOpen());
    }
    if (c.IsUppercaseAscii())
    {
      this.StringBuffer.Append(char.ToLowerInvariant(c));
      return this.TagName(this.NewTagOpen());
    }
    if (c == '!')
      return this.MarkupDeclaration(this.GetNext());
    if (c == '?' && this.IsSupportingProcessingInstructions)
      return this.ProcessingInstruction(c);
    if (c != '?')
    {
      this.State = HtmlParseMode.PCData;
      this.RaiseErrorOccurred(HtmlParseError.AmbiguousOpenTag);
      this.StringBuffer.Append('<');
      return this.DataText(c);
    }
    this.RaiseErrorOccurred(HtmlParseError.BogusComment);
    return this.BogusComment(c);
  }

  private HtmlToken TagEnd(char c)
  {
    if (c.IsLowercaseAscii())
    {
      this.StringBuffer.Append(c);
      return this.TagName(this.NewTagClose());
    }
    if (c.IsUppercaseAscii())
    {
      this.StringBuffer.Append(char.ToLowerInvariant(c));
      return this.TagName(this.NewTagClose());
    }
    if (c == '>')
    {
      this.State = HtmlParseMode.PCData;
      this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
      return this.Data(this.GetNext());
    }
    if (c == char.MaxValue)
    {
      this.Back();
      this.RaiseErrorOccurred(HtmlParseError.EOF);
      this.StringBuffer.Append('<').Append('/');
      return this.NewCharacter();
    }
    this.RaiseErrorOccurred(HtmlParseError.BogusComment);
    return this.BogusComment(c);
  }

  private HtmlToken TagName(HtmlTagToken tag)
  {
    while (true)
    {
      char next = this.GetNext();
      if (next != '>')
      {
        if (!next.IsSpaceCharacter())
        {
          if (next != '/')
          {
            if (next.IsUppercaseAscii())
            {
              this.StringBuffer.Append(char.ToLowerInvariant(next));
            }
            else
            {
              switch (next)
              {
                case char.MinValue:
                  this.AppendReplacement();
                  continue;
                case char.MaxValue:
                  goto label_11;
                default:
                  this.StringBuffer.Append(next);
                  continue;
              }
            }
          }
          else
            goto label_5;
        }
        else
          goto label_3;
      }
      else
        break;
    }
    tag.Name = this.FlushBuffer((Func<StringBuilder, string>) (b => HtmlTagNameLookup.TryGetWellKnownTagName(b)));
    return this.EmitTag(tag);
label_3:
    tag.Name = this.FlushBuffer((Func<StringBuilder, string>) (b => HtmlTagNameLookup.TryGetWellKnownTagName(b)));
    return this.ParseAttributes(tag);
label_5:
    tag.Name = this.FlushBuffer((Func<StringBuilder, string>) (b => HtmlTagNameLookup.TryGetWellKnownTagName(b)));
    return this.TagSelfClosing(tag);
label_11:
    return this.NewEof();
  }

  private HtmlToken TagSelfClosing(HtmlTagToken tag)
  {
    return this.TagSelfClosingInner(tag) ?? this.ParseAttributes(tag);
  }

  private HtmlToken TagSelfClosingInner(HtmlTagToken tag)
  {
    while (true)
    {
      switch (this.GetNext())
      {
        case '/':
          this.RaiseErrorOccurred(HtmlParseError.ClosingSlashMisplaced);
          continue;
        case '>':
          goto label_1;
        case char.MaxValue:
          goto label_2;
        default:
          goto label_4;
      }
    }
label_1:
    tag.IsSelfClosing = true;
    return this.EmitTag(tag);
label_2:
    return this.NewEof();
label_4:
    this.RaiseErrorOccurred(HtmlParseError.ClosingSlashMisplaced);
    this.Back();
    return (HtmlToken) null;
  }

  private HtmlToken MarkupDeclaration(char c)
  {
    if (this.ContinuesWithSensitive("--"))
    {
      this.Advance();
      return this.CommentStart(this.GetNext());
    }
    if (this.ContinuesWithInsensitive(TagNames.Doctype))
    {
      this.Advance(6);
      return this.Doctype(this.GetNext());
    }
    if (this.IsAcceptingCharacterData && this.ContinuesWithSensitive(Keywords.CData))
    {
      this.Advance(6);
      return this.CharacterData(this.GetNext());
    }
    this.RaiseErrorOccurred(HtmlParseError.UndefinedMarkupDeclaration);
    return this.BogusComment(c);
  }

  private HtmlToken ProcessingInstruction(char c)
  {
    this.StringBuffer.Clear();
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          c = '�';
          break;
        case '>':
          goto label_5;
        case char.MaxValue:
          goto label_2;
      }
      this.StringBuffer.Append(c);
      c = this.GetNext();
    }
label_2:
    this.Back();
label_5:
    this.State = HtmlParseMode.PCData;
    return this.NewProcessingInstruction();
  }

  private HtmlToken BogusComment(char c)
  {
    this.StringBuffer.Clear();
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          c = '�';
          break;
        case '>':
          goto label_5;
        case char.MaxValue:
          goto label_2;
      }
      this.StringBuffer.Append(c);
      c = this.GetNext();
    }
label_2:
    this.Back();
label_5:
    this.State = HtmlParseMode.PCData;
    return this.NewComment();
  }

  private HtmlToken CommentStart(char c)
  {
    this.StringBuffer.Clear();
    switch (c)
    {
      case char.MinValue:
        this.AppendReplacement();
        return this.Comment(this.GetNext());
      case '-':
        return this.CommentDashStart(this.GetNext()) ?? this.Comment(this.GetNext());
      case '>':
        this.State = HtmlParseMode.PCData;
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        this.Back();
        break;
      default:
        this.StringBuffer.Append(c);
        return this.Comment(this.GetNext());
    }
    return this.NewComment();
  }

  private HtmlToken CommentDashStart(char c)
  {
    switch (c)
    {
      case char.MinValue:
        this.RaiseErrorOccurred(HtmlParseError.Null);
        this.StringBuffer.Append('-').Append('�');
        return this.Comment(this.GetNext());
      case '-':
        return this.CommentEnd(this.GetNext());
      case '>':
        this.State = HtmlParseMode.PCData;
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        this.Back();
        break;
      default:
        this.StringBuffer.Append('-').Append(c);
        return this.Comment(this.GetNext());
    }
    return this.NewComment();
  }

  private HtmlToken Comment(char c)
  {
    HtmlToken htmlToken;
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          this.AppendReplacement();
          break;
        case '-':
          htmlToken = this.CommentDashEnd(this.GetNext());
          if (htmlToken == null)
            break;
          goto label_2;
        case char.MaxValue:
          goto label_3;
        default:
          this.StringBuffer.Append(c);
          break;
      }
      c = this.GetNext();
    }
label_2:
    return htmlToken;
label_3:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    return this.NewComment();
  }

  private HtmlToken CommentDashEnd(char c)
  {
    switch (c)
    {
      case char.MinValue:
        this.RaiseErrorOccurred(HtmlParseError.Null);
        c = '�';
        break;
      case '-':
        return this.CommentEnd(this.GetNext());
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        this.Back();
        return this.NewComment();
    }
    this.StringBuffer.Append('-').Append(c);
    return (HtmlToken) null;
  }

  private HtmlToken CommentEnd(char c)
  {
    while (true)
    {
      switch (c)
      {
        case char.MinValue:
          goto label_2;
        case '!':
          goto label_3;
        case '-':
          this.RaiseErrorOccurred(HtmlParseError.CommentEndedWithDash);
          this.StringBuffer.Append('-');
          c = this.GetNext();
          continue;
        case '>':
          goto label_1;
        case char.MaxValue:
          goto label_5;
        default:
          goto label_6;
      }
    }
label_1:
    this.State = HtmlParseMode.PCData;
    return this.NewComment();
label_2:
    this.RaiseErrorOccurred(HtmlParseError.Null);
    this.StringBuffer.Append('-').Append('�');
    return (HtmlToken) null;
label_3:
    this.RaiseErrorOccurred(HtmlParseError.CommentEndedWithEM);
    return this.CommentBangEnd(this.GetNext());
label_5:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    return this.NewComment();
label_6:
    this.RaiseErrorOccurred(HtmlParseError.CommentEndedUnexpected);
    this.StringBuffer.Append('-').Append('-').Append(c);
    return (HtmlToken) null;
  }

  private HtmlToken CommentBangEnd(char c)
  {
    switch (c)
    {
      case char.MinValue:
        this.RaiseErrorOccurred(HtmlParseError.Null);
        this.StringBuffer.Append('-').Append('-').Append('!').Append('�');
        return (HtmlToken) null;
      case '-':
        this.StringBuffer.Append('-').Append('-').Append('!');
        return this.CommentDashEnd(this.GetNext());
      case '>':
        this.State = HtmlParseMode.PCData;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        this.Back();
        break;
      default:
        this.StringBuffer.Append('-').Append('-').Append('!').Append(c);
        return (HtmlToken) null;
    }
    return this.NewComment();
  }

  private HtmlToken Doctype(char c)
  {
    if (c.IsSpaceCharacter())
      return this.DoctypeNameBefore(this.GetNext());
    if (c == char.MaxValue)
    {
      this.RaiseErrorOccurred(HtmlParseError.EOF);
      this.Back();
      return (HtmlToken) this.NewDoctype(true);
    }
    this.RaiseErrorOccurred(HtmlParseError.DoctypeUnexpected);
    return this.DoctypeNameBefore(c);
  }

  private HtmlToken DoctypeNameBefore(char c)
  {
    while (c.IsSpaceCharacter())
      c = this.GetNext();
    if (c.IsUppercaseAscii())
    {
      HtmlDoctypeToken doctype = this.NewDoctype(false);
      this.StringBuffer.Append(char.ToLowerInvariant(c));
      return this.DoctypeName(doctype);
    }
    switch (c)
    {
      case char.MinValue:
        HtmlDoctypeToken doctype1 = this.NewDoctype(false);
        this.AppendReplacement();
        return this.DoctypeName(doctype1);
      case '>':
        HtmlDoctypeToken htmlDoctypeToken1 = this.NewDoctype(true);
        this.State = HtmlParseMode.PCData;
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        return (HtmlToken) htmlDoctypeToken1;
      case char.MaxValue:
        HtmlDoctypeToken htmlDoctypeToken2 = this.NewDoctype(true);
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        this.Back();
        return (HtmlToken) htmlDoctypeToken2;
      default:
        HtmlDoctypeToken doctype2 = this.NewDoctype(false);
        this.StringBuffer.Append(c);
        return this.DoctypeName(doctype2);
    }
  }

  private HtmlToken DoctypeName(HtmlDoctypeToken doctype)
  {
    while (true)
    {
      char next = this.GetNext();
      if (!next.IsSpaceCharacter())
      {
        if (next != '>')
        {
          if (next.IsUppercaseAscii())
          {
            this.StringBuffer.Append(char.ToLowerInvariant(next));
          }
          else
          {
            switch (next)
            {
              case char.MinValue:
                this.AppendReplacement();
                continue;
              case char.MaxValue:
                goto label_8;
              default:
                this.StringBuffer.Append(next);
                continue;
            }
          }
        }
        else
          goto label_3;
      }
      else
        break;
    }
    doctype.Name = this.FlushBuffer();
    return this.DoctypeNameAfter(doctype);
label_3:
    this.State = HtmlParseMode.PCData;
    doctype.Name = this.FlushBuffer();
    goto label_10;
label_8:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    doctype.IsQuirksForced = true;
    doctype.Name = this.FlushBuffer();
label_10:
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeNameAfter(HtmlDoctypeToken doctype)
  {
    switch (this.SkipSpaces())
    {
      case '>':
        this.State = HtmlParseMode.PCData;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        this.Back();
        doctype.IsQuirksForced = true;
        break;
      default:
        if (this.ContinuesWithInsensitive(Keywords.Public))
        {
          this.Advance(5);
          return this.DoctypePublic(doctype);
        }
        if (this.ContinuesWithInsensitive(Keywords.System))
        {
          this.Advance(5);
          return this.DoctypeSystem(doctype);
        }
        this.RaiseErrorOccurred(HtmlParseError.DoctypeUnexpectedAfterName);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypePublic(HtmlDoctypeToken doctype)
  {
    char next = this.GetNext();
    if (next.IsSpaceCharacter())
      return this.DoctypePublicIdentifierBefore(doctype);
    switch (next)
    {
      case '"':
        this.RaiseErrorOccurred(HtmlParseError.DoubleQuotationMarkUnexpected);
        doctype.PublicIdentifier = string.Empty;
        return this.DoctypePublicIdentifierDoubleQuoted(doctype);
      case '\'':
        this.RaiseErrorOccurred(HtmlParseError.SingleQuotationMarkUnexpected);
        doctype.PublicIdentifier = string.Empty;
        return this.DoctypePublicIdentifierSingleQuoted(doctype);
      case '>':
        this.State = HtmlParseMode.PCData;
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.IsQuirksForced = true;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypePublicInvalid);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypePublicIdentifierBefore(HtmlDoctypeToken doctype)
  {
    switch (this.SkipSpaces())
    {
      case '"':
        doctype.PublicIdentifier = string.Empty;
        return this.DoctypePublicIdentifierDoubleQuoted(doctype);
      case '\'':
        doctype.PublicIdentifier = string.Empty;
        return this.DoctypePublicIdentifierSingleQuoted(doctype);
      case '>':
        this.State = HtmlParseMode.PCData;
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.IsQuirksForced = true;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypePublicInvalid);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypePublicIdentifierDoubleQuoted(HtmlDoctypeToken doctype)
  {
    while (true)
    {
      char next = this.GetNext();
      switch (next)
      {
        case char.MinValue:
          this.AppendReplacement();
          continue;
        case '"':
          goto label_1;
        case '>':
          goto label_3;
        case char.MaxValue:
          goto label_4;
        default:
          this.StringBuffer.Append(next);
          continue;
      }
    }
label_1:
    doctype.PublicIdentifier = this.FlushBuffer();
    return this.DoctypePublicIdentifierAfter(doctype);
label_3:
    this.State = HtmlParseMode.PCData;
    this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
    doctype.IsQuirksForced = true;
    doctype.PublicIdentifier = this.FlushBuffer();
    goto label_6;
label_4:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    doctype.IsQuirksForced = true;
    doctype.PublicIdentifier = this.FlushBuffer();
label_6:
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypePublicIdentifierSingleQuoted(HtmlDoctypeToken doctype)
  {
    while (true)
    {
      char next = this.GetNext();
      switch (next)
      {
        case char.MinValue:
          this.AppendReplacement();
          continue;
        case '\'':
          goto label_1;
        case '>':
          goto label_3;
        case char.MaxValue:
          goto label_4;
        default:
          this.StringBuffer.Append(next);
          continue;
      }
    }
label_1:
    doctype.PublicIdentifier = this.FlushBuffer();
    return this.DoctypePublicIdentifierAfter(doctype);
label_3:
    this.State = HtmlParseMode.PCData;
    this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
    doctype.IsQuirksForced = true;
    doctype.PublicIdentifier = this.FlushBuffer();
    goto label_6;
label_4:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    doctype.IsQuirksForced = true;
    doctype.PublicIdentifier = this.FlushBuffer();
    this.Back();
label_6:
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypePublicIdentifierAfter(HtmlDoctypeToken doctype)
  {
    char next = this.GetNext();
    if (next.IsSpaceCharacter())
      return this.DoctypeBetween(doctype);
    switch (next)
    {
      case '"':
        this.RaiseErrorOccurred(HtmlParseError.DoubleQuotationMarkUnexpected);
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierDoubleQuoted(doctype);
      case '\'':
        this.RaiseErrorOccurred(HtmlParseError.SingleQuotationMarkUnexpected);
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierSingleQuoted(doctype);
      case '>':
        this.State = HtmlParseMode.PCData;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeBetween(HtmlDoctypeToken doctype)
  {
    switch (this.SkipSpaces())
    {
      case '"':
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierDoubleQuoted(doctype);
      case '\'':
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierSingleQuoted(doctype);
      case '>':
        this.State = HtmlParseMode.PCData;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeSystem(HtmlDoctypeToken doctype)
  {
    char next = this.GetNext();
    if (next.IsSpaceCharacter())
    {
      this.State = HtmlParseMode.PCData;
      return this.DoctypeSystemIdentifierBefore(doctype);
    }
    switch (next)
    {
      case '"':
        this.RaiseErrorOccurred(HtmlParseError.DoubleQuotationMarkUnexpected);
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierDoubleQuoted(doctype);
      case '\'':
        this.RaiseErrorOccurred(HtmlParseError.SingleQuotationMarkUnexpected);
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierSingleQuoted(doctype);
      case '>':
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.SystemIdentifier = this.FlushBuffer();
        doctype.IsQuirksForced = true;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeSystemInvalid);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeSystemIdentifierBefore(HtmlDoctypeToken doctype)
  {
    switch (this.SkipSpaces())
    {
      case '"':
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierDoubleQuoted(doctype);
      case '\'':
        doctype.SystemIdentifier = string.Empty;
        return this.DoctypeSystemIdentifierSingleQuoted(doctype);
      case '>':
        this.State = HtmlParseMode.PCData;
        this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
        doctype.IsQuirksForced = true;
        doctype.SystemIdentifier = this.FlushBuffer();
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        doctype.SystemIdentifier = this.FlushBuffer();
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        doctype.IsQuirksForced = true;
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeSystemIdentifierDoubleQuoted(HtmlDoctypeToken doctype)
  {
    while (true)
    {
      char next = this.GetNext();
      switch (next)
      {
        case char.MinValue:
          this.AppendReplacement();
          continue;
        case '"':
          goto label_1;
        case '>':
          goto label_3;
        case char.MaxValue:
          goto label_4;
        default:
          this.StringBuffer.Append(next);
          continue;
      }
    }
label_1:
    doctype.SystemIdentifier = this.FlushBuffer();
    return this.DoctypeSystemIdentifierAfter(doctype);
label_3:
    this.State = HtmlParseMode.PCData;
    this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
    doctype.IsQuirksForced = true;
    doctype.SystemIdentifier = this.FlushBuffer();
    goto label_6;
label_4:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    doctype.IsQuirksForced = true;
    doctype.SystemIdentifier = this.FlushBuffer();
    this.Back();
label_6:
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeSystemIdentifierSingleQuoted(HtmlDoctypeToken doctype)
  {
    while (true)
    {
      char next = this.GetNext();
      switch (next)
      {
        case char.MinValue:
          this.AppendReplacement();
          continue;
        case '\'':
          goto label_1;
        case '>':
          goto label_3;
        case char.MaxValue:
          goto label_4;
        default:
          this.StringBuffer.Append(next);
          continue;
      }
    }
label_1:
    doctype.SystemIdentifier = this.FlushBuffer();
    return this.DoctypeSystemIdentifierAfter(doctype);
label_3:
    this.State = HtmlParseMode.PCData;
    this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
    doctype.IsQuirksForced = true;
    doctype.SystemIdentifier = this.FlushBuffer();
    goto label_6;
label_4:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    doctype.IsQuirksForced = true;
    doctype.SystemIdentifier = this.FlushBuffer();
    this.Back();
label_6:
    return (HtmlToken) doctype;
  }

  private HtmlToken DoctypeSystemIdentifierAfter(HtmlDoctypeToken doctype)
  {
    switch (this.SkipSpaces())
    {
      case '>':
        this.State = HtmlParseMode.PCData;
        break;
      case char.MaxValue:
        this.RaiseErrorOccurred(HtmlParseError.EOF);
        doctype.IsQuirksForced = true;
        this.Back();
        break;
      default:
        this.RaiseErrorOccurred(HtmlParseError.DoctypeInvalidCharacter);
        return this.BogusDoctype(doctype);
    }
    return (HtmlToken) doctype;
  }

  private HtmlToken BogusDoctype(HtmlDoctypeToken doctype)
  {
    char next;
    do
    {
      next = this.GetNext();
      if (next == '>')
        goto label_2;
    }
    while (next != char.MaxValue);
    goto label_3;
label_2:
    this.State = HtmlParseMode.PCData;
    goto label_4;
label_3:
    this.Back();
label_4:
    return (HtmlToken) doctype;
  }

  private HtmlToken ParseAttributes(HtmlTagToken tag)
  {
    HtmlTokenizer.AttributeState attributeState = HtmlTokenizer.AttributeState.BeforeName;
    char allowedCharacter = '"';
    char c = char.MinValue;
    TextPosition currentPosition = this.GetCurrentPosition();
    HtmlToken attributes;
    while (true)
    {
      switch (attributeState)
      {
        case HtmlTokenizer.AttributeState.BeforeName:
          c = this.SkipSpaces();
          switch (c)
          {
            case '/':
              attributeState = HtmlTokenizer.AttributeState.SelfClose;
              continue;
            case '>':
              goto label_4;
            default:
              if (c.IsUppercaseAscii())
              {
                this.StringBuffer.Append(char.ToLowerInvariant(c));
                currentPosition = this.GetCurrentPosition();
                attributeState = HtmlTokenizer.AttributeState.Name;
                continue;
              }
              switch (c)
              {
                case char.MinValue:
                  this.AppendReplacement();
                  currentPosition = this.GetCurrentPosition();
                  attributeState = HtmlTokenizer.AttributeState.Name;
                  continue;
                case '"':
                case '\'':
                case '<':
                case '=':
                  this.RaiseErrorOccurred(HtmlParseError.AttributeNameInvalid);
                  this.StringBuffer.Append(c);
                  currentPosition = this.GetCurrentPosition();
                  attributeState = HtmlTokenizer.AttributeState.Name;
                  continue;
                case char.MaxValue:
                  goto label_11;
                default:
                  this.StringBuffer.Append(c);
                  currentPosition = this.GetCurrentPosition();
                  attributeState = HtmlTokenizer.AttributeState.Name;
                  continue;
              }
          }
        case HtmlTokenizer.AttributeState.Name:
          c = this.GetNext();
          switch (c)
          {
            case '=':
              tag.AddAttribute(this.FlushBuffer(), currentPosition);
              attributeState = HtmlTokenizer.AttributeState.BeforeValue;
              continue;
            case '>':
              goto label_14;
            default:
              if (c.IsSpaceCharacter())
              {
                tag.AddAttribute(this.FlushBuffer(), currentPosition);
                attributeState = HtmlTokenizer.AttributeState.AfterName;
                continue;
              }
              if (c == '/')
              {
                tag.AddAttribute(this.FlushBuffer(), currentPosition);
                attributeState = HtmlTokenizer.AttributeState.SelfClose;
                continue;
              }
              if (c.IsUppercaseAscii())
              {
                this.StringBuffer.Append(char.ToLowerInvariant(c));
                continue;
              }
              switch (c)
              {
                case char.MinValue:
                  this.AppendReplacement();
                  continue;
                case '"':
                case '\'':
                case '<':
                  this.RaiseErrorOccurred(HtmlParseError.AttributeNameInvalid);
                  this.StringBuffer.Append(c);
                  continue;
                case char.MaxValue:
                  goto label_25;
                default:
                  this.StringBuffer.Append(c);
                  continue;
              }
          }
        case HtmlTokenizer.AttributeState.AfterName:
          c = this.SkipSpaces();
          switch (c)
          {
            case '/':
              attributeState = HtmlTokenizer.AttributeState.SelfClose;
              continue;
            case '=':
              attributeState = HtmlTokenizer.AttributeState.BeforeValue;
              continue;
            case '>':
              goto label_27;
            default:
              if (c.IsUppercaseAscii())
              {
                this.StringBuffer.Append(char.ToLowerInvariant(c));
                currentPosition = this.GetCurrentPosition();
                attributeState = HtmlTokenizer.AttributeState.Name;
                continue;
              }
              switch (c)
              {
                case char.MinValue:
                  this.AppendReplacement();
                  currentPosition = this.GetCurrentPosition();
                  attributeState = HtmlTokenizer.AttributeState.Name;
                  continue;
                case '"':
                case '\'':
                case '<':
                  this.RaiseErrorOccurred(HtmlParseError.AttributeNameInvalid);
                  this.StringBuffer.Append(c);
                  currentPosition = this.GetCurrentPosition();
                  attributeState = HtmlTokenizer.AttributeState.Name;
                  continue;
                case char.MaxValue:
                  goto label_36;
                default:
                  this.StringBuffer.Append(c);
                  currentPosition = this.GetCurrentPosition();
                  attributeState = HtmlTokenizer.AttributeState.Name;
                  continue;
              }
          }
        case HtmlTokenizer.AttributeState.BeforeValue:
          c = this.SkipSpaces();
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              attributeState = HtmlTokenizer.AttributeState.UnquotedValue;
              c = this.GetNext();
              continue;
            case '"':
            case '\'':
              attributeState = HtmlTokenizer.AttributeState.QuotedValue;
              allowedCharacter = c;
              continue;
            case '&':
              attributeState = HtmlTokenizer.AttributeState.UnquotedValue;
              continue;
            case '<':
            case '=':
            case '`':
              this.RaiseErrorOccurred(HtmlParseError.AttributeValueInvalid);
              this.StringBuffer.Append(c);
              attributeState = HtmlTokenizer.AttributeState.UnquotedValue;
              c = this.GetNext();
              continue;
            case '>':
              goto label_40;
            case char.MaxValue:
              goto label_44;
            default:
              this.StringBuffer.Append(c);
              attributeState = HtmlTokenizer.AttributeState.UnquotedValue;
              c = this.GetNext();
              continue;
          }
        case HtmlTokenizer.AttributeState.QuotedValue:
          c = this.GetNext();
          if ((int) c == (int) allowedCharacter)
          {
            tag.SetAttributeValue(this.FlushBuffer());
            attributeState = HtmlTokenizer.AttributeState.AfterValue;
            continue;
          }
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              continue;
            case '&':
              this.AppendCharacterReference(this.GetNext(), allowedCharacter);
              continue;
            case char.MaxValue:
              goto label_51;
            default:
              this.StringBuffer.Append(c);
              continue;
          }
        case HtmlTokenizer.AttributeState.AfterValue:
          c = this.GetNext();
          if (c != '>')
          {
            if (c.IsSpaceCharacter())
            {
              attributeState = HtmlTokenizer.AttributeState.BeforeName;
              continue;
            }
            switch (c)
            {
              case '/':
                attributeState = HtmlTokenizer.AttributeState.SelfClose;
                continue;
              case char.MaxValue:
                goto label_68;
              default:
                this.RaiseErrorOccurred(HtmlParseError.AttributeNameExpected);
                this.Back();
                attributeState = HtmlTokenizer.AttributeState.BeforeName;
                continue;
            }
          }
          else
            goto label_63;
        case HtmlTokenizer.AttributeState.UnquotedValue:
          if (c != '>')
          {
            if (c.IsSpaceCharacter())
            {
              tag.SetAttributeValue(this.FlushBuffer());
              attributeState = HtmlTokenizer.AttributeState.BeforeName;
              continue;
            }
            switch (c)
            {
              case char.MinValue:
                this.AppendReplacement();
                c = this.GetNext();
                continue;
              case '"':
              case '\'':
              case '<':
              case '=':
              case '`':
                this.RaiseErrorOccurred(HtmlParseError.AttributeValueInvalid);
                this.StringBuffer.Append(c);
                c = this.GetNext();
                continue;
              case '&':
                this.AppendCharacterReference(this.GetNext(), '>');
                c = this.GetNext();
                continue;
              case char.MaxValue:
                goto label_61;
              default:
                this.StringBuffer.Append(c);
                c = this.GetNext();
                continue;
            }
          }
          else
            goto label_53;
        case HtmlTokenizer.AttributeState.SelfClose:
          attributes = this.TagSelfClosingInner(tag);
          if (attributes == null)
          {
            attributeState = HtmlTokenizer.AttributeState.BeforeName;
            continue;
          }
          goto label_72;
        default:
          continue;
      }
    }
label_4:
    return this.EmitTag(tag);
label_11:
    return this.NewEof();
label_14:
    tag.AddAttribute(this.FlushBuffer(), currentPosition);
    return this.EmitTag(tag);
label_25:
    return this.NewEof();
label_27:
    return this.EmitTag(tag);
label_36:
    return this.NewEof();
label_40:
    this.RaiseErrorOccurred(HtmlParseError.TagClosedWrong);
    return this.EmitTag(tag);
label_44:
    return this.NewEof();
label_51:
    return this.NewEof();
label_53:
    tag.SetAttributeValue(this.FlushBuffer());
    return this.EmitTag(tag);
label_61:
    return this.NewEof();
label_63:
    return this.EmitTag(tag);
label_68:
    return this.NewEof();
label_72:
    return attributes;
  }

  private HtmlToken ScriptData(char c)
  {
    int length1 = this._lastStartTag.Length;
    int length2 = TagNames.Script.Length;
    HtmlTokenizer.ScriptState scriptState = HtmlTokenizer.ScriptState.Normal;
    int startIndex = 0;
    while (true)
    {
      switch (scriptState)
      {
        case HtmlTokenizer.ScriptState.Normal:
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              break;
            case '<':
              this.StringBuffer.Append('<');
              scriptState = HtmlTokenizer.ScriptState.OpenTag;
              continue;
            case char.MaxValue:
              goto label_5;
            default:
              this.StringBuffer.Append(c);
              break;
          }
          c = this.GetNext();
          continue;
        case HtmlTokenizer.ScriptState.OpenTag:
          c = this.GetNext();
          switch (c)
          {
            case '!':
              scriptState = HtmlTokenizer.ScriptState.StartEscape;
              continue;
            case '/':
              scriptState = HtmlTokenizer.ScriptState.EndTag;
              continue;
            default:
              scriptState = HtmlTokenizer.ScriptState.Normal;
              continue;
          }
        case HtmlTokenizer.ScriptState.EndTag:
          c = this.GetNext();
          startIndex = this.StringBuffer.Append('/').Length;
          HtmlTagToken tag = this.NewTagClose();
          while (c.IsLetter())
          {
            this.StringBuffer.Append(c);
            c = this.GetNext();
            bool flag1 = c.IsSpaceCharacter();
            bool flag2 = c == '>';
            bool flag3 = c == '/';
            if (this.StringBuffer.Length - startIndex == length1 && flag1 | flag2 | flag3 && this.StringBuffer.ToString(startIndex, length1).Isi(this._lastStartTag))
            {
              if (startIndex > 2)
              {
                this.Back(3 + length1);
                this.StringBuffer.Remove(startIndex - 2, length1 + 2);
                return this.NewCharacter();
              }
              this.StringBuffer.Clear();
              if (flag1)
              {
                tag.Name = this._lastStartTag;
                return this.ParseAttributes(tag);
              }
              if (flag3)
              {
                tag.Name = this._lastStartTag;
                return this.TagSelfClosing(tag);
              }
              if (flag2)
              {
                tag.Name = this._lastStartTag;
                return this.EmitTag(tag);
              }
            }
          }
          scriptState = HtmlTokenizer.ScriptState.Normal;
          continue;
        case HtmlTokenizer.ScriptState.StartEscape:
          this.StringBuffer.Append('!');
          c = this.GetNext();
          scriptState = c != '-' ? HtmlTokenizer.ScriptState.Normal : HtmlTokenizer.ScriptState.StartEscapeDash;
          continue;
        case HtmlTokenizer.ScriptState.Escaped:
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              c = this.GetNext();
              continue;
            case '-':
              this.StringBuffer.Append('-');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDash;
              continue;
            case '<':
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedOpenTag;
              continue;
            case char.MaxValue:
              goto label_32;
            default:
              scriptState = HtmlTokenizer.ScriptState.Normal;
              continue;
          }
        case HtmlTokenizer.ScriptState.StartEscapeDash:
          c = this.GetNext();
          this.StringBuffer.Append('-');
          if (c == '-')
          {
            this.StringBuffer.Append('-');
            scriptState = HtmlTokenizer.ScriptState.EscapedDashDash;
            continue;
          }
          scriptState = HtmlTokenizer.ScriptState.Normal;
          continue;
        case HtmlTokenizer.ScriptState.EscapedDash:
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              break;
            case '-':
              this.StringBuffer.Append('-');
              scriptState = HtmlTokenizer.ScriptState.EscapedDashDash;
              continue;
            case '<':
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedOpenTag;
              continue;
            case char.MaxValue:
              goto label_38;
            default:
              this.StringBuffer.Append(c);
              break;
          }
          c = this.GetNext();
          scriptState = HtmlTokenizer.ScriptState.Escaped;
          continue;
        case HtmlTokenizer.ScriptState.EscapedDashDash:
          c = this.GetNext();
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.Escaped;
              continue;
            case '-':
              this.StringBuffer.Append('-');
              continue;
            case '<':
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedOpenTag;
              continue;
            case '>':
              this.StringBuffer.Append('>');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.Normal;
              continue;
            case char.MaxValue:
              goto label_46;
            default:
              this.StringBuffer.Append(c);
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.Escaped;
              continue;
          }
        case HtmlTokenizer.ScriptState.EscapedOpenTag:
          if (c == '/')
          {
            c = this.GetNext();
            scriptState = HtmlTokenizer.ScriptState.EscapedEndTag;
            continue;
          }
          if (c.IsLetter())
          {
            startIndex = this.StringBuffer.Append('<').Length;
            this.StringBuffer.Append(c);
            scriptState = HtmlTokenizer.ScriptState.StartDoubleEscape;
            continue;
          }
          this.StringBuffer.Append('<');
          scriptState = HtmlTokenizer.ScriptState.Escaped;
          continue;
        case HtmlTokenizer.ScriptState.EscapedEndTag:
          startIndex = this.StringBuffer.Append('<').Append('/').Length;
          if (c.IsLetter())
          {
            this.StringBuffer.Append(c);
            scriptState = HtmlTokenizer.ScriptState.EscapedNameEndTag;
            continue;
          }
          scriptState = HtmlTokenizer.ScriptState.Escaped;
          continue;
        case HtmlTokenizer.ScriptState.EscapedNameEndTag:
          c = this.GetNext();
          if (this.StringBuffer.Length - startIndex != length2 || c != '/' && c != '>' && !c.IsSpaceCharacter() || !this.StringBuffer.ToString(startIndex, length2).Isi(TagNames.Script))
          {
            if (!c.IsLetter())
            {
              scriptState = HtmlTokenizer.ScriptState.Escaped;
              continue;
            }
            this.StringBuffer.Append(c);
            continue;
          }
          goto label_57;
        case HtmlTokenizer.ScriptState.StartDoubleEscape:
          c = this.GetNext();
          if (this.StringBuffer.Length - startIndex == length2 && (c == '/' || c == '>' || c.IsSpaceCharacter()))
          {
            int num = this.StringBuffer.ToString(startIndex, length2).Isi(TagNames.Script) ? 1 : 0;
            this.StringBuffer.Append(c);
            c = this.GetNext();
            scriptState = num != 0 ? HtmlTokenizer.ScriptState.EscapedDouble : HtmlTokenizer.ScriptState.Escaped;
            continue;
          }
          if (c.IsLetter())
          {
            this.StringBuffer.Append(c);
            continue;
          }
          scriptState = HtmlTokenizer.ScriptState.Escaped;
          continue;
        case HtmlTokenizer.ScriptState.EscapedDouble:
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              break;
            case '-':
              this.StringBuffer.Append('-');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDoubleDash;
              continue;
            case '<':
              this.StringBuffer.Append('<');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDoubleOpenTag;
              continue;
            case char.MaxValue:
              goto label_70;
          }
          this.StringBuffer.Append(c);
          c = this.GetNext();
          continue;
        case HtmlTokenizer.ScriptState.EscapedDoubleDash:
          switch (c)
          {
            case char.MinValue:
              this.RaiseErrorOccurred(HtmlParseError.Null);
              c = '�';
              break;
            case '-':
              this.StringBuffer.Append('-');
              scriptState = HtmlTokenizer.ScriptState.EscapedDoubleDashDash;
              continue;
            case '<':
              this.StringBuffer.Append('<');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDoubleOpenTag;
              continue;
            case char.MaxValue:
              goto label_76;
          }
          scriptState = HtmlTokenizer.ScriptState.EscapedDouble;
          continue;
        case HtmlTokenizer.ScriptState.EscapedDoubleDashDash:
          c = this.GetNext();
          switch (c)
          {
            case char.MinValue:
              this.AppendReplacement();
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDouble;
              continue;
            case '-':
              this.StringBuffer.Append('-');
              continue;
            case '<':
              this.StringBuffer.Append('<');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDoubleOpenTag;
              continue;
            case '>':
              this.StringBuffer.Append('>');
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.Normal;
              continue;
            case char.MaxValue:
              goto label_83;
            default:
              this.StringBuffer.Append(c);
              c = this.GetNext();
              scriptState = HtmlTokenizer.ScriptState.EscapedDouble;
              continue;
          }
        case HtmlTokenizer.ScriptState.EscapedDoubleOpenTag:
          if (c == '/')
          {
            startIndex = this.StringBuffer.Append('/').Length;
            scriptState = HtmlTokenizer.ScriptState.EndDoubleEscape;
            continue;
          }
          scriptState = HtmlTokenizer.ScriptState.EscapedDouble;
          continue;
        case HtmlTokenizer.ScriptState.EndDoubleEscape:
          c = this.GetNext();
          if (this.StringBuffer.Length - startIndex == length2 && (c.IsSpaceCharacter() || c == '/' || c == '>'))
          {
            int num = this.StringBuffer.ToString(startIndex, length2).Isi(TagNames.Script) ? 1 : 0;
            this.StringBuffer.Append(c);
            c = this.GetNext();
            scriptState = num != 0 ? HtmlTokenizer.ScriptState.Escaped : HtmlTokenizer.ScriptState.EscapedDouble;
            continue;
          }
          if (c.IsLetter())
          {
            this.StringBuffer.Append(c);
            continue;
          }
          scriptState = HtmlTokenizer.ScriptState.EscapedDouble;
          continue;
        default:
          continue;
      }
    }
label_5:
    this.Back();
    return this.NewCharacter();
label_32:
    this.Back();
    return this.NewCharacter();
label_38:
    this.Back();
    return this.NewCharacter();
label_46:
    return this.NewCharacter();
label_57:
    this.Back(length2 + 3);
    this.StringBuffer.Remove(startIndex - 2, length2 + 2);
    return this.NewCharacter();
label_70:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    return this.NewCharacter();
label_76:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    return this.NewCharacter();
label_83:
    this.RaiseErrorOccurred(HtmlParseError.EOF);
    this.Back();
    return this.NewCharacter();
  }

  private HtmlToken NewCharacter()
  {
    return new HtmlToken(HtmlTokenType.Character, this._position, this.FlushBuffer());
  }

  private HtmlToken NewProcessingInstruction()
  {
    return new HtmlToken(HtmlTokenType.Comment, this._position, this.FlushBuffer())
    {
      IsProcessingInstruction = true
    };
  }

  private HtmlToken NewComment()
  {
    return new HtmlToken(HtmlTokenType.Comment, this._position, this.FlushBuffer());
  }

  private HtmlToken NewEof(bool acceptable = false)
  {
    if (!acceptable)
      this.RaiseErrorOccurred(HtmlParseError.EOF);
    return new HtmlToken(HtmlTokenType.EndOfFile, this._position);
  }

  private HtmlDoctypeToken NewDoctype(bool quirksForced)
  {
    return new HtmlDoctypeToken(quirksForced, this._position);
  }

  private HtmlTagToken NewTagOpen() => new HtmlTagToken(HtmlTokenType.StartTag, this._position);

  private HtmlTagToken NewTagClose() => new HtmlTagToken(HtmlTokenType.EndTag, this._position);

  private void RaiseErrorOccurred(HtmlParseError code)
  {
    this.RaiseErrorOccurred(code, this.GetCurrentPosition());
  }

  private void AppendReplacement()
  {
    this.RaiseErrorOccurred(HtmlParseError.Null);
    this.StringBuffer.Append('�');
  }

  private HtmlToken CreateIfAppropriate(char c)
  {
    bool flag1 = c.IsSpaceCharacter();
    bool flag2 = c == '>';
    bool flag3 = c == '/';
    if (this.StringBuffer.Length == this._lastStartTag.Length && flag1 | flag2 | flag3 && this.StringBuffer.ToString().Is(this._lastStartTag))
    {
      HtmlTagToken tag = this.NewTagClose();
      this.StringBuffer.Clear();
      if (flag1)
      {
        tag.Name = this._lastStartTag;
        return this.ParseAttributes(tag);
      }
      if (flag3)
      {
        tag.Name = this._lastStartTag;
        return this.TagSelfClosing(tag);
      }
      if (flag2)
      {
        tag.Name = this._lastStartTag;
        return this.EmitTag(tag);
      }
    }
    return (HtmlToken) null;
  }

  private HtmlToken EmitTag(HtmlTagToken tag)
  {
    List<HtmlAttributeToken> attributes = tag.Attributes;
    this.State = HtmlParseMode.PCData;
    switch (tag.Type)
    {
      case HtmlTokenType.StartTag:
        for (int index1 = attributes.Count - 1; index1 > 0; --index1)
        {
          for (int index2 = index1 - 1; index2 >= 0; --index2)
          {
            if (attributes[index2].Name.Is(attributes[index1].Name))
            {
              attributes.RemoveAt(index1);
              this.RaiseErrorOccurred(HtmlParseError.AttributeDuplicateOmitted, tag.Position);
              break;
            }
          }
        }
        this._lastStartTag = tag.Name;
        break;
      case HtmlTokenType.EndTag:
        if (tag.IsSelfClosing)
          this.RaiseErrorOccurred(HtmlParseError.EndTagCannotBeSelfClosed, tag.Position);
        if (attributes.Count != 0)
        {
          this.RaiseErrorOccurred(HtmlParseError.EndTagCannotHaveAttributes, tag.Position);
          break;
        }
        break;
    }
    return (HtmlToken) tag;
  }

  private enum AttributeState : byte
  {
    BeforeName,
    Name,
    AfterName,
    BeforeValue,
    QuotedValue,
    AfterValue,
    UnquotedValue,
    SelfClose,
  }

  private enum ScriptState : byte
  {
    Normal,
    OpenTag,
    EndTag,
    StartEscape,
    Escaped,
    StartEscapeDash,
    EscapedDash,
    EscapedDashDash,
    EscapedOpenTag,
    EscapedEndTag,
    EscapedNameEndTag,
    StartDoubleEscape,
    EscapedDouble,
    EscapedDoubleDash,
    EscapedDoubleDashDash,
    EscapedDoubleOpenTag,
    EndDoubleEscape,
  }
}
