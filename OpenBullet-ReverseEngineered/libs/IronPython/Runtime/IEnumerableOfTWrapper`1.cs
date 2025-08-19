// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.IEnumerableOfTWrapper`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

[PythonType("enumerable_wrapper")]
public class IEnumerableOfTWrapper<T> : IEnumerable<T>, IEnumerable
{
  private IEnumerable enumerable;

  public IEnumerableOfTWrapper(IEnumerable enumerable) => this.enumerable = enumerable;

  public IEnumerator<T> GetEnumerator()
  {
    return (IEnumerator<T>) new IEnumeratorOfTWrapper<T>(this.enumerable.GetEnumerator());
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
