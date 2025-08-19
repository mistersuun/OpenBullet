// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.InstanceFinalizer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;

#nullable disable
namespace IronPython.Runtime;

internal sealed class InstanceFinalizer
{
  private object _instance;

  internal InstanceFinalizer(CodeContext context, object inst) => this._instance = inst;

  internal object CallDirect(CodeContext context)
  {
    if (this._instance is OldInstance instance)
    {
      object func;
      if (instance.TryGetBoundCustomMember(context, "__del__", out func))
        return context.LanguageContext.CallSplat(func);
    }
    else
      PythonTypeOps.TryInvokeUnaryOperator(context, this._instance, "__del__", out object _);
    return (object) null;
  }
}
