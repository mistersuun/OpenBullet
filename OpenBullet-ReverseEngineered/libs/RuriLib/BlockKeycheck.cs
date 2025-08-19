// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockKeycheck
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Microsoft.CSharp.RuntimeBinder;
using RuriLib.Functions.Conditions;
using RuriLib.LS;
using RuriLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

#nullable disable
namespace RuriLib;

public class BlockKeycheck : BlockBase
{
  private bool banOn4XX;
  private bool banOnToCheck = true;
  public List<KeyChain> KeyChains = new List<KeyChain>();

  public bool BanOn4XX
  {
    get => this.banOn4XX;
    set
    {
      this.banOn4XX = value;
      this.OnPropertyChanged(nameof (BanOn4XX));
    }
  }

  public bool BanOnToCheck
  {
    get => this.banOnToCheck;
    set
    {
      this.banOnToCheck = value;
      this.OnPropertyChanged(nameof (BanOnToCheck));
    }
  }

  public BlockKeycheck() => this.Label = "KEY CHECK";

  public override BlockBase FromLS(string line)
  {
    string input = line.Trim();
    if (input.StartsWith("#"))
      this.Label = LineParser.ParseLabel(ref input);
    while (LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Boolean)
      LineParser.SetBool(ref input, (object) this);
    while (input != "")
    {
      LineParser.EnsureIdentifier(ref input, "KEYCHAIN");
      KeyChain keyChain1 = new KeyChain();
      KeyChain keyChain2 = keyChain1;
      // ISSUE: reference to a compiler-generated field
      if (BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, KeyChain.KeychainType>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (KeyChain.KeychainType), typeof (BlockKeycheck)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      int num1 = (int) BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__0, LineParser.ParseEnum(ref input, "Keychain Type", typeof (KeyChain.KeychainType)));
      keyChain2.Type = (KeyChain.KeychainType) num1;
      if (keyChain1.Type == KeyChain.KeychainType.Custom && LineParser.Lookahead(ref input) == RuriLib.LS.TokenType.Literal)
        keyChain1.CustomType = LineParser.ParseLiteral(ref input, "Custom Type");
      KeyChain keyChain3 = keyChain1;
      // ISSUE: reference to a compiler-generated field
      if (BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, KeyChain.KeychainMode>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (KeyChain.KeychainMode), typeof (BlockKeycheck)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      int num2 = (int) BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__1.Target((CallSite) BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__1, LineParser.ParseEnum(ref input, "Keychain Mode", typeof (KeyChain.KeychainMode)));
      keyChain3.Mode = (KeyChain.KeychainMode) num2;
      while (!input.StartsWith("KEYCHAIN") && input != "")
      {
        Key key1 = new Key();
        LineParser.EnsureIdentifier(ref input, "KEY");
        string literal = LineParser.ParseLiteral(ref input, "Left Term");
        if (LineParser.CheckIdentifier(ref input, "KEY") || LineParser.CheckIdentifier(ref input, "KEYCHAIN") || input == "")
        {
          key1.LeftTerm = "<SOURCE>";
          key1.Comparer = Comparer.Contains;
          key1.RightTerm = literal;
        }
        else
        {
          key1.LeftTerm = literal;
          Key key2 = key1;
          // ISSUE: reference to a compiler-generated field
          if (BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, Comparer>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (Comparer), typeof (BlockKeycheck)));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          int num3 = (int) BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__2.Target((CallSite) BlockKeycheck.\u003C\u003Eo__10.\u003C\u003Ep__2, LineParser.ParseEnum(ref input, "Condition", typeof (Comparer)));
          key2.Comparer = (Comparer) num3;
          if (key1.Comparer != Comparer.Exists && key1.Comparer != Comparer.DoesNotExist)
            key1.RightTerm = LineParser.ParseLiteral(ref input, "Right Term");
        }
        keyChain1.Keys.Add(key1);
      }
      this.KeyChains.Add(keyChain1);
    }
    return (BlockBase) this;
  }

  public override string ToLS(bool indent = true)
  {
    BlockWriter blockWriter = new BlockWriter(this.GetType(), indent, this.Disabled);
    blockWriter.Label(this.Label).Token((object) "KEYCHECK").Boolean(this.BanOn4XX, "BanOn4XX").Boolean(this.BanOnToCheck, "BanOnToCheck");
    foreach (KeyChain keyChain in this.KeyChains)
    {
      blockWriter.Indent().Token((object) "KEYCHAIN").Token((object) keyChain.Type);
      if (keyChain.Type == KeyChain.KeychainType.Custom)
        blockWriter.Literal(keyChain.CustomType);
      blockWriter.Token((object) keyChain.Mode);
      foreach (Key key in (Collection<Key>) keyChain.Keys)
      {
        if (key.LeftTerm == "<SOURCE>" && key.Comparer == Comparer.Contains)
        {
          blockWriter.Indent(2).Token((object) "KEY").Literal(key.RightTerm);
        }
        else
        {
          blockWriter.Indent(2).Token((object) "KEY").Literal(key.LeftTerm).Token((object) key.Comparer);
          if (key.Comparer != Comparer.Exists && key.Comparer != Comparer.DoesNotExist)
            blockWriter.Literal(key.RightTerm);
        }
      }
    }
    return blockWriter.ToString();
  }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.ResponseCode.StartsWith("4") && this.banOn4XX)
    {
      data.Status = BotStatus.BAN;
    }
    else
    {
      foreach (string globalBanKey in data.GlobalSettings.Proxies.GlobalBanKeys)
      {
        if (globalBanKey != "" && data.ResponseSource.Contains(globalBanKey))
        {
          data.Status = BotStatus.BAN;
          return;
        }
      }
      bool flag = false;
      foreach (KeyChain keyChain in this.KeyChains)
      {
        if (keyChain.CheckKeys(data))
        {
          flag = true;
          switch (keyChain.Type)
          {
            case KeyChain.KeychainType.Success:
              data.Status = BotStatus.SUCCESS;
              continue;
            case KeyChain.KeychainType.Failure:
              data.Status = BotStatus.FAIL;
              continue;
            case KeyChain.KeychainType.Ban:
              data.Status = BotStatus.BAN;
              continue;
            case KeyChain.KeychainType.Retry:
              data.Status = BotStatus.RETRY;
              continue;
            case KeyChain.KeychainType.Custom:
              data.Status = BotStatus.CUSTOM;
              data.CustomStatus = keyChain.CustomType;
              continue;
            default:
              continue;
          }
        }
      }
      if (flag || !this.banOnToCheck)
        return;
      data.Status = BotStatus.BAN;
    }
  }
}
