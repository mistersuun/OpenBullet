// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.ListEqualityComparer`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal sealed class ListEqualityComparer<T> : EqualityComparer<ICollection<T>>
{
  internal static readonly ListEqualityComparer<T> Instance = new ListEqualityComparer<T>();

  private ListEqualityComparer()
  {
  }

  public override bool Equals(ICollection<T> x, ICollection<T> y) => x.ListEquals<T>(y);

  public override int GetHashCode(ICollection<T> obj) => obj.ListHashCode<T>();
}
