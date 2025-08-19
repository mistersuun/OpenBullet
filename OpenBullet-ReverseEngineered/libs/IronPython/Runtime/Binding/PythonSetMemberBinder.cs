// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonSetMemberBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Runtime;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonSetMemberBinder : SetMemberBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonSetMemberBinder(PythonContext context, string name)
    : base(name, false)
  {
    this._context = context;
  }

  public PythonSetMemberBinder(PythonContext context, string name, bool ignoreCase)
    : base(name, ignoreCase)
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackSetMember(
    DynamicMetaObject self,
    DynamicMetaObject value,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject result;
    return self.NeedsDeferral() ? this.Defer(self, value) : (ComBinder.TryBindSetMember((SetMemberBinder) this, self, BindingHelpers.GetComArgument(value), out result) ? result : this.Context.Binder.SetMember(this.Name, self, value, errorSuggestion, (OverloadResolverFactory) new PythonOverloadResolverFactory(this._context.Binder, Utils.Constant((object) this.Context.SharedContext))));
  }

  public override T BindDelegate<T>(CallSite<T> site, object[] args)
  {
    if (args[0] is IFastSettable fastSettable)
    {
      T obj = fastSettable.MakeSetBinding<T>(site, this);
      if ((object) obj != null)
        return obj;
    }
    if (args[0] is IPythonObject self && !(self is IProxyObject))
    {
      FastBindResult<T> fastBindResult = UserTypeOps.MakeSetBinding<T>(this.Context.SharedContext, site, self, args[1], this);
      if ((object) fastBindResult.Target != null)
      {
        if (fastBindResult.ShouldCache)
          this.CacheTarget<T>(fastBindResult.Target);
        return fastBindResult.Target;
      }
    }
    return base.BindDelegate<T>(site, args);
  }

  public PythonContext Context => this._context;

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonSetMemberBinder pythonSetMemberBinder && pythonSetMemberBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public override string ToString() => "Python SetMember " + this.Name;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeSetAction"), BindingHelpers.CreateBinderStateExpression(), Utils.Constant((object) this.Name));
  }
}
