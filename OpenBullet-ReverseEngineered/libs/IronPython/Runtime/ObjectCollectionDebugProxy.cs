// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ObjectCollectionDebugProxy
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime;

internal class ObjectCollectionDebugProxy
{
  private readonly ICollection<object> _collection;

  public ObjectCollectionDebugProxy(ICollection<object> collection)
  {
    this._collection = collection;
  }

  [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
  internal IList<object> Members
  {
    get => (IList<object>) new List<object>((IEnumerable<object>) this._collection);
  }
}
