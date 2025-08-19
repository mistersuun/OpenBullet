// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockFunction
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.IdentityModel.Tokens;
using RuriLib.Functions.Crypto;
using RuriLib.Functions.Formats;
using RuriLib.LS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public class BlockFunction : BlockBase
{
  private string variableName = "";
  private bool isCapture;
  private string inputString = "";
  private BlockFunction.Function functionType;
  private Hash hashType = Hash.SHA512;
  private string hmacKey = "";
  private bool hmacBase64;
  private bool stopAfterFirstMatch = true;
  private string dateFormat = "yyyy-MM-dd:HH-mm-ss";
  private string replaceWhat = "";
  private string replaceWith = "";
  private bool useRegex;
  private string regexMatch = "";
  private int randomMin;
  private int randomMax;
  private string stringToFind = "";
  private string rsaKey = "";
  private string rsaMod = "";
  private string rsaExp = "";
  private bool rsaOAEP = true;
  private string charIndex = "0";
  private string substringIndex = "0";
  private string substringLength = "1";
  private string aesKey = "";
  private string aesIV = "";
  private CipherMode aesMode = CipherMode.CBC;
  private PaddingMode aesPadding = PaddingMode.None;
  private string kdfSalt = "";
  private int kdfSaltSize = 8;
  private int kdfIterations = 1;
  private int kdfKeySize = 16 /*0x10*/;
  private Hash kdfAlgorithm = Hash.SHA1;

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

  public string InputString
  {
    get => this.inputString;
    set
    {
      this.inputString = value;
      this.OnPropertyChanged(nameof (InputString));
    }
  }

  public BlockFunction.Function FunctionType
  {
    get => this.functionType;
    set
    {
      this.functionType = value;
      this.OnPropertyChanged(nameof (FunctionType));
    }
  }

  public Hash HashType
  {
    get => this.hashType;
    set
    {
      this.hashType = value;
      this.OnPropertyChanged(nameof (HashType));
    }
  }

  public string HmacKey
  {
    get => this.hmacKey;
    set
    {
      this.hmacKey = value;
      this.OnPropertyChanged(nameof (HmacKey));
    }
  }

  public bool HmacBase64
  {
    get => this.hmacBase64;
    set
    {
      this.hmacBase64 = value;
      this.OnPropertyChanged(nameof (HmacBase64));
    }
  }

  public bool StopAfterFirstMatch
  {
    get => this.stopAfterFirstMatch;
    set
    {
      this.stopAfterFirstMatch = value;
      this.OnPropertyChanged(nameof (StopAfterFirstMatch));
    }
  }

  public Dictionary<string, string> TranslationDictionary { get; set; } = new Dictionary<string, string>();

  public string DateFormat
  {
    get => this.dateFormat;
    set
    {
      this.dateFormat = value;
      this.OnPropertyChanged(nameof (DateFormat));
    }
  }

  public string ReplaceWhat
  {
    get => this.replaceWhat;
    set
    {
      this.replaceWhat = value;
      this.OnPropertyChanged(nameof (ReplaceWhat));
    }
  }

  public string ReplaceWith
  {
    get => this.replaceWith;
    set
    {
      this.replaceWith = value;
      this.OnPropertyChanged(nameof (ReplaceWith));
    }
  }

  public bool UseRegex
  {
    get => this.useRegex;
    set
    {
      this.useRegex = value;
      this.OnPropertyChanged(nameof (UseRegex));
    }
  }

  public string RegexMatch
  {
    get => this.regexMatch;
    set
    {
      this.regexMatch = value;
      this.OnPropertyChanged(nameof (RegexMatch));
    }
  }

  public int RandomMin
  {
    get => this.randomMin;
    set
    {
      this.randomMin = value;
      this.OnPropertyChanged(nameof (RandomMin));
    }
  }

  public int RandomMax
  {
    get => this.randomMax;
    set
    {
      this.randomMax = value;
      this.OnPropertyChanged(nameof (RandomMax));
    }
  }

  public string StringToFind
  {
    get => this.stringToFind;
    set
    {
      this.stringToFind = value;
      this.OnPropertyChanged(nameof (StringToFind));
    }
  }

  public string RsaKey
  {
    get => this.rsaKey;
    set
    {
      this.rsaKey = value;
      this.OnPropertyChanged(nameof (RsaKey));
    }
  }

  public string RsaMod
  {
    get => this.rsaMod;
    set
    {
      this.rsaMod = value;
      this.OnPropertyChanged(nameof (RsaMod));
    }
  }

  public string RsaExp
  {
    get => this.rsaExp;
    set
    {
      this.rsaExp = value;
      this.OnPropertyChanged(nameof (RsaExp));
    }
  }

  public bool RsaOAEP
  {
    get => this.rsaOAEP;
    set
    {
      this.rsaOAEP = value;
      this.OnPropertyChanged(nameof (RsaOAEP));
    }
  }

  public string CharIndex
  {
    get => this.charIndex;
    set
    {
      this.charIndex = value;
      this.OnPropertyChanged(nameof (CharIndex));
    }
  }

  public string SubstringIndex
  {
    get => this.substringIndex;
    set
    {
      this.substringIndex = value;
      this.OnPropertyChanged(nameof (SubstringIndex));
    }
  }

  public string SubstringLength
  {
    get => this.substringLength;
    set
    {
      this.substringLength = value;
      this.OnPropertyChanged(nameof (SubstringLength));
    }
  }

  public string AesKey
  {
    get => this.aesKey;
    set
    {
      this.aesKey = value;
      this.OnPropertyChanged(nameof (AesKey));
    }
  }

  public string AesIV
  {
    get => this.aesIV;
    set
    {
      this.aesIV = value;
      this.OnPropertyChanged(nameof (AesIV));
    }
  }

  public CipherMode AesMode
  {
    get => this.aesMode;
    set
    {
      this.aesMode = value;
      this.OnPropertyChanged(nameof (AesMode));
    }
  }

  public PaddingMode AesPadding
  {
    get => this.aesPadding;
    set
    {
      this.aesPadding = value;
      this.OnPropertyChanged(nameof (AesPadding));
    }
  }

  public string KdfSalt
  {
    get => this.kdfSalt;
    set
    {
      this.kdfSalt = value;
      this.OnPropertyChanged(nameof (KdfSalt));
    }
  }

  public int KdfSaltSize
  {
    get => this.kdfSaltSize;
    set
    {
      this.kdfSaltSize = value;
      this.OnPropertyChanged(nameof (KdfSaltSize));
    }
  }

  public int KdfIterations
  {
    get => this.kdfIterations;
    set
    {
      this.kdfIterations = value;
      this.OnPropertyChanged(nameof (KdfIterations));
    }
  }

  public int KdfKeySize
  {
    get => this.kdfKeySize;
    set
    {
      this.kdfKeySize = value;
      this.OnPropertyChanged(nameof (KdfKeySize));
    }
  }

  public Hash KdfAlgorithm
  {
    get => this.kdfAlgorithm;
    set
    {
      this.kdfAlgorithm = value;
      this.OnPropertyChanged(nameof (KdfAlgorithm));
    }
  }

  public BlockFunction() => this.Label = "FUNCTION";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    // ISSUE: reference to a compiler-generated field
    if (BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__0 == null)
    {
      // ISSUE: reference to a compiler-generated field
      BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, BlockFunction.Function>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (BlockFunction.Function), typeof (BlockFunction)));
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this.FunctionType = BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__0.Target((CallSite) BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "Function Name", typeof (BlockFunction.Function)));
    switch (this.FunctionType)
    {
      case BlockFunction.Function.Hash:
        // ISSUE: reference to a compiler-generated field
        if (BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Hash>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Hash), typeof (BlockFunction)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.HashType = BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__1.Target((CallSite) BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__1, LineParser.ParseEnum(ref input, "Hash Type", typeof (Hash)));
        break;
      case BlockFunction.Function.HMAC:
        // ISSUE: reference to a compiler-generated field
        if (BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, Hash>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Hash), typeof (BlockFunction)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.HashType = BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__2.Target((CallSite) BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__2, LineParser.ParseEnum(ref input, "Hash Type", typeof (Hash)));
        this.HmacKey = LineParser.ParseLiteral(ref input, "HMAC Key");
        while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        break;
      case BlockFunction.Function.Translate:
        if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
          LineParser.SetBool(ref input, (object) this);
        this.TranslationDictionary = new Dictionary<string, string>();
        while (input != "" && LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Parameter)
        {
          LineParser.EnsureIdentifier(ref input, "KEY");
          string literal1 = LineParser.ParseLiteral(ref input, "Key");
          LineParser.EnsureIdentifier(ref input, "VALUE");
          string literal2 = LineParser.ParseLiteral(ref input, "Value");
          this.TranslationDictionary[literal1] = literal2;
        }
        break;
      case BlockFunction.Function.DateToUnixTime:
        this.DateFormat = LineParser.ParseLiteral(ref input, "DATE FORMAT");
        break;
      case BlockFunction.Function.Replace:
        this.ReplaceWhat = LineParser.ParseLiteral(ref input, "What");
        this.ReplaceWith = LineParser.ParseLiteral(ref input, "With");
        if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
        {
          LineParser.SetBool(ref input, (object) this);
          break;
        }
        break;
      case BlockFunction.Function.RegexMatch:
        this.RegexMatch = LineParser.ParseLiteral(ref input, "Pattern");
        break;
      case BlockFunction.Function.RandomNum:
        this.RandomMin = LineParser.ParseInt(ref input, "Minimum");
        this.RandomMax = LineParser.ParseInt(ref input, "Maximum");
        break;
      case BlockFunction.Function.CountOccurrences:
        this.StringToFind = LineParser.ParseLiteral(ref input, "string to find");
        break;
      case BlockFunction.Function.RSAEncrypt:
      case BlockFunction.Function.RSADecrypt:
        this.RsaKey = LineParser.ParseLiteral(ref input, "Private Key");
        this.RsaMod = LineParser.ParseLiteral(ref input, "Public Key Modulus");
        this.RsaExp = LineParser.ParseLiteral(ref input, "Public Key Exponent");
        if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
        {
          LineParser.SetBool(ref input, (object) this);
          break;
        }
        break;
      case BlockFunction.Function.CharAt:
        this.CharIndex = LineParser.ParseLiteral(ref input, "Index");
        break;
      case BlockFunction.Function.Substring:
        this.SubstringIndex = LineParser.ParseLiteral(ref input, "Index");
        this.SubstringLength = LineParser.ParseLiteral(ref input, "Length");
        break;
      case BlockFunction.Function.AESEncrypt:
      case BlockFunction.Function.AESDecrypt:
        this.AesKey = LineParser.ParseLiteral(ref input, "Key");
        this.AesIV = LineParser.ParseLiteral(ref input, "IV");
        // ISSUE: reference to a compiler-generated field
        if (BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, CipherMode>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (CipherMode), typeof (BlockFunction)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.AesMode = BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__3.Target((CallSite) BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__3, LineParser.ParseEnum(ref input, "Cipher mode", typeof (CipherMode)));
        // ISSUE: reference to a compiler-generated field
        if (BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, PaddingMode>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (PaddingMode), typeof (BlockFunction)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.AesPadding = BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__4.Target((CallSite) BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__4, LineParser.ParseEnum(ref input, "Padding mode", typeof (PaddingMode)));
        break;
      case BlockFunction.Function.PBKDF2PKCS5:
        if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Literal)
          this.KdfSalt = LineParser.ParseLiteral(ref input, "Salt");
        else
          this.KdfSaltSize = LineParser.ParseInt(ref input, "Salt size");
        this.KdfIterations = LineParser.ParseInt(ref input, "Iterations");
        this.KdfKeySize = LineParser.ParseInt(ref input, "Key size");
        // ISSUE: reference to a compiler-generated field
        if (BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, Hash>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Hash), typeof (BlockFunction)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.KdfAlgorithm = BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__5.Target((CallSite) BlockFunction.\u003C\u003Eo__134.\u003C\u003Ep__5, LineParser.ParseEnum(ref input, "Algorithm", typeof (Hash)));
        break;
    }
    if (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Literal)
      this.InputString = LineParser.ParseLiteral(ref input, "INPUT");
    if (LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Arrow, false) == "")
      return (BlockBase) this;
    try
    {
      string token = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Parameter, true);
      if (!(token.ToUpper() == "VAR"))
      {
        if (!(token.ToUpper() == "CAP"))
          goto label_47;
      }
      this.IsCapture = token.ToUpper() == "CAP";
    }
    catch
    {
      throw new ArgumentException("Invalid or missing variable type");
    }
