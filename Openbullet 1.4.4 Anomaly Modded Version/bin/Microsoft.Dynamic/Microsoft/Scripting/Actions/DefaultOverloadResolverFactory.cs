// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.DefaultOverloadResolverFactory
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Dynamic;

#nullable disable
namespace Microsoft.Scripting.Actions;

internal sealed class DefaultOverloadResolverFactory : OverloadResolverFactory
{
  private readonly DefaultBinder _binder;

  public DefaultOverloadResolverFactory(DefaultBinder binder) => this._binder = binder;

  public override DefaultOverloadResolver CreateOverloadResolver(
    IList<DynamicMetaObject> args,
    CallSignature signature,
    CallTypes callType)
  {
    return new DefaultOverloadResolver((ActionBinder) this._binder, args, signature, callType);
  }
}
