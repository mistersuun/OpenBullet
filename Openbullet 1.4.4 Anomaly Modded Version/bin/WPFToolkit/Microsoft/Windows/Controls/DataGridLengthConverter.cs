// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridLengthConverter
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Security;

#nullable disable
namespace Microsoft.Windows.Controls;

public class DataGridLengthConverter : TypeConverter
{
  private const int NumDescriptiveUnits = 3;
  private static string[] _unitStrings = new string[5]
  {
    "auto",
    "px",
    "sizetocells",
    "sizetoheader",
    "*"
  };
  private static string[] _nonStandardUnitStrings = new string[3]
  {
    "in",
    "cm",
    "pt"
  };
  private static double[] _pixelUnitFactors = new double[3]
  {
    96.0,
    4800.0 / (double) sbyte.MaxValue,
    4.0 / 3.0
  };

  public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
  {
    switch (Type.GetTypeCode(sourceType))
    {
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
      case TypeCode.Decimal:
      case TypeCode.String:
        return true;
      default:
        return false;
    }
  }

  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    return (object) destinationType == (object) typeof (string) || (object) destinationType == (object) typeof (InstanceDescriptor);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value)
  {
    if (value != null)
    {
      if (value is string s)
        return (object) DataGridLengthConverter.ConvertFromString(s, culture);
      double d = Convert.ToDouble(value, (IFormatProvider) culture);
      DataGridLengthUnitType type;
      if (DoubleUtil.IsNaN(d))
      {
        d = 1.0;
        type = DataGridLengthUnitType.Auto;
      }
      else
        type = DataGridLengthUnitType.Pixel;
      if (!double.IsInfinity(d))
        return (object) new DataGridLength(d, type);
    }
    throw this.GetConvertFromException(value);
  }

  [SecurityCritical]
  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value,
    Type destinationType)
  {
    if ((object) destinationType == null)
      throw new ArgumentNullException(nameof (destinationType));
    if (value != null && value is DataGridLength length)
    {
      if ((object) destinationType == (object) typeof (string))
        return (object) DataGridLengthConverter.ConvertToString(length, culture);
      if ((object) destinationType == (object) typeof (InstanceDescriptor))
        return (object) new InstanceDescriptor((MemberInfo) typeof (DataGridLength).GetConstructor(new Type[2]
        {
          typeof (double),
          typeof (DataGridLengthUnitType)
        }), (ICollection) new object[2]
        {
          (object) length.Value,
          (object) length.UnitType
        });
    }
    throw this.GetConvertToException(value, destinationType);
  }

  internal static string ConvertToString(DataGridLength length, CultureInfo cultureInfo)
  {
    switch (length.UnitType)
    {
      case DataGridLengthUnitType.Auto:
      case DataGridLengthUnitType.SizeToCells:
      case DataGridLengthUnitType.SizeToHeader:
        return length.UnitType.ToString();
      case DataGridLengthUnitType.Star:
        return !DoubleUtil.IsOne(length.Value) ? Convert.ToString(length.Value, (IFormatProvider) cultureInfo) + "*" : "*";
      default:
        return Convert.ToString(length.Value, (IFormatProvider) cultureInfo);
    }
  }

  private static DataGridLength ConvertFromString(string s, CultureInfo cultureInfo)
  {
    string lowerInvariant = s.Trim().ToLowerInvariant();
    for (int type = 0; type < 3; ++type)
    {
      string unitString = DataGridLengthConverter._unitStrings[type];
      if (lowerInvariant == unitString)
        return new DataGridLength(1.0, (DataGridLengthUnitType) type);
    }
    double num1 = 0.0;
    DataGridLengthUnitType type1 = DataGridLengthUnitType.Pixel;
    int length1 = lowerInvariant.Length;
    int num2 = 0;
    double num3 = 1.0;
    int length2 = DataGridLengthConverter._unitStrings.Length;
    for (int index = 3; index < length2; ++index)
    {
      string unitString = DataGridLengthConverter._unitStrings[index];
      if (lowerInvariant.EndsWith(unitString, StringComparison.Ordinal))
      {
        num2 = unitString.Length;
        type1 = (DataGridLengthUnitType) index;
        break;
      }
    }
    if (num2 == 0)
    {
      int length3 = DataGridLengthConverter._nonStandardUnitStrings.Length;
      for (int index = 0; index < length3; ++index)
      {
        string standardUnitString = DataGridLengthConverter._nonStandardUnitStrings[index];
        if (lowerInvariant.EndsWith(standardUnitString, StringComparison.Ordinal))
        {
          num2 = standardUnitString.Length;
          num3 = DataGridLengthConverter._pixelUnitFactors[index];
          break;
        }
      }
    }
    if (length1 == num2)
    {
      if (type1 == DataGridLengthUnitType.Star)
        num1 = 1.0;
    }
    else
      num1 = Convert.ToDouble(lowerInvariant.Substring(0, length1 - num2), (IFormatProvider) cultureInfo) * num3;
    return new DataGridLength(num1, type1);
  }
}
