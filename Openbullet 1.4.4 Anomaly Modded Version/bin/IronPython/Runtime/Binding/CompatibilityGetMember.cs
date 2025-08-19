// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.CompatibilityGetMember
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.ComInterop;
using System.Dynamic;

#nullable disable
namespace IronPython.Runtime.Binding;

internal class CompatibilityGetMember : GetMemberBinder, IPythonSite, IInvokeOnGetBinder
{
  private readonly PythonContext _context;
  private readonly bool _isNoThrow;

  public CompatibilityGetMember(PythonContext context, string name)
    : base(name, false)
  {
    this._context = context;
  }

  public CompatibilityGetMember(PythonContext context, string name, bool isNoThrow)
    : base(name, false)
  {
    this._context = context;
    this._isNoThrow = isNoThrow;
  }

  public override DynamicMetaObject FallbackGetMember(
    DynamicMetaObject self,
    DynamicMetaObject errorSuggestion)
  {
    DynamicMetaObject result;
    return ComBinder.TryBindGetMember((GetMemberBinder) this, self, out result, true) ? result : PythonGetMemberBinder.FallbackWorker(this._context, self, PythonContext.GetCodeContextMOCls((DynamicMetaObjectBinder) this), this.Name, this._isNoThrow ? GetMemberOptions.IsNoThrow : GetMemberOptions.None, (DynamicMetaObjectBinder) this, errorSuggestion);
  }

  public PythonContext Context => this._context;

  public override int GetHashCode() => base.GetHashCode() ^ this._context.Binder.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is CompatibilityGetMember compatibilityGetMember && compatibilityGetMember._context.Binder == this._context.Binder && base.Equals(obj);
  }

  public bool InvokeOnGet => false;
}
