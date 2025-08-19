// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockParse
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using RuriLib.LS;
using RuriLib.Utils.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable
namespace RuriLib;

public class BlockParse : BlockBase
{
  private string parseTarget = "<SOURCE>";
  private string variableName = "";
  private bool isCapture;
  private string prefix = "";
  private string suffix = "";
  private bool recursive;
  private bool encodeOutput;
  private bool createEmpty = true;
  private ParseType type;
  private string leftString = "";
  private string rightString = "";
  private bool useRegexLR;
  private string cssSelector = "";
  private string attributeName = "";
  private int cssElementIndex;
  private string jsonField = "";
  private bool jTokenParsing;
  private string regexString = "";
  private string regexOutput = "";

  public string ParseTarget
  {
    get => this.parseTarget;
    set
    {
      this.parseTarget = value;
      this.OnPropertyChanged(nameof (ParseTarget));
    }
  }

  public string VariableName
  {
    get => this.variableName;
    set
    {
      this.variableName = value;
      this.OnPropertyChanged(nameof (VariableName));
    }
  }

  public bool IsCapture
  {
    get => this.isCapture;
    set
    {
      this.isCapture = value;
      this.OnPropertyChanged(nameof (IsCapture));
    }
  }

  public string Prefix
  {
    get => this.prefix;
    set
    {
      this.prefix = value;
      this.OnPropertyChanged(nameof (Prefix));
    }
  }

  public string Suffix
  {
    get => this.suffix;
    set
    {
      this.suffix = value;
      this.OnPropertyChanged(nameof (Suffix));
    }
  }

  public bool Recursive
  {
    get => this.recursive;
    set
    {
      this.recursive = value;
      this.OnPropertyChanged(nameof (Recursive));
    }
  }

  public bool EncodeOutput
  {
    get => this.encodeOutput;
    set
    {
      this.encodeOutput = value;
      this.OnPropertyChanged(nameof (EncodeOutput));
    }
  }

  public bool CreateEmpty
  {
    get => this.createEmpty;
    set
    {
      this.createEmpty = value;
      this.OnPropertyChanged(nameof (CreateEmpty));
    }
  }

  public ParseType Type
  {
    get => this.type;
    set
    {
      this.type = value;
      this.OnPropertyChanged(nameof (Type));
    }
  }

  public string LeftString
  {
    get => this.leftString;
    set
    {
      this.leftString = value;
      this.OnPropertyChanged(nameof (LeftString));
    }
  }

  public string RightString
  {
    get => this.rightString;
    set
    {
      this.rightString = value;
      this.OnPropertyChanged(nameof (RightString));
    }
  }

  public bool UseRegexLR
  {
    get => this.useRegexLR;
    set
    {
      this.useRegexLR = value;
      this.OnPropertyChanged(nameof (UseRegexLR));
    }
  }

  public string CssSelector
  {
    get => this.cssSelector;
    set
    {
      this.cssSelector = value;
      this.OnPropertyChanged(nameof (CssSelector));
    }
  }

  public string AttributeName
  {
    get => this.attributeName;
    set
    {
      this.attributeName = value;
      this.OnPropertyChanged(nameof (AttributeName));
    }
  }

  public int CssElementIndex
  {
    get => this.cssElementIndex;
    set
    {
      this.cssElementIndex = value;
      this.OnPropertyChanged(nameof (CssElementIndex));
    }
  }

  public string JsonField
  {
    get => this.jsonField;
    set
    {
      this.jsonField = value;
      this.OnPropertyChanged(nameof (JsonField));
    }
  }

  public bool JTokenParsing
  {
    get => this.jTokenParsing;
    set
    {
      this.jTokenParsing = value;
      this.OnPropertyChanged(nameof (JTokenParsing));
    }
  }

  public string RegexString
  {
    get => this.regexString;
    set
    {
      this.regexString = value;
      this.OnPropertyChanged(nameof (RegexString));
    }
  }

  public string RegexOutput
  {
    get => this.regexOutput;
    set
    {
      this.regexOutput = value;
      this.OnPropertyChanged(nameof (RegexOutput));
    }
  }

