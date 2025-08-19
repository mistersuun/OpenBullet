// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdColor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Runtime.Serialization;
using System.Security;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdColor : XshdElement, ISerializable
{
  public string Name { get; set; }

  public HighlightingBrush Foreground { get; set; }

  public HighlightingBrush Background { get; set; }

  public System.Windows.FontWeight? FontWeight { get; set; }

  public bool? Underline { get; set; }

  public System.Windows.FontStyle? FontStyle { get; set; }

  public string ExampleText { get; set; }

  public XshdColor()
  {
  }

  protected XshdColor(SerializationInfo info, StreamingContext context)
  {
    this.Name = info != null ? info.GetString(nameof (Name)) : throw new ArgumentNullException(nameof (info));
    this.Foreground = (HighlightingBrush) info.GetValue(nameof (Foreground), typeof (HighlightingBrush));
    this.Background = (HighlightingBrush) info.GetValue(nameof (Background), typeof (HighlightingBrush));
    if (info.GetBoolean("HasWeight"))
      this.FontWeight = new System.Windows.FontWeight?(System.Windows.FontWeight.FromOpenTypeWeight(info.GetInt32("Weight")));
    if (info.GetBoolean("HasStyle"))
      this.FontStyle = (System.Windows.FontStyle?) new FontStyleConverter().ConvertFromInvariantString(info.GetString("Style"));
    this.ExampleText = info.GetString(nameof (ExampleText));
    if (!info.GetBoolean("HasUnderline"))
      return;
    this.Underline = new bool?(info.GetBoolean(nameof (Underline)));
  }

  [SecurityCritical]
  public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    if (info == null)
      throw new ArgumentNullException(nameof (info));
    info.AddValue("Name", (object) this.Name);
    info.AddValue("Foreground", (object) this.Foreground);
    info.AddValue("Background", (object) this.Background);
    info.AddValue("HasUnderline", this.Underline.HasValue);
    if (this.Underline.HasValue)
      info.AddValue("Underline", this.Underline.Value);
    info.AddValue("HasWeight", this.FontWeight.HasValue);
    if (this.FontWeight.HasValue)
      info.AddValue("Weight", this.FontWeight.Value.ToOpenTypeWeight());
    info.AddValue("HasStyle", this.FontStyle.HasValue);
    if (this.FontStyle.HasValue)
      info.AddValue("Style", (object) this.FontStyle.Value.ToString());
    info.AddValue("ExampleText", (object) this.ExampleText);
  }

  public override object AcceptVisitor(IXshdVisitor visitor) => visitor.VisitColor(this);
}
