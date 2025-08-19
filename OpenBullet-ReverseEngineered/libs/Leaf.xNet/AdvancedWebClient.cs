// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.AdvancedWebClient
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;
using System.Net;

#nullable disable
namespace Leaf.xNet;

public class AdvancedWebClient : WebClient
{
  public int Timeout { get; set; } = 10000;

  public int ReadWriteTimeout { get; set; } = 10000;

  public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;

  public bool ServerCertificateValidation { get; set; }

  protected override WebRequest GetWebRequest(Uri uri)
  {
    // ISSUE: unable to decompile the method.
  }
}
