// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ItemEnumerable
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Collections;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("iterable")]
public class ItemEnumerable : IEnumerable
{
  private readonly object _getitem;
  private readonly CallSite<Func<CallSite, CodeContext, object, int, object>> _site;

  internal ItemEnumerable(
    object getitem,
    CallSite<Func<CallSite, CodeContext, object, int, object>> site)
  {
    this._getitem = getitem;
    this._site = site;
  }

  public IEnumerator __iter__() => ((IEnumerable) this).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator) new ItemEnumerator(this._getitem, this._site);
  }
}
