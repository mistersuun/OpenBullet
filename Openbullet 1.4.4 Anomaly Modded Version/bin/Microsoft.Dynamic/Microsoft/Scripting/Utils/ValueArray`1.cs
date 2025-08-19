// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.ValueArray`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public class ValueArray<T> : IEquatable<ValueArray<T>>
{
  private readonly T[] _array;

  public ValueArray(T[] array)
  {
    ContractUtils.RequiresNotNull((object) array, nameof (array));
    ContractUtils.RequiresNotNullItems<T>((IList<T>) array, nameof (array));
    this._array = array;
  }

  public bool Equals(ValueArray<T> other)
  {
    return other != null && this._array.ValueEquals<T>(other._array);
  }

  public override bool Equals(object obj) => this.Equals(obj as ValueArray<T>);

  public override int GetHashCode()
  {
    int hashCode = 6551;
    for (int index = 0; index < this._array.Length; ++index)
      hashCode ^= this._array[index].GetHashCode();
    return hashCode;
  }
}
