// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.DataGridLength
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using MS.Internal;
using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace Microsoft.Windows.Controls;

[TypeConverter(typeof (DataGridLengthConverter))]
public struct DataGridLength : IEquatable<DataGridLength>
{
  private const double AutoValue = 1.0;
  private double _unitValue;
  private DataGridLengthUnitType _unitType;
  private double _desiredValue;
  private double _displayValue;
  private static readonly DataGridLength _auto = new DataGridLength(1.0, DataGridLengthUnitType.Auto, 0.0, 0.0);
  private static readonly DataGridLength _sizeToCells = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells, 0.0, 0.0);
  private static readonly DataGridLength _sizeToHeader = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader, 0.0, 0.0);

  public DataGridLength(double pixels)
    : this(pixels, DataGridLengthUnitType.Pixel)
  {
  }

  public DataGridLength(double value, DataGridLengthUnitType type)
    : this(value, type, type == DataGridLengthUnitType.Pixel ? value : double.NaN, type == DataGridLengthUnitType.Pixel ? value : double.NaN)
  {
  }

  public DataGridLength(
    double value,
    DataGridLengthUnitType type,
    double desiredValue,
    double displayValue)
  {
    if (DoubleUtil.IsNaN(value) || double.IsInfinity(value))
      throw new ArgumentException(SR.Get(SRID.DataGridLength_Infinity), nameof (value));
    if (type != DataGridLengthUnitType.Auto && type != DataGridLengthUnitType.Pixel && type != DataGridLengthUnitType.Star && type != DataGridLengthUnitType.SizeToCells && type != DataGridLengthUnitType.SizeToHeader)
      throw new ArgumentException(SR.Get(SRID.DataGridLength_InvalidType), nameof (type));
    if (double.IsInfinity(desiredValue))
      throw new ArgumentException(SR.Get(SRID.DataGridLength_Infinity), nameof (desiredValue));
    if (double.IsInfinity(displayValue))
      throw new ArgumentException(SR.Get(SRID.DataGridLength_Infinity), nameof (displayValue));
    this._unitValue = type == DataGridLengthUnitType.Auto ? 1.0 : value;
    this._unitType = type;
    this._desiredValue = desiredValue;
    this._displayValue = displayValue;
  }

  public static bool operator ==(DataGridLength gl1, DataGridLength gl2)
  {
    return gl1.UnitType == gl2.UnitType && gl1.Value == gl2.Value && gl1.DesiredValue == gl2.DesiredValue && gl1.DisplayValue == gl2.DisplayValue;
  }

  public static bool operator !=(DataGridLength gl1, DataGridLength gl2)
  {
    return gl1.UnitType != gl2.UnitType || gl1.Value != gl2.Value || gl1.DesiredValue != gl2.DesiredValue || gl1.DisplayValue != gl2.DisplayValue;
  }

  public override bool Equals(object obj)
  {
    return obj is DataGridLength dataGridLength && this == dataGridLength;
  }

  public bool Equals(DataGridLength other) => this == other;

  public override int GetHashCode()
  {
    return (int) ((int) this._unitValue + this._unitType + (int) this._desiredValue + (int) this._displayValue);
  }

  public bool IsAbsolute => this._unitType == DataGridLengthUnitType.Pixel;

  public bool IsAuto => this._unitType == DataGridLengthUnitType.Auto;

  public bool IsStar => this._unitType == DataGridLengthUnitType.Star;

  public bool IsSizeToCells => this._unitType == DataGridLengthUnitType.SizeToCells;

  public bool IsSizeToHeader => this._unitType == DataGridLengthUnitType.SizeToHeader;

  public double Value => this._unitType != DataGridLengthUnitType.Auto ? this._unitValue : 1.0;

  public DataGridLengthUnitType UnitType => this._unitType;

  public double DesiredValue => this._desiredValue;

  public double DisplayValue => this._displayValue;

  public override string ToString()
  {
    return DataGridLengthConverter.ConvertToString(this, CultureInfo.InvariantCulture);
  }

  public static DataGridLength Auto => DataGridLength._auto;

  public static DataGridLength SizeToCells => DataGridLength._sizeToCells;

  public static DataGridLength SizeToHeader => DataGridLength._sizeToHeader;

  public static implicit operator DataGridLength(double value) => new DataGridLength(value);
}
