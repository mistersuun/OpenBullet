// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.Extensible`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class Extensible<T>
{
  public Extensible()
  {
  }

  public Extensible(T value) => this.Value = value;

  public T Value { get; }

  public override bool Equals(object obj) => this.Value.Equals(obj);

  public override int GetHashCode() => this.Value.GetHashCode();

  public override string ToString() => this.Value.ToString();

  public static implicit operator T(Extensible<T> extensible) => extensible.Value;
}
