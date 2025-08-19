// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonDeleteIndexBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonDeleteIndexBinder : DeleteIndexBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonDeleteIndexBinder(PythonContext context, int argCount)
    : base(new CallInfo(argCount, new string[0]))
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackDeleteIndex(
    DynamicMetaObject target,
    DynamicMetaObject[] indexes,
    DynamicMetaObject errorSuggestion)
  {
    return PythonProtocol.Index((DynamicMetaObjectBinder) this, PythonIndexType.DeleteItem, ArrayUtils.Insert<DynamicMetaObject>(target, indexes), errorSuggestion);
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonDeleteIndexBinder deleteIndexBinder && deleteIndexBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeDeleteIndexAction"), BindingHelpers.CreateBinderStateExpression(), Microsoft.Scripting.Ast.Utils.Constant((object) this.CallInfo.ArgumentCount));
  }
}
