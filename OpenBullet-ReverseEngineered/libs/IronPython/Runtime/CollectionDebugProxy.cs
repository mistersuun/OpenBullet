// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.CollectionDebugProxy
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime;

internal class CollectionDebugProxy
{
  private readonly ICollection _collection;

  public CollectionDebugProxy(ICollection collection) => this._collection = collection;

  [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
  internal IList Members
  {
    get
    {
      List<object> members = new List<object>(this._collection.Count);
      foreach (object obj in (IEnumerable) this._collection)
        members.Add(obj);
      return (IList) members;
    }
  }
}