  public BlockParse() => this.Label = "PARSE";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    this.ParseTarget = LineParser.ParseLiteral(ref input, "TARGET");
    // ISSUE: reference to a compiler-generated field
    if (BlockParse.\u003C\u003Eo__77.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockParse.\u003C\u003Eo__77.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, ParseType>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (ParseType), typeof (BlockParse)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.Type = BlockParse.\u003C\u003Eo__77.\u003C\u003Ep__0.Target((CallSite) BlockParse.\u003C\u003Eo__77.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "TYPE", typeof (ParseType)));
    switch (this.Type)
    {
      case ParseType.LR:
        this.LeftString = LineParser.ParseLiteral(ref input, "LEFT STRING");
        this.RightString = LineParser.ParseLiteral(ref input, "RIGHT STRING");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        break;
      case ParseType.CSS:
        this.CssSelector = LineParser.ParseLiteral(ref input, "SELECTOR");
        this.AttributeName = LineParser.ParseLiteral(ref input, "ATTRIBUTE");
        if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        else if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Integer)
          this.CssElementIndex = LineParser.ParseInt(ref input, "INDEX");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        break;
      case ParseType.JSON:
        this.JsonField = LineParser.ParseLiteral(ref input, "FIELD");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        break;
      case ParseType.REGEX:
        this.RegexString = LineParser.ParseLiteral(ref input, "PATTERN");
        this.RegexOutput = LineParser.ParseLiteral(ref input, "OUTPUT");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        break;
    }
    LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Arrow, true);
    try
    {
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (!(token.ToUpper() == "VAR"))
      {
        if (!(token.ToUpper() == "CAP"))
          goto label_25;
      }
      this.IsCapture = token.ToUpper() == "CAP";
    }
    catch
    {
      throw new ArgumentException("Invalid or missing variable type");
    }
label_25:
    try
    {
      this.VariableName = LineParser.ParseLiteral(ref input, "NAME");
    }
    catch
    {
      throw new ArgumentException("Variable name not specified");
    }
    try
    {
      this.Prefix = LineParser.ParseLiteral(ref input, "PREFIX");
      this.Suffix = LineParser.ParseLiteral(ref input, "SUFFIX");
    }
    catch
    {
    }
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "PARSE").Literal(this.ParseTarget).Token((object) this.Type);
    switch (this.Type)
    {
      case ParseType.LR:
        blockWriter.Literal(this.LeftString).Literal(this.RightString).Boolean(this.Recursive, "Recursive").Boolean(this.EncodeOutput, "EncodeOutput").Boolean(this.CreateEmpty, "CreateEmpty").Boolean(this.UseRegexLR, "UseRegexLR");
        break;
      case ParseType.CSS:
        blockWriter.Literal(this.CssSelector).Literal(this.AttributeName);
        if (this.Recursive)
          blockWriter.Boolean(this.Recursive, "Recursive");
        else
          blockWriter.Integer(this.CssElementIndex, "CssElementIndex");
        blockWriter.Boolean(this.EncodeOutput, "EncodeOutput").Boolean(this.CreateEmpty, "CreateEmpty");
        break;
      case ParseType.JSON:
        blockWriter.Literal(this.JsonField).Boolean(this.JTokenParsing, "JTokenParsing").Boolean(this.Recursive, "Recursive").Boolean(this.EncodeOutput, "EncodeOutput").Boolean(this.CreateEmpty, "CreateEmpty");
        break;
      case ParseType.REGEX:
        blockWriter.Literal(this.RegexString).Literal(this.RegexOutput).Boolean(this.Recursive, "Recursive").Boolean(this.EncodeOutput, "EncodeOutput").Boolean(this.CreateEmpty, "CreateEmpty");
        break;
    }
    blockWriter.Arrow().Token(this.IsCapture ? (object) "CAP" : (object) "VAR").Literal(this.VariableName);
    if (!blockWriter.CheckDefault((object) this.Prefix, "Prefix") || !blockWriter.CheckDefault((object) this.Suffix, "Suffix"))
      blockWriter.Literal(this.Prefix).Literal(this.Suffix);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    string input = BlockBase.ReplaceValues(this.parseTarget, data);
    List<string> values = new List<string>();
    switch (this.Type)
    {
      case ParseType.LR:
        values = Parse.LR(input, BlockBase.ReplaceValues(this.leftString, data), BlockBase.ReplaceValues(this.rightString, data), this.recursive, this.useRegexLR).ToList<string>();
        break;
      case ParseType.CSS:
        values = Parse.CSS(input, BlockBase.ReplaceValues(this.cssSelector, data), BlockBase.ReplaceValues(this.attributeName, data), this.cssElementIndex, this.recursive).ToList<string>();
        break;
      case ParseType.JSON:
        values = Parse.JSON(input, BlockBase.ReplaceValues(this.jsonField, data), this.recursive, this.jTokenParsing).ToList<string>();
        break;
      case ParseType.XPATH:
        throw new NotImplementedException("XPATH parsing is not implemented yet");
      case ParseType.REGEX:
        values = Parse.REGEX(input, BlockBase.ReplaceValues(this.regexString, data), BlockBase.ReplaceValues(this.regexOutput, data), this.recursive).ToList<string>();
        break;
    }
    BlockBase.InsertVariables(data, this.isCapture, this.recursive, values, this.variableName, this.prefix, this.suffix, this.encodeOutput, this.createEmpty);
  }
}
