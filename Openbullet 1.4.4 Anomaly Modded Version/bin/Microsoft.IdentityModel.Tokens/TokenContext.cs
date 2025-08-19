// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.TokenContext
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public class TokenContext
{
  public TokenContext()
  {
  }

  public TokenContext(Guid activityId) => this.ActivityId = activityId;

  public Guid ActivityId { get; set; } = Guid.Empty;

  public bool CaptureLogs { get; set; }

  public ICollection<string> Logs { get; private set; } = (ICollection<string>) new Collection<string>();
}
