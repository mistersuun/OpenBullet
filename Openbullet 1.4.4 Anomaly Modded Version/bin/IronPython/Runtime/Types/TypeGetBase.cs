// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.TypeGetBase
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Binding;
using System;

#nullable disable
namespace IronPython.Runtime.Types;

internal abstract class TypeGetBase : FastGetBase
{
  private readonly FastGetDelegate[] _delegates;

  public TypeGetBase(PythonGetMemberBinder binder, FastGetDelegate[] delegates)
  {
    this._delegates = delegates;
  }

  protected object RunDelegates(object self, CodeContext context)
  {
    ++this._hitCount;
    for (int index = 0; index < this._delegates.Length; ++index)
    {
      object result;
      if (this._delegates[index](context, self, out result))
        return result;
    }
    throw new InvalidOperationException();
  }

  protected object RunDelegatesNoOptimize(object self, CodeContext context)
  {
    for (int index = 0; index < this._delegates.Length; ++index)
    {
      object result;
      if (this._delegates[index](context, self, out result))
        return result;
    }
    throw new InvalidOperationException();
  }
}
