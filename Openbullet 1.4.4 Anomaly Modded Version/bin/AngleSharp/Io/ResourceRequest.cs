// Decompiled with JetBrains decompiler
// Type: AngleSharp.Io.ResourceRequest
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Io;

public class ResourceRequest
{
  public ResourceRequest(IElement source, Url target)
  {
    this.Source = source;
    this.Target = target;
    this.Origin = source.Owner.Origin;
    this.IsManualRedirectDesired = false;
    this.IsSameOriginForced = false;
    this.IsCookieBlocked = false;
    this.IsCredentialOmitted = false;
  }

  public IElement Source { get; }

  public Url Target { get; }

  public string Origin { get; set; }

  public bool IsManualRedirectDesired { get; set; }

  public bool IsSameOriginForced { get; set; }

  public bool IsCredentialOmitted { get; set; }

  public bool IsCookieBlocked { get; set; }
}
