// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Media.Animation.AnimationRate
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit.Media.Animation;

[TypeConverter(typeof (AnimationRateConverter))]
[StructLayout(LayoutKind.Explicit)]
public struct AnimationRate
{
  private static AnimationRate _default = new AnimationRate(true);
  [FieldOffset(0)]
  private long _duration;
  [FieldOffset(0)]
  private double _speed;
  [FieldOffset(8)]
  private AnimationRate.RateType _rateType;

  public AnimationRate(TimeSpan duration)
  {
    if (duration < TimeSpan.Zero)
      throw new ArgumentException(ErrorMessages.GetMessage("NegativeTimeSpanNotSupported"));
    this._speed = 0.0;
    this._duration = duration.Ticks;
    this._rateType = AnimationRate.RateType.TimeSpan;
  }

  public AnimationRate(double speed)
  {
    if (DoubleHelper.IsNaN(speed) || speed < 0.0)
      throw new ArgumentException(ErrorMessages.GetMessage("NegativeSpeedNotSupported"));
    this._duration = 0L;
    this._speed = speed;
    this._rateType = AnimationRate.RateType.Speed;
  }

  private AnimationRate(bool ignore)
  {
    this._duration = 0L;
    this._speed = double.NaN;
    this._rateType = AnimationRate.RateType.Speed;
  }

  public static AnimationRate Default => AnimationRate._default;

  public bool HasDuration => this._rateType == AnimationRate.RateType.TimeSpan;

  public TimeSpan Duration
  {
    get
    {
      if (this.HasDuration)
        return TimeSpan.FromTicks(this._duration);
      throw new InvalidOperationException(string.Format(ErrorMessages.GetMessage("InvalidRatePropertyAccessed"), (object) nameof (Duration), (object) this, (object) "Speed"));
    }
  }

  public bool HasSpeed => this._rateType == AnimationRate.RateType.Speed;

  public double Speed
  {
    get
    {
      if (this.HasSpeed)
        return this._speed;
      throw new InvalidOperationException(string.Format(ErrorMessages.GetMessage("InvalidRatePropertyAccessed"), (object) nameof (Speed), (object) this, (object) "Duration"));
    }
  }

  public AnimationRate Add(AnimationRate animationRate) => this + animationRate;

  public override bool Equals(object value)
  {
    return value != null && value is AnimationRate animationRate && this.Equals(animationRate);
  }

  public bool Equals(AnimationRate animationRate)
  {
    if (this.HasDuration)
      return animationRate.HasDuration && this._duration == animationRate._duration;
    if (!animationRate.HasSpeed)
      return false;
    return DoubleHelper.IsNaN(this._speed) ? DoubleHelper.IsNaN(animationRate._speed) : this._speed == animationRate._speed;
  }

  public static bool Equals(AnimationRate t1, AnimationRate t2) => t1.Equals(t2);

  public override int GetHashCode()
  {
    return this.HasDuration ? this._duration.GetHashCode() : this._speed.GetHashCode();
  }

  public AnimationRate Subtract(AnimationRate animationRate) => this - animationRate;

  public override string ToString()
  {
    return this.HasDuration ? TypeDescriptor.GetConverter((object) this._duration).ConvertToString((object) this._duration) : TypeDescriptor.GetConverter((object) this._speed).ConvertToString((object) this._speed);
  }

  public static implicit operator AnimationRate(TimeSpan duration)
  {
    return !(duration < TimeSpan.Zero) ? new AnimationRate(duration) : throw new ArgumentException(ErrorMessages.GetMessage("NegativeTimeSpanNotSupported"));
  }

  public static implicit operator AnimationRate(double speed)
  {
    return !DoubleHelper.IsNaN(speed) && speed >= 0.0 ? new AnimationRate(speed) : throw new ArgumentException(ErrorMessages.GetMessage("NegativeSpeedNotSupported"));
  }

  public static implicit operator AnimationRate(int speed)
  {
    return !DoubleHelper.IsNaN((double) speed) && speed >= 0 ? new AnimationRate((double) speed) : throw new ArgumentException(ErrorMessages.GetMessage("NegativeSpeedNotSupported"));
  }

  public static AnimationRate operator +(AnimationRate t1, AnimationRate t2)
  {
    if (t1.HasDuration && t2.HasDuration)
      return new AnimationRate((double) (t1._duration + t2._duration));
    return t1.HasSpeed && t2.HasSpeed ? new AnimationRate(t1._speed + t2._speed) : (AnimationRate) 0.0;
  }

  public static AnimationRate operator -(AnimationRate t1, AnimationRate t2)
  {
    if (t1.HasDuration && t2.HasDuration)
      return new AnimationRate((double) (t1._duration - t2._duration));
    return t1.HasSpeed && t2.HasSpeed ? new AnimationRate(t1._speed - t2._speed) : (AnimationRate) 0.0;
  }

  public static bool operator ==(AnimationRate t1, AnimationRate t2) => t1.Equals(t2);

  public static bool operator !=(AnimationRate t1, AnimationRate t2) => !t1.Equals(t2);

  public static bool operator >(AnimationRate t1, AnimationRate t2)
  {
    if (t1.HasDuration && t2.HasDuration)
      return t1._duration > t2._duration;
    if (!t1.HasSpeed || !t2.HasSpeed)
      return t1.HasSpeed;
    return t1._speed > t2._speed && !DoubleHelper.AreVirtuallyEqual(t1._speed, t2._speed);
  }

  public static bool operator >=(AnimationRate t1, AnimationRate t2) => !(t1 < t2);

  public static bool operator <(AnimationRate t1, AnimationRate t2)
  {
    if (t1.HasDuration && t2.HasDuration)
      return t1._duration < t2._duration;
    if (!t1.HasSpeed || !t2.HasSpeed)
      return t1.HasDuration;
    return t1._speed < t2._speed && !DoubleHelper.AreVirtuallyEqual(t1._speed, t2._speed);
  }

  public static bool operator <=(AnimationRate t1, AnimationRate t2) => !(t1 > t2);

  public static int Compare(AnimationRate t1, AnimationRate t2)
  {
    if (t1 < t2)
      return -1;
    return t1 > t2 ? 1 : 0;
  }

  public static AnimationRate Plus(AnimationRate animationRate) => animationRate;

  public static AnimationRate operator +(AnimationRate animationRate) => animationRate;

  private enum RateType
  {
    TimeSpan,
    Speed,
  }
}
