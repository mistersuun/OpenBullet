// Decompiled with JetBrains decompiler
// Type: Tesseract.Interop.LeptonicaApi
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using InteropDotNet;

#nullable disable
namespace Tesseract.Interop;

internal static class LeptonicaApi
{
  private static ILeptonicaApiSignatures native;

  public static ILeptonicaApiSignatures Native
  {
    get
    {
      if (LeptonicaApi.native == null)
        LeptonicaApi.Initialize();
      return LeptonicaApi.native;
    }
  }

  public static void Initialize()
  {
    if (LeptonicaApi.native != null)
      return;
    LeptonicaApi.native = InteropRuntimeImplementer.CreateInstance<ILeptonicaApiSignatures>();
  }
}
