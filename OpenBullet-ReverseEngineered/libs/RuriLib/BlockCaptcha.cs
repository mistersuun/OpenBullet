// Decompiled with JetBrains decompiler
// Type: RuriLib.BlockCaptcha
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using Newtonsoft.Json;
using RuriLib.CaptchaServices;
using System;
using System.Windows.Media;

#nullable disable
namespace RuriLib;

public abstract class BlockCaptcha : BlockBase
{
  [JsonIgnore]
  public double Balance { get; set; }

  public override void Process(BotData data)
  {
    base.Process(data);
    if (data.GlobalSettings.Captchas.BypassBalanceCheck)
      return;
    this.Balance = 0.0;
    data.Log(new LogEntry("Checking balance...", Colors.White));
    this.Balance = Service.Initialize(data.GlobalSettings.Captchas).GetBalance();
    if (this.Balance <= 0.0)
      throw new Exception($"[{data.GlobalSettings.Captchas.CurrentService}] Bad token/credentials or zero balance!");
    data.Log(new LogEntry($"[{data.GlobalSettings.Captchas.CurrentService}] Current Balance: ${this.Balance}", Colors.GreenYellow));
    data.Balance = this.Balance;
  }
}
