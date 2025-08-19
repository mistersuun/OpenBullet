// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonOverloadResolverFactory
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Runtime;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal sealed class PythonOverloadResolverFactory : OverloadResolverFactory
{
  private readonly PythonBinder _binder;
  internal readonly Expression _codeContext;

  public PythonOverloadResolverFactory(PythonBinder binder, Expression codeContext)
  {
    this._binder = binder;
    this._codeContext = codeContext;
  }

  public override DefaultOverloadResolver CreateOverloadResolver(
    IList<DynamicMetaObject> args,
    CallSignature signature,
    CallTypes callType)
  {
    return (DefaultOverloadResolver) new PythonOverloadResolver(this._binder, args, signature, callType, this._codeContext);
  }
}
