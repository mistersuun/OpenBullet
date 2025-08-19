// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.MutableTuple`16
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.CodeDom.Compiler;

#nullable disable
namespace Microsoft.Scripting;

[GeneratedCode("DLR", "2.0")]
public class MutableTuple<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : 
  MutableTuple<T0, T1, T2, T3, T4, T5, T6, T7>
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
    T7 item7,
    T8 item8,
    T9 item9,
    T10 item10,
    T11 item11,
    T12 item12,
    T13 item13,
    T14 item14,
    T15 item15)
    : base(item0, item1, item2, item3, item4, item5, item6, item7)
  {
    this.Item008 = item8;
    this.Item009 = item9;
    this.Item010 = item10;
    this.Item011 = item11;
    this.Item012 = item12;
    this.Item013 = item13;
    this.Item014 = item14;
    this.Item015 = item15;
  }

  public T8 Item008 { get; set; }

  public T9 Item009 { get; set; }

  public T10 Item010 { get; set; }

  public T11 Item011 { get; set; }

  public T12 Item012 { get; set; }

  public T13 Item013 { get; set; }

  public T14 Item014 { get; set; }

  public T15 Item015 { get; set; }

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
      case 8:
        return (object) this.Item008;
      case 9:
        return (object) this.Item009;
      case 10:
        return (object) this.Item010;
      case 11:
        return (object) this.Item011;
      case 12:
        return (object) this.Item012;
      case 13:
        return (object) this.Item013;
      case 14:
        return (object) this.Item014;
      case 15:
        return (object) this.Item015;
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
      case 8:
        this.Item008 = (T8) value;
        break;
      case 9:
        this.Item009 = (T9) value;
        break;
      case 10:
        this.Item010 = (T10) value;
        break;
      case 11:
        this.Item011 = (T11) value;
        break;
      case 12:
        this.Item012 = (T12) value;
        break;
      case 13:
        this.Item013 = (T13) value;
        break;
      case 14:
        this.Item014 = (T14) value;
        break;
      case 15:
        this.Item015 = (T15) value;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (index));
    }
  }

  public override int Capacity => 16 /*0x10*/;
}
