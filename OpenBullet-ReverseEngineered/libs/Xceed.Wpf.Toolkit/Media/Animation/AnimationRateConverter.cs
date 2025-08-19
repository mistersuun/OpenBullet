// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.Media.Animation.AnimationRateConverter
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Xceed.Wpf.Toolkit.Media.Animation;

public class AnimationRateConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext td, Type t)
  {
    return t == typeof (string) || t == typeof (double) || t == typeof (int) || t == typeof (TimeSpan);
  }

  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    return destinationType == typeof (InstanceDescriptor) || destinationType == typeof (string) || destinationType == typeof (double) || destinationType == typeof (TimeSpan);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext td,
    CultureInfo cultureInfo,
    object value)
  {
    Type type = value.GetType();
    if (value is string)
      return (value as string).Contains(":") ? (object) new AnimationRate((TimeSpan) TypeDescriptor.GetConverter((object) TimeSpan.Zero).ConvertFrom(td, cultureInfo, value)) : (object) new AnimationRate((double) TypeDescriptor.GetConverter((object) 0.0).ConvertFrom(td, cultureInfo, value));
    if (type == typeof (double))
      return (object) (AnimationRate) (double) value;
    return type == typeof (int) ? (object) (AnimationRate) (int) value : (object) (AnimationRate) (TimeSpan) value;
  }

  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo cultureInfo,
    object value,
    Type destinationType)
  {
    if (destinationType != (Type) null && value is AnimationRate animationRate)
    {
      if (destinationType == typeof (InstanceDescriptor))
      {
        if (animationRate.HasDuration)
          return (object) new InstanceDescriptor((MemberInfo) typeof (AnimationRate).GetConstructor(new Type[1]
          {
            typeof (TimeSpan)
          }), (ICollection) new object[1]
          {
            (object) animationRate.Duration
          });
        if (animationRate.HasSpeed)
          return (object) new InstanceDescriptor((MemberInfo) typeof (AnimationRate).GetConstructor(new Type[1]
          {
            typeof (double)
          }), (ICollection) new object[1]
          {
            (object) animationRate.Speed
          });
      }
      else
      {
        if (destinationType == typeof (string))
          return (object) animationRate.ToString();
        if (destinationType == typeof (double))
          return (object) (animationRate.HasSpeed ? animationRate.Speed : 0.0);
        if (destinationType == typeof (TimeSpan))
          return (object) (animationRate.HasDuration ? animationRate.Duration : TimeSpan.FromSeconds(0.0));
      }
    }
    return base.ConvertTo(context, cultureInfo, value, destinationType);
  }
}
