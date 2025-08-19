// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.ArgumentArray
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class ArgumentArray
{
  private readonly object[] _arguments;
  private readonly int _first;
  private static readonly MethodInfo _GetArgMethod = RuntimeReflectionExtensions.GetMethodInfo((Delegate) new Func<ArgumentArray, int, object>(ArgumentArray.GetArg));

  internal ArgumentArray(object[] arguments, int first, int count)
  {
    this._arguments = arguments;
    this._first = first;
    this.Count = count;
  }

  public int Count { get; }

  public object GetArgument(int index)
  {
    ContractUtils.RequiresArrayIndex<object>((IList<object>) this._arguments, index, nameof (index));
    return this._arguments[this._first + index];
  }

  public DynamicMetaObject GetMetaObject(Expression parameter, int index)
  {
    return DynamicMetaObject.Create(this.GetArgument(index), (Expression) Expression.Call(ArgumentArray._GetArgMethod, Microsoft.Scripting.Ast.Utils.Convert(parameter, typeof (ArgumentArray)), Microsoft.Scripting.Ast.Utils.Constant((object) index)));
  }

  [CLSCompliant(false)]
  public static object GetArg(ArgumentArray array, int index)
  {
    return array._arguments[array._first + index];
  }
}
