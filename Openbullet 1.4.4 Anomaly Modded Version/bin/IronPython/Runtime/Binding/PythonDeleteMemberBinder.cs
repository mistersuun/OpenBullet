// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.PythonDeleteMemberBinder
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Actions.Calls;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class PythonDeleteMemberBinder : DeleteMemberBinder, IPythonSite, IExpressionSerializable
{
  private readonly PythonContext _context;

  public PythonDeleteMemberBinder(PythonContext context, string name)
    : base(name, false)
  {
    this._context = context;
  }

  public PythonDeleteMemberBinder(PythonContext context, string name, bool ignoreCase)
    : base(name, ignoreCase)
  {
    this._context = context;
  }

  public override DynamicMetaObject FallbackDeleteMember(
    DynamicMetaObject self,
    DynamicMetaObject errorSuggestion)
  {
    return self.NeedsDeferral() ? this.Defer(self) : this.Context.Binder.DeleteMember(this.Name, self, (OverloadResolverFactory) new PythonOverloadResolverFactory(this._context.Binder, Utils.Constant((object) this.Context.SharedContext)), errorSuggestion);
  }

  public PythonContext Context => this._context;

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is PythonDeleteMemberBinder deleteMemberBinder && deleteMemberBinder._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public override string ToString() => "Python DeleteMember " + this.Name;

  public Expression CreateExpression()
  {
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeDeleteAction"), BindingHelpers.CreateBinderStateExpression(), Utils.Constant((object) this.Name));
  }
}