label_47:
    try
    {
      this.VariableName = LineParser.ParseToken(ref input, RuriLib.LS.TokenType.Literal, true);
    }
    catch
    {
      throw new ArgumentException("Variable name not specified");
    }
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "FUNCTION").Token((object) this.FunctionType);
    switch (this.FunctionType)
    {
      case BlockFunction.Function.Hash:
        blockWriter.Token((object) this.HashType);
        break;
      case BlockFunction.Function.HMAC:
        blockWriter.Token((object) this.HashType).Literal(this.HmacKey).Boolean(this.HmacBase64, "HmacBase64");
        break;
      case BlockFunction.Function.Translate:
        blockWriter.Boolean(this.StopAfterFirstMatch, "StopAfterFirstMatch");
        foreach (KeyValuePair<string, string> translation in this.TranslationDictionary)
          blockWriter.Indent().Token((object) "KEY").Literal(translation.Key).Token((object) "VALUE").Literal(translation.Value);
        blockWriter.Indent();
        break;
      case BlockFunction.Function.DateToUnixTime:
        blockWriter.Literal(this.DateFormat, "DateFormat");
        break;
      case BlockFunction.Function.Replace:
        blockWriter.Literal(this.ReplaceWhat).Literal(this.ReplaceWith).Boolean(this.UseRegex, "UseRegex");
        break;
      case BlockFunction.Function.RegexMatch:
        blockWriter.Literal(this.RegexMatch, "RegexMatch");
        break;
      case BlockFunction.Function.RandomNum:
        blockWriter.Integer(this.RandomMin).Integer(this.RandomMax);
        break;
      case BlockFunction.Function.CountOccurrences:
        blockWriter.Literal(this.StringToFind);
        break;
      case BlockFunction.Function.RSAEncrypt:
      case BlockFunction.Function.RSADecrypt:
        blockWriter.Literal(this.RsaKey).Literal(this.RsaMod).Literal(this.RsaExp).Boolean(this.RsaOAEP, "RsaOAEP");
        break;
      case BlockFunction.Function.CharAt:
        blockWriter.Literal(this.CharIndex);
        break;
      case BlockFunction.Function.Substring:
        blockWriter.Literal(this.SubstringIndex).Literal(this.SubstringLength);
        break;
      case BlockFunction.Function.AESEncrypt:
      case BlockFunction.Function.AESDecrypt:
        blockWriter.Literal(this.AesKey).Literal(this.AesIV).Token((object) this.AesMode).Token((object) this.AesPadding);
        break;
      case BlockFunction.Function.PBKDF2PKCS5:
        if (this.KdfSalt != "")
          blockWriter.Literal(this.KdfSalt);
        else
          blockWriter.Integer(this.KdfSaltSize);
        blockWriter.Integer(this.KdfIterations).Integer(this.KdfKeySize).Token((object) this.KdfAlgorithm);
        break;
    }
    blockWriter.Literal(this.InputString, "InputString");
    if (!blockWriter.CheckDefault((object) this.VariableName, "VariableName"))
      blockWriter.Arrow().Token(this.IsCapture ? (object) "CAP" : (object) "VAR").Literal(this.VariableName);
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
    CultureInfo provider = new CultureInfo("en-US");
    List<string> stringList = BlockBase.ReplaceValuesRecursive(this.inputString, data);
    List<string> values = new List<string>();
    for (int index = 0; index < stringList.Count; ++index)
    {
      string lower = stringList[index];
      string outputString = "";
      long unixTimeSeconds;
      int num1;
      Decimal num2;
      switch (this.FunctionType)
      {
        case BlockFunction.Function.Constant:
          outputString = lower;
          break;
        case BlockFunction.Function.Base64Encode:
          outputString = lower.ToBase64();
          break;
        case BlockFunction.Function.Base64Decode:
          outputString = lower.FromBase64();
          break;
        case BlockFunction.Function.Hash:
          outputString = BlockFunction.GetHash(lower, this.hashType).ToLower();
          break;
        case BlockFunction.Function.HMAC:
          outputString = BlockFunction.Hmac(lower, this.hashType, BlockBase.ReplaceValues(this.hmacKey, data), this.hmacBase64);
          break;
        case BlockFunction.Function.Translate:
          outputString = lower;
          using (IEnumerator<KeyValuePair<string, string>> enumerator = this.TranslationDictionary.OrderBy<KeyValuePair<string, string>, int>((System.Func<KeyValuePair<string, string>, int>) (e => e.Key.Length)).Reverse<KeyValuePair<string, string>>().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<string, string> current = enumerator.Current;
              if (outputString.Contains(current.Key))
              {
                outputString = outputString.Replace(current.Key, current.Value);
                if (this.StopAfterFirstMatch)
                  break;
              }
            }
            break;
          }
        case BlockFunction.Function.DateToUnixTime:
          unixTimeSeconds = lower.ToDateTime(this.dateFormat).ToUnixTimeSeconds();
          outputString = unixTimeSeconds.ToString();
          break;
        case BlockFunction.Function.Length:
          num1 = lower.Length;
          outputString = num1.ToString();
          break;
        case BlockFunction.Function.ToLowercase:
          outputString = lower.ToLower();
          break;
        case BlockFunction.Function.ToUppercase:
          outputString = lower.ToUpper();
          break;
        case BlockFunction.Function.Replace:
          outputString = !this.useRegex ? lower.Replace(BlockBase.ReplaceValues(this.replaceWhat, data), BlockBase.ReplaceValues(this.replaceWith, data)) : Regex.Replace(lower, BlockBase.ReplaceValues(this.replaceWhat, data), BlockBase.ReplaceValues(this.replaceWith, data));
          break;
        case BlockFunction.Function.RegexMatch:
          outputString = Regex.Match(lower, BlockBase.ReplaceValues(this.regexMatch, data)).Value;
          break;
        case BlockFunction.Function.URLEncode:
          outputString = string.Join("", ((IEnumerable<string>) BlockFunction.SplitInChunks(lower, 2080)).Select<string, string>((System.Func<string, string>) (s => Uri.EscapeDataString(s))));
          break;
        case BlockFunction.Function.URLDecode:
          outputString = Uri.UnescapeDataString(lower);
          break;
        case BlockFunction.Function.Unescape:
          outputString = Regex.Unescape(lower);
          break;
        case BlockFunction.Function.HTMLEntityEncode:
          outputString = WebUtility.HtmlEncode(lower);
          break;
        case BlockFunction.Function.HTMLEntityDecode:
          outputString = WebUtility.HtmlDecode(lower);
          break;
        case BlockFunction.Function.UnixTimeToDate:
          outputString = double.Parse(lower).ToDateTime().ToShortDateString();
          break;
        case BlockFunction.Function.CurrentUnixTime:
          unixTimeSeconds = DateTime.UtcNow.ToUnixTimeSeconds();
          outputString = unixTimeSeconds.ToString();
          break;
        case BlockFunction.Function.UnixTimeToISO8601:
          outputString = double.Parse(lower).ToDateTime().ToISO8601();
          break;
        case BlockFunction.Function.RandomNum:
          num1 = data.Random.Next(this.randomMin, this.randomMax);
          outputString = num1.ToString();
          break;
        case BlockFunction.Function.RandomString:
          string[] source = new string[8]
          {
            "?l",
            "?u",
            "?d",
            "?s",
            "?h",
            "?a",
            "?m",
            "?i"
          };
          string str1 = "abcdefghijklmnopqrstuvwxyz";
          string str2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
          string str3 = "0123456789";
          string str4 = "\\!\"£$%&/()=?^'{}[]@#,;.:-_*+";
          string str5 = str3 + "abcdef";
          string str6 = str1 + str2 + str3 + str4;
          string str7 = str2 + str3;
          string str8 = str1 + str2 + str3;
          outputString = lower;
          char ch;
          while (((IEnumerable<string>) source).Any<string>((System.Func<string, bool>) (r => outputString.Contains(r))))
          {
            if (outputString.Contains("?l"))
            {
              string text = outputString;
              ch = str1[data.Random.Next(0, str1.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?l", replace);
            }
            else if (outputString.Contains("?u"))
            {
              string text = outputString;
              ch = str2[data.Random.Next(0, str2.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?u", replace);
            }
            else if (outputString.Contains("?d"))
            {
              string text = outputString;
              ch = str3[data.Random.Next(0, str3.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?d", replace);
            }
            else if (outputString.Contains("?s"))
            {
              string text = outputString;
              ch = str4[data.Random.Next(0, str4.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?s", replace);
            }
            else if (outputString.Contains("?h"))
            {
              string text = outputString;
              ch = str5[data.Random.Next(0, str5.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?h", replace);
            }
            else if (outputString.Contains("?a"))
            {
              string text = outputString;
              ch = str6[data.Random.Next(0, str6.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?a", replace);
            }
            else if (outputString.Contains("?m"))
            {
              string text = outputString;
              ch = str7[data.Random.Next(0, str7.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?m", replace);
            }
            else if (outputString.Contains("?i"))
            {
              string text = outputString;
              ch = str8[data.Random.Next(0, str8.Length)];
              string replace = ch.ToString();
              outputString = this.ReplaceFirst(text, "?i", replace);
            }
          }
          break;
        case BlockFunction.Function.Ceil:
          num2 = Math.Ceiling(Decimal.Parse(lower, style, (IFormatProvider) provider));
          outputString = num2.ToString();
          break;
        case BlockFunction.Function.Floor:
          num2 = Math.Floor(Decimal.Parse(lower, style, (IFormatProvider) provider));
          outputString = num2.ToString();
          break;
        case BlockFunction.Function.Round:
          num2 = Math.Round(Decimal.Parse(lower, style, (IFormatProvider) provider), 0, MidpointRounding.AwayFromZero);
          outputString = num2.ToString();
          break;
        case BlockFunction.Function.Compute:
          outputString = new DataTable().Compute(lower.Replace(',', '.'), (string) null).ToString();
          break;
        case BlockFunction.Function.CountOccurrences:
          num1 = BlockFunction.CountStringOccurrences(lower, this.stringToFind);
          outputString = num1.ToString();
          break;
        case BlockFunction.Function.ClearCookies:
          data.Cookies.Clear();
          break;
        case BlockFunction.Function.RSAEncrypt:
          outputString = RuriLib.Functions.Crypto.Crypto.RSAEncrypt(lower, BlockBase.ReplaceValues(this.RsaKey, data), BlockBase.ReplaceValues(this.RsaMod, data), BlockBase.ReplaceValues(this.RsaExp, data), this.RsaOAEP);
          break;
        case BlockFunction.Function.RSADecrypt:
          outputString = RuriLib.Functions.Crypto.Crypto.RSADecrypt(lower, BlockBase.ReplaceValues(this.RsaKey, data), BlockBase.ReplaceValues(this.RsaMod, data), BlockBase.ReplaceValues(this.RsaExp, data), this.RsaOAEP);
          break;
        case BlockFunction.Function.Delay:
          try
          {
            Thread.Sleep(int.Parse(lower));
            break;
          }
          catch
          {
            break;
          }
        case BlockFunction.Function.CharAt:
          outputString = lower.ToCharArray()[int.Parse(BlockBase.ReplaceValues(this.charIndex, data))].ToString();
          break;
        case BlockFunction.Function.Substring:
          outputString = lower.Substring(int.Parse(BlockBase.ReplaceValues(this.substringIndex, data)), int.Parse(BlockBase.ReplaceValues(this.substringLength, data)));
          break;
        case BlockFunction.Function.ReverseString:
          char[] charArray = lower.ToCharArray();
          Array.Reverse((Array) charArray);
          outputString = new string(charArray);
          break;
        case BlockFunction.Function.Trim:
          outputString = lower.Trim();
          break;
        case BlockFunction.Function.GetRandomUA:
          outputString = BlockFunction.RandomUserAgent(data.Random);
          break;
        case BlockFunction.Function.AESEncrypt:
          outputString = RuriLib.Functions.Crypto.Crypto.AESEncrypt(lower, BlockBase.ReplaceValues(this.aesKey, data), BlockBase.ReplaceValues(this.aesIV, data), this.AesMode, this.AesPadding);
          break;
        case BlockFunction.Function.AESDecrypt:
          outputString = RuriLib.Functions.Crypto.Crypto.AESDecrypt(lower, BlockBase.ReplaceValues(this.aesKey, data), BlockBase.ReplaceValues(this.aesIV, data), this.AesMode, this.AesPadding);
          break;
        case BlockFunction.Function.PBKDF2PKCS5:
          outputString = RuriLib.Functions.Crypto.Crypto.PBKDF2PKCS5(lower, BlockBase.ReplaceValues(this.KdfSalt, data), this.KdfSaltSize, this.KdfIterations, this.KdfKeySize, this.KdfAlgorithm);
          break;
        case BlockFunction.Function.GenerateOAuthVerifier:
          byte[] data1 = new byte[32 /*0x20*/];
          RandomNumberGenerator.Create().GetBytes(data1);
          outputString = Base64UrlEncoder.Encode(BitConverter.ToString(data1));
          break;
        case BlockFunction.Function.GenerateOAuthChallenge:
          outputString = Base64UrlEncoder.Encode(lower = BlockFunction.GetHash(lower, Hash.SHA256).ToLower());
          break;
        case BlockFunction.Function.GenerateGUID:
          outputString = Guid.NewGuid().ToString();
          break;
        case BlockFunction.Function.GenerateBytes:
          try
          {
            byte[] data2 = new byte[Convert.ToInt32(lower)];
            RandomNumberGenerator.Create().GetBytes(data2);
            outputString = BitConverter.ToString(data2).ToString();
            break;
          }
          catch (FormatException ex)
          {
            data.Status = BotStatus.ERROR;
            data.LogBuffer.Add(new LogEntry("ERROR: " + ex.Message, Colors.Tomato));
            outputString = "INTEGERS ONLY";
            break;
          }
          catch (OverflowException ex)
          {
            data.Status = BotStatus.ERROR;
            data.LogBuffer.Add(new LogEntry("ERROR: " + ex.Message, Colors.Tomato));
            outputString = "BYTE SIZE TOO LARGE FOR 32BIT INTEGER";
            break;
          }
      }
      data.Log(new LogEntry($"Executed function {this.functionType} on input {lower} with outcome {outputString}", Colors.GreenYellow));
      values.Add(outputString);
    }
    bool recursive = values.Count > 1 || this.InputString.Contains("[*]") || this.InputString.Contains("(*)") || this.InputString.Contains("{*}");
    BlockBase.InsertVariables(data, this.isCapture, recursive, values, this.variableName, "", "", false, true);
  }

  public static string GetHash(string baseString, Hash type)
  {
    switch (type)
    {
      case Hash.MD4:
        return RuriLib.Functions.Crypto.Crypto.MD4(baseString);
      case Hash.MD5:
        return RuriLib.Functions.Crypto.Crypto.MD5(baseString);
      case Hash.SHA1:
        return RuriLib.Functions.Crypto.Crypto.SHA1(baseString);
      case Hash.SHA256:
        return RuriLib.Functions.Crypto.Crypto.SHA256(baseString);
      case Hash.SHA384:
        return RuriLib.Functions.Crypto.Crypto.SHA384(baseString);
      case Hash.SHA512:
        return RuriLib.Functions.Crypto.Crypto.SHA512(baseString);
      default:
        throw new NotSupportedException("Unsupported algorithm");
    }
  }

  public static string Hmac(string baseString, Hash type, string key, bool base64)
  {
    switch (type)
    {
      case Hash.MD5:
        return RuriLib.Functions.Crypto.Crypto.HMACMD5(baseString, key, base64);
      case Hash.SHA1:
        return RuriLib.Functions.Crypto.Crypto.HMACSHA1(baseString, key, base64);
      case Hash.SHA256:
        return RuriLib.Functions.Crypto.Crypto.HMACSHA256(baseString, key, base64);
      case Hash.SHA384:
        return RuriLib.Functions.Crypto.Crypto.HMACSHA384(baseString, key, base64);
      case Hash.SHA512:
        return RuriLib.Functions.Crypto.Crypto.HMACSHA512(baseString, key, base64);
      default:
        throw new NotSupportedException("Unsupported algorithm");
    }
  }

  public string GetDictionary()
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<string, string> translation in this.TranslationDictionary)
    {
      stringBuilder.Append($"{translation.Key}: {translation.Value}");
      if (!translation.Equals((object) this.TranslationDictionary.Last<KeyValuePair<string, string>>()))
        stringBuilder.Append(Environment.NewLine);
    }
    return stringBuilder.ToString();
  }

  public void SetDictionary(string[] lines)
  {
    this.TranslationDictionary.Clear();
    foreach (string line in lines)
    {
      if (line.Contains<char>(':'))
      {
        string[] strArray = line.Split(new char[1]{ ':' }, 2);
        this.TranslationDictionary[strArray[0]] = strArray[1].TrimStart();
      }
    }
  }

  public static int CountStringOccurrences(string input, string text)
  {
    int num1 = 0;
    int startIndex = 0;
    int num2;
    while ((num2 = input.IndexOf(text, startIndex)) != -1)
    {
      startIndex = num2 + text.Length;
      ++num1;
    }
    return num1;
  }

  private string ReplaceFirst(string text, string search, string replace)
  {
    int length = text.IndexOf(search);
    return length < 0 ? text : text.Substring(0, length) + replace + text.Substring(length + search.Length);
  }

  public static string RandomUserAgent(Random rand)
  {
    int num = rand.Next(99) + 1;
    if (num >= 1 && num <= 70)
      return Extreme.Net.Http.ChromeUserAgent();
    if (num > 70 && num <= 85)
      return Extreme.Net.Http.FirefoxUserAgent();
    if (num > 85 && num <= 91)
      return Extreme.Net.Http.IEUserAgent();
    return num > 91 && num <= 96 /*0x60*/ ? Extreme.Net.Http.OperaUserAgent() : Extreme.Net.Http.OperaMiniUserAgent();
  }

  public static string[] SplitInChunks(string str, int chunkSize)
  {
    if (str.Length >= chunkSize)
      return Enumerable.Range(0, (int) Math.Ceiling((double) str.Length / (double) chunkSize)).Select<int, string>((System.Func<int, string>) (i => str.Substring(i * chunkSize, Math.Min(str.Length - i * chunkSize, chunkSize)))).ToArray<string>();
    return new string[1]{ str };
  }

  public enum Function
  {
    Constant,
    Base64Encode,
    Base64Decode,
    Hash,
    HMAC,
    Translate,
    DateToUnixTime,
    Length,
    ToLowercase,
    ToUppercase,
    Replace,
    RegexMatch,
    URLEncode,
    URLDecode,
    Unescape,
    HTMLEntityEncode,
    HTMLEntityDecode,
    UnixTimeToDate,
    CurrentUnixTime,
    UnixTimeToISO8601,
    RandomNum,
    RandomString,
    Ceil,
    Floor,
    Round,
    Compute,
    CountOccurrences,
    ClearCookies,
    RSAEncrypt,
    RSADecrypt,
    Delay,
    CharAt,
    Substring,
    ReverseString,
    Trim,
    GetRandomUA,
    AESEncrypt,
    AESDecrypt,
    PBKDF2PKCS5,
    GenerateOAuthVerifier,
    GenerateOAuthChallenge,
    GenerateGUID,
    GenerateBytes,
  }
}
