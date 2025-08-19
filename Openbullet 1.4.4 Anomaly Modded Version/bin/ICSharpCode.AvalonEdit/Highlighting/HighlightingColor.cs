// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.HighlightingColor
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Media;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public class HighlightingColor : ISerializable, IFreezable, ICloneable, IEquatable<HighlightingColor>
{
  internal static readonly HighlightingColor Empty = FreezableHelper.FreezeAndReturn<HighlightingColor>(new HighlightingColor());
  private string name;
  private System.Windows.FontWeight? fontWeight;
  private System.Windows.FontStyle? fontStyle;
  private bool? underline;
  private HighlightingBrush foreground;
  private HighlightingBrush background;
  private bool frozen;

  public string Name
  {
    get => this.name;
    set
    {
      if (this.frozen)
        throw new InvalidOperationException();
      this.name = value;
    }
  }

  public System.Windows.FontWeight? FontWeight
  {
    get => this.fontWeight;
    set
    {
      if (this.frozen)
        throw new InvalidOperationException();
      this.fontWeight = value;
    }
  }

  public System.Windows.FontStyle? FontStyle
  {
    get => this.fontStyle;
    set
    {
      if (this.frozen)
        throw new InvalidOperationException();
      this.fontStyle = value;
    }
  }

  public bool? Underline
  {
    get => this.underline;
    set
    {
      if (this.frozen)
        throw new InvalidOperationException();
      this.underline = value;
    }
  }

  public HighlightingBrush Foreground
  {
    get => this.foreground;
    set
    {
      if (this.frozen)
        throw new InvalidOperationException();
      this.foreground = value;
    }
  }

  public HighlightingBrush Background
  {
    get => this.background;
    set
    {
      if (this.frozen)
        throw new InvalidOperationException();
      this.background = value;
    }
  }

  public HighlightingColor()
  {
  }

  protected HighlightingColor(SerializationInfo info, StreamingContext context)
  {
    this.Name = info != null ? info.GetString(nameof (Name)) : throw new ArgumentNullException(nameof (info));
    if (info.GetBoolean("HasWeight"))
      this.FontWeight = new System.Windows.FontWeight?(System.Windows.FontWeight.FromOpenTypeWeight(info.GetInt32("Weight")));
    if (info.GetBoolean("HasStyle"))
      this.FontStyle = (System.Windows.FontStyle?) new FontStyleConverter().ConvertFromInvariantString(info.GetString("Style"));
    if (info.GetBoolean("HasUnderline"))
      this.Underline = new bool?(info.GetBoolean(nameof (Underline)));
    this.Foreground = (HighlightingBrush) info.GetValue(nameof (Foreground), typeof (HighlightingBrush));
    this.Background = (HighlightingBrush) info.GetValue(nameof (Background), typeof (HighlightingBrush));
  }

  [SecurityCritical]
  public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    if (info == null)
      throw new ArgumentNullException(nameof (info));
    info.AddValue("Name", (object) this.Name);
    info.AddValue("HasWeight", this.FontWeight.HasValue);
    if (this.FontWeight.HasValue)
      info.AddValue("Weight", this.FontWeight.Value.ToOpenTypeWeight());
    info.AddValue("HasStyle", this.FontStyle.HasValue);
    if (this.FontStyle.HasValue)
      info.AddValue("Style", (object) this.FontStyle.Value.ToString());
    info.AddValue("HasUnderline", this.Underline.HasValue);
    if (this.Underline.HasValue)
      info.AddValue("Underline", this.Underline.Value);
    info.AddValue("Foreground", (object) this.Foreground);
    info.AddValue("Background", (object) this.Background);
  }

  public virtual string ToCss()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this.Foreground != null)
    {
      Color? color = this.Foreground.GetColor((ITextRunConstructionContext) null);
      if (color.HasValue)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "color: #{0:x2}{1:x2}{2:x2}; ", (object) color.Value.R, (object) color.Value.G, (object) color.Value.B);
    }
    if (this.FontWeight.HasValue)
    {
      stringBuilder.Append("font-weight: ");
      stringBuilder.Append(this.FontWeight.Value.ToString().ToLowerInvariant());
      stringBuilder.Append("; ");
    }
    if (this.FontStyle.HasValue)
    {
      stringBuilder.Append("font-style: ");
      stringBuilder.Append(this.FontStyle.Value.ToString().ToLowerInvariant());
      stringBuilder.Append("; ");
    }
    if (this.Underline.HasValue)
    {
      stringBuilder.Append("text-decoration: ");
      stringBuilder.Append(this.Underline.Value ? "underline" : "none");
      stringBuilder.Append("; ");
    }
    return stringBuilder.ToString();
  }

  public override string ToString()
  {
    return $"[{this.GetType().Name} {(string.IsNullOrEmpty(this.Name) ? this.ToCss() : this.Name)}]";
  }

  public virtual void Freeze() => this.frozen = true;

  public bool IsFrozen => this.frozen;

  public virtual HighlightingColor Clone()
  {
    HighlightingColor highlightingColor = (HighlightingColor) this.MemberwiseClone();
    highlightingColor.frozen = false;
    return highlightingColor;
  }

  object ICloneable.Clone() => (object) this.Clone();

  public sealed override bool Equals(object obj) => this.Equals(obj as HighlightingColor);

  public virtual bool Equals(HighlightingColor other)
  {
    if (other == null || !(this.name == other.name))
      return false;
    System.Windows.FontWeight? fontWeight1 = this.fontWeight;
    System.Windows.FontWeight? fontWeight2 = other.fontWeight;
    if ((fontWeight1.HasValue != fontWeight2.HasValue ? 0 : (!fontWeight1.HasValue ? 1 : (fontWeight1.GetValueOrDefault() == fontWeight2.GetValueOrDefault() ? 1 : 0))) != 0)
    {
      System.Windows.FontStyle? fontStyle1 = this.fontStyle;
      System.Windows.FontStyle? fontStyle2 = other.fontStyle;
      if ((fontStyle1.HasValue != fontStyle2.HasValue ? 0 : (!fontStyle1.HasValue ? 1 : (fontStyle1.GetValueOrDefault() == fontStyle2.GetValueOrDefault() ? 1 : 0))) != 0)
      {
        bool? underline1 = this.underline;
        bool? underline2 = other.underline;
        if ((underline1.GetValueOrDefault() != underline2.GetValueOrDefault() ? 0 : (underline1.HasValue == underline2.HasValue ? 1 : 0)) != 0 && object.Equals((object) this.foreground, (object) other.foreground))
          return object.Equals((object) this.background, (object) other.background);
      }
    }
    return false;
  }

  public override int GetHashCode()
  {
    int num = 0;
    if (this.name != null)
      num += 1000000007 * this.name.GetHashCode();
    int hashCode = num + 1000000009 * this.fontWeight.GetHashCode() + 1000000021 * this.fontStyle.GetHashCode();
    if (this.foreground != null)
      hashCode += 1000000033 * this.foreground.GetHashCode();
    if (this.background != null)
      hashCode += 1000000087 * this.background.GetHashCode();
    return hashCode;
  }

  public void MergeWith(HighlightingColor color)
  {
    FreezableHelper.ThrowIfFrozen((IFreezable) this);
    if (color.fontWeight.HasValue)
      this.fontWeight = color.fontWeight;
    if (color.fontStyle.HasValue)
      this.fontStyle = color.fontStyle;
    if (color.foreground != null)
      this.foreground = color.foreground;
    if (color.background != null)
      this.background = color.background;
    if (!color.underline.HasValue)
      return;
    this.underline = color.underline;
  }

  internal bool IsEmptyForMerge
  {
    get
    {
      return !this.fontWeight.HasValue && !this.fontStyle.HasValue && !this.underline.HasValue && this.foreground == null && this.background == null;
    }
  }
}
