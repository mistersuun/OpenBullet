// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.MutableTuple`8
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.CodeDom.Compiler;

#nullable disable
namespace Microsoft.Scripting;

[GeneratedCode("DLR", "2.0")]
public class MutableTuple<T0, T1, T2, T3, T4, T5, T6, T7> : MutableTuple<T0, T1, T2, T3>
{
  public MutableTuple()
  {
  }

  public MutableTuple(
    T0 item0,
    T1 item1,
    T2 item2,
    T3 item3,
    T4 item4,
    T5 item5,
    T6 item6,
    T7 item7)
    : base(item0, item1, item2, item3)
  {
    this.Item004 = item4;
    this.Item005 = item5;
    this.Item006 = item6;
    this.Item007 = item7;
  }

  public T4 Item004 { get; set; }

  public T5 Item005 { get; set; }

  public T6 Item006 { get; set; }

  public T7 Item007 { get; set; }

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
      case 4:
        return (object) this.Item004;
      case 5:
        return (object) this.Item005;
      case 6:
        return (object) this.Item006;
      case 7:
        return (object) this.Item007;
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
      case 4:
        this.Item004 = (T4) value;
        break;
      case 5:
        this.Item005 = (T5) value;
        break;
      case 6:
        this.Item006 = (T6) value;
        break;
      case 7:
        this.Item007 = (T7) value;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (index));
    }
  }

  public override int Capacity => 8;
}
