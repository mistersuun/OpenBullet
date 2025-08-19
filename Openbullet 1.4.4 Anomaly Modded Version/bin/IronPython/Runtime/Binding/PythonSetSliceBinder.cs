// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonSetSliceBinder
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

internal class PythonSetSliceBinder : DynamicMetaObjectBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonSetSliceBinder(PythonContext context) => this._context = context;

  public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
  {
    return PythonProtocol.Index((DynamicMetaObjectBinder) this, PythonIndexType.SetSlice, ArrayUtils.Insert<DynamicMetaObject>(target, args));
  }

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonSetSliceBinder pythonSetSliceBinder && pythonSetSliceBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public PythonContext Context => this._context;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeSetSliceBinder"), BindingHelpers.CreateBinderStateExpression());
  }
}
