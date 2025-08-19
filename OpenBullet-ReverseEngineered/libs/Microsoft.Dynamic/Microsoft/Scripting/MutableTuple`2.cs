// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.MutableTuple`2
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.CodeDom.Compiler;

#nullable disable
namespace Microsoft.Scripting;

[GeneratedCode("DLR", "2.0")]
public class MutableTuple<T0, T1> : MutableTuple<T0>
{
  public MutableTuple()
  {
  }

  public MutableTuple(T0 item0, T1 item1)
    : base(item0)
  {
    this.Item001 = item1;
  }

  public T1 Item001 { get; set; }

  public override object GetValue(int index)
  {
    if (index == 0)
      return (object) this.Item000;
    if (index == 1)
      return (object) this.Item001;
    throw new ArgumentOutOfRangeException(nameof (index));
  }

  public override void SetValue(int index, object value)
  {
    if (index != 0)
    {
      if (index != 1)
        throw new ArgumentOutOfRangeException(nameof (index));
      this.Item001 = (T1) value;
    }
    else
      this.Item000 = (T0) value;
  }

  public override int Capacity => 2;
}
