// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.MutableTuple`32
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.CodeDom.Compiler;

#nullable disable
namespace Microsoft.Scripting;

[GeneratedCode("DLR", "2.0")]
public class MutableTuple<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : 
  MutableTuple<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
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
    T15 item15,
    T16 item16,
    T17 item17,
    T18 item18,
    T19 item19,
    T20 item20,
    T21 item21,
    T22 item22,
    T23 item23,
    T24 item24,
    T25 item25,
    T26 item26,
    T27 item27,
    T28 item28,
    T29 item29,
    T30 item30,
    T31 item31)
    : base(item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15)
  {
    this.Item016 = item16;
    this.Item017 = item17;
    this.Item018 = item18;
    this.Item019 = item19;
    this.Item020 = item20;
    this.Item021 = item21;
    this.Item022 = item22;
    this.Item023 = item23;
    this.Item024 = item24;
    this.Item025 = item25;
    this.Item026 = item26;
    this.Item027 = item27;
    this.Item028 = item28;
    this.Item029 = item29;
    this.Item030 = item30;
    this.Item031 = item31;
  }

  public T16 Item016 { get; set; }

  public T17 Item017 { get; set; }

  public T18 Item018 { get; set; }

  public T19 Item019 { get; set; }

  public T20 Item020 { get; set; }

  public T21 Item021 { get; set; }

  public T22 Item022 { get; set; }

  public T23 Item023 { get; set; }

  public T24 Item024 { get; set; }

  public T25 Item025 { get; set; }

  public T26 Item026 { get; set; }

  public T27 Item027 { get; set; }

  public T28 Item028 { get; set; }

  public T29 Item029 { get; set; }

  public T30 Item030 { get; set; }

  public T31 Item031 { get; set; }

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
      case 16 /*0x10*/:
        return (object) this.Item016;
      case 17:
        return (object) this.Item017;
      case 18:
        return (object) this.Item018;
      case 19:
        return (object) this.Item019;
      case 20:
        return (object) this.Item020;
      case 21:
        return (object) this.Item021;
      case 22:
        return (object) this.Item022;
      case 23:
        return (object) this.Item023;
      case 24:
        return (object) this.Item024;
      case 25:
        return (object) this.Item025;
      case 26:
        return (object) this.Item026;
      case 27:
        return (object) this.Item027;
      case 28:
        return (object) this.Item028;
      case 29:
        return (object) this.Item029;
      case 30:
        return (object) this.Item030;
      case 31 /*0x1F*/:
        return (object) this.Item031;
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
      case 16 /*0x10*/:
        this.Item016 = (T16) value;
        break;
      case 17:
        this.Item017 = (T17) value;
        break;
      case 18:
        this.Item018 = (T18) value;
        break;
      case 19:
        this.Item019 = (T19) value;
        break;
      case 20:
        this.Item020 = (T20) value;
        break;
      case 21:
        this.Item021 = (T21) value;
        break;
      case 22:
        this.Item022 = (T22) value;
        break;
      case 23:
        this.Item023 = (T23) value;
        break;
      case 24:
        this.Item024 = (T24) value;
        break;
      case 25:
        this.Item025 = (T25) value;
        break;
      case 26:
        this.Item026 = (T26) value;
        break;
      case 27:
        this.Item027 = (T27) value;
        break;
      case 28:
        this.Item028 = (T28) value;
        break;
      case 29:
        this.Item029 = (T29) value;
        break;
      case 30:
        this.Item030 = (T30) value;
        break;
      case 31 /*0x1F*/:
        this.Item031 = (T31) value;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof (index));
    }
  }

  public override int Capacity => 32 /*0x20*/;
}
