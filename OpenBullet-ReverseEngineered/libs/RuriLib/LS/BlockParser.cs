// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.BlockParser
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace RuriLib.LS;

public static class BlockParser
{
  public static bool IsBlock(string line)
  {
    return ((IEnumerable<string>) Enum.GetNames(typeof (BlockParser.BlockName))).Select<string, string>((Func<string, string>) (n => n.ToUpper())).Contains<string>(BlockParser.GetBlockType(line).ToUpper());
  }

  public static string GetBlockType(string line)
  {
    return Regex.Match(line, "^!?(#[^ ]* )?([^ ]*)").Groups[2].Value;
  }

  public static BlockBase Parse(string line)
  {
    string input = line.Trim();
    bool flag = !(input == "") ? input.StartsWith("!") : throw new ArgumentNullException();
    if (flag)
      input = input.Substring(1).Trim();
    string token1 = LineParser.ParseToken(ref input, TokenType.Label, false);
    string token2;
    try
    {
      token2 = LineParser.ParseToken(ref input, TokenType.Parameter, true);
    }
    catch
    {
      throw new ArgumentException("Missing identifier");
    }
    BlockBase blockBase;
    switch ((BlockParser.BlockName) Enum.Parse(typeof (BlockParser.BlockName), token2, true))
    {
      case BlockParser.BlockName.BYPASSCF:
        blockBase = new BlockBypassCF().FromLS(input);
        break;
      case BlockParser.BlockName.CAPTCHA:
        blockBase = new BlockImageCaptcha().FromLS(input);
        break;
      case BlockParser.BlockName.FUNCTION:
        blockBase = new BlockFunction().FromLS(input);
        break;
      case BlockParser.BlockName.KEYCHECK:
        blockBase = new BlockKeycheck().FromLS(input);
        break;
      case BlockParser.BlockName.PARSE:
        blockBase = new BlockParse().FromLS(input);
        break;
      case BlockParser.BlockName.RECAPTCHA:
        blockBase = new BlockRecaptcha().FromLS(input);
        break;
      case BlockParser.BlockName.REQUEST:
        blockBase = new BlockRequest().FromLS(input);
        break;
      case BlockParser.BlockName.TCP:
        blockBase = new BlockTCP().FromLS(input);
        break;
      case BlockParser.BlockName.OCR:
        blockBase = new BlockOCR().FromLS(input);
        break;
      case BlockParser.BlockName.BlockchainDNS:
        blockBase = new BlockBlockchainDNS().FromLS(input);
        break;
      case BlockParser.BlockName.UTILITY:
        blockBase = new BlockUtility().FromLS(input);
        break;
      case BlockParser.BlockName.BROWSERACTION:
        blockBase = new SBlockBrowserAction().FromLS(input);
        break;
      case BlockParser.BlockName.ELEMENTACTION:
        blockBase = new SBlockElementAction().FromLS(input);
        break;
      case BlockParser.BlockName.EXECUTEJS:
        blockBase = new SBlockExecuteJS().FromLS(input);
        break;
      case BlockParser.BlockName.NAVIGATE:
        blockBase = new SBlockNavigate().FromLS(input);
        break;
      default:
        throw new ArgumentException($"Invalid identifier '{token2}'");
    }
    if (blockBase != null)
      blockBase.Disabled = flag;
    if (blockBase != null && token1 != "")
      blockBase.Label = token1.Replace("#", "");
    return blockBase;
  }

  public enum BlockName
  {
    BYPASSCF,
    CAPTCHA,
    FUNCTION,
    KEYCHECK,
    PARSE,
    RECAPTCHA,
    REQUEST,
    TCP,
    OCR,
    BlockchainDNS,
    UTILITY,
    BROWSERACTION,
    ELEMENTACTION,
    EXECUTEJS,
    NAVIGATE,
  }
}
