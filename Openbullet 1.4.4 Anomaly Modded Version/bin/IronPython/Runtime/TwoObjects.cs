// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.TwoObjects
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

internal class TwoObjects
{
  internal object obj1;
  internal object obj2;

  public TwoObjects(object obj1, object obj2)
  {
    this.obj1 = obj1;
    this.obj2 = obj2;
  }

  public override int GetHashCode() => throw new NotSupportedException();

  public override bool Equals(object other)
  {
    return other is TwoObjects twoObjects && twoObjects.obj1 == this.obj1 && twoObjects.obj2 == this.obj2;
  }
}
