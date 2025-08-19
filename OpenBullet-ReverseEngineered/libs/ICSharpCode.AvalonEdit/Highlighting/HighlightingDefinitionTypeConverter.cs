// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingDefinitionTypeConverter
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

public sealed class HighlightingDefinitionTypeConverter : TypeConverter
{
  public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
  {
    return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
  }

  public override object ConvertFrom(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value)
  {
    return value is string name ? (object) HighlightingManager.Instance.GetDefinition(name) : base.ConvertFrom(context, culture, value);
  }

  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    return destinationType == typeof (string) || base.CanConvertTo(context, destinationType);
  }

  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value,
    Type destinationType)
  {
    return value is IHighlightingDefinition highlightingDefinition && destinationType == typeof (string) ? (object) highlightingDefinition.Name : base.ConvertTo(context, culture, value, destinationType);
  }
}
