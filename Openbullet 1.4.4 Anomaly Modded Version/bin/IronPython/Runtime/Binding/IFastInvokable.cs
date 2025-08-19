// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.IFastInvokable
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal interface IFastInvokable
{
  FastBindResult<T> MakeInvokeBinding<T>(
    CallSite<T> site,
    PythonInvokeBinder binder,
    CodeContext context,
    object[] args)
    where T : class;
}
