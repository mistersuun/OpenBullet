// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.OverloadResolverFactory
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public abstract class OverloadResolverFactory
{
  public abstract DefaultOverloadResolver CreateOverloadResolver(
    IList<DynamicMetaObject> args,
    CallSignature signature,
    CallTypes callType);
}
