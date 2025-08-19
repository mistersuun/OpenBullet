// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.MutableTuple`4
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.CodeDom.Compiler;

#nullable disable
namespace Microsoft.Scripting;

[GeneratedCode("DLR", "2.0")]
public class MutableTuple<T0, T1, T2, T3> : MutableTuple<T0, T1>
{
  public MutableTuple()
  {
  }

  public MutableTuple(T0 item0, T1 item1, T2 item2, T3 item3)
    : base(item0, item1)
  {
    this.Item002 = item2;
    this.Item003 = item3;
  }

  public T2 Item002 { get; set; }

  public T3 Item003 { get; set; }

  public override object GetValue(int index)
  {
    switch (index)
    {
      case 0:
        return (object) this.Item000;
      case 1:
        return (object) this.Item001;
      case 2:
        return (object) this.Item002;
      case 3:
        return (object) this.Item003;
      default:
        throw new ArgumentOutOfRangeException(nameof (index));
    }
  }

  public override void SetValue(int index, object value)
  {
    switch (index)
    {
      case 0:
        this.Item000 = (T0) value;
        break;
      case 1:
        this.Item001 = (T1) value;
        break;
      case 2:
        this.Item002 = (T2) value;
        break;
      case 3:
        this.Item003 = (T3) value;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (index));
    }
  }

  public override int Capacity => 4;
}
