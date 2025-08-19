// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.CorsRequest
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

#nullable disable
namespace AngleSharp.Io;

public class CorsRequest
{
  public CorsRequest(ResourceRequest request) => this.Request = request;

  public ResourceRequest Request { get; }

  public CorsSetting Setting { get; set; }

  public OriginBehavior Behavior { get; set; }

  public IIntegrityProvider Integrity { get; set; }
}
