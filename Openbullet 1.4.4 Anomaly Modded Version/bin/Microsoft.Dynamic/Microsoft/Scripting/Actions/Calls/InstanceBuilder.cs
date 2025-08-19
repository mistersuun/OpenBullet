// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.InstanceBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Generation;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public class InstanceBuilder
{
  private readonly int _index;

  public InstanceBuilder(int index) => this._index = index;

  public virtual bool HasValue => this._index != -1;

  public virtual int ConsumedArgumentCount => 1;

  protected internal virtual Expression ToExpression(
    ref MethodInfo method,
    OverloadResolver resolver,
    RestrictedArguments args,
    bool[] hasBeenUsed)
  {
    if (this._index == -1)
      return Utils.Constant((object) null);
    hasBeenUsed[this._index] = true;
    this.GetCallableMethod(args, ref method);
    return resolver.Convert(args.GetObject(this._index), args.GetType(this._index), (ParameterInfo) null, method.DeclaringType);
  }

  private void GetCallableMethod(RestrictedArguments args, ref MethodInfo method)
  {
    method = CompilerHelpers.TryGetCallableMethod(args.GetObject(this._index).LimitType, method);
  }
}
