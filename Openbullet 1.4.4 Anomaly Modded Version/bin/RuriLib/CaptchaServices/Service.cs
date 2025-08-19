// Decompiled with JetBrains decompiler
// Type: RuriLib.CaptchaServices.Service
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using RuriLib.ViewModels;
using System;

#nullable disable
namespace RuriLib.CaptchaServices;

public static class Service
{
  public static CaptchaService Initialize(SettingsCaptchas cs)
  {
    switch (cs.CurrentService)
    {
      case ServiceType.AntiCaptcha:
        return (CaptchaService) new AntiCaptcha(cs.AntiCapToken, cs.Timeout);
      case ServiceType.AZCaptcha:
        return (CaptchaService) new AZCaptcha(cs.AZCapToken, cs.Timeout);
      case ServiceType.CaptchasIO:
        return (CaptchaService) new CaptchasIO(cs.CIOToken, cs.Timeout);
      case ServiceType.DBC:
        return (CaptchaService) new DeathByCaptcha(cs.DBCUser, cs.DBCPass, cs.Timeout);
      case ServiceType.DeCaptcher:
        return (CaptchaService) new DeCaptcher(cs.DCUser, cs.DCPass, cs.Timeout);
      case ServiceType.ImageTypers:
        return (CaptchaService) new ImageTyperz(cs.ImageTypToken, cs.Timeout);
      case ServiceType.SolveRecaptcha:
        return (CaptchaService) new SolveReCaptcha(cs.SRUserId, cs.SRToken, cs.Timeout);
      case ServiceType.TwoCaptcha:
        return (CaptchaService) new TwoCaptcha(cs.TwoCapToken, cs.Timeout);
      case ServiceType.RuCaptcha:
        return (CaptchaService) new RuCaptcha(cs.RuCapToken, cs.Timeout);
      case ServiceType.CustomTwoCaptcha:
        return (CaptchaService) new CustomTwoCaptcha(cs.CustomTwoCapToken, cs.CustomTwoCapDomain, cs.CustomTwoCapPort, cs.Timeout);
      default:
        throw new NotSupportedException("Service not supported");
    }
  }
}
