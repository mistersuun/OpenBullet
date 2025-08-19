// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.SystemColorHighlightingBrush
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
internal sealed class SystemColorHighlightingBrush : HighlightingBrush, ISerializable
{
  private readonly PropertyInfo property;

  public SystemColorHighlightingBrush(PropertyInfo property) => this.property = property;

  public override Brush GetBrush(ITextRunConstructionContext context)
  {
    return (Brush) this.property.GetValue((object) null, (object[]) null);
  }

  public override string ToString() => this.property.Name;

  private SystemColorHighlightingBrush(SerializationInfo info, StreamingContext context)
  {
    this.property = typeof (SystemColors).GetProperty(info.GetString("propertyName"));
    if (this.property == (PropertyInfo) null)
      throw new ArgumentException("Error deserializing SystemColorHighlightingBrush");
  }

  void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("propertyName", (object) this.property.Name);
  }

  public override bool Equals(object obj)
  {
    return obj is SystemColorHighlightingBrush highlightingBrush && object.Equals((object) this.property, (object) highlightingBrush.property);
  }

  public override int GetHashCode() => this.property.GetHashCode();
}
