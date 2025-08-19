// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.IEnumeratorOfTWrapper`1
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Runtime;

public class IEnumeratorOfTWrapper<T> : IEnumerator<T>, IDisposable, IEnumerator
{
  private IEnumerator enumerable;

  public IEnumeratorOfTWrapper(IEnumerator enumerable) => this.enumerable = enumerable;

  public T Current
  {
    get
    {
      try
      {
        return (T) this.enumerable.Current;
      }
      catch (InvalidCastException ex)
      {
        throw new InvalidCastException($"Error in IEnumeratorOfTWrapper.Current. Could not cast: {typeof (T).ToString()} in {this.enumerable.Current.GetType().ToString()}", (Exception) ex);
      }
    }
  }

  public void Dispose()
  {
  }

  object IEnumerator.Current => this.enumerable.Current;

  public bool MoveNext() => this.enumerable.MoveNext();

  public void Reset() => this.enumerable.Reset();
}
