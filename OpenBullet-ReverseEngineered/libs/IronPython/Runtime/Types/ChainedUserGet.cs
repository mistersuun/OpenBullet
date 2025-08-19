// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.ChainedUserGet
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Types;

internal class ChainedUserGet : UserGetBase
{
  public ChainedUserGet(
    PythonGetMemberBinder binder,
    int version,
    Func<CallSite, object, CodeContext, object> func)
    : base(binder, version)
  {
    this._func = func;
  }

  internal override bool ShouldCache => false;
}
