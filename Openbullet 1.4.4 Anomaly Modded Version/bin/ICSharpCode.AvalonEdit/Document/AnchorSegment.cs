// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.AnchorSegment
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Globalization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public sealed class AnchorSegment : ISegment
{
  private readonly TextAnchor start;
  private readonly TextAnchor end;

  public int Offset => this.start.Offset;

  public int Length => Math.Max(0, this.end.Offset - this.start.Offset);

  public int EndOffset => Math.Max(this.start.Offset, this.end.Offset);

  public AnchorSegment(TextAnchor start, TextAnchor end)
  {
    if (start == null)
      throw new ArgumentNullException(nameof (start));
    if (end == null)
      throw new ArgumentNullException(nameof (end));
    if (!start.SurviveDeletion)
      throw new ArgumentException("Anchors for AnchorSegment must use SurviveDeletion", nameof (start));
    if (!end.SurviveDeletion)
      throw new ArgumentException("Anchors for AnchorSegment must use SurviveDeletion", nameof (end));
    this.start = start;
    this.end = end;
  }

  public AnchorSegment(TextDocument document, ISegment segment)
    : this(document, ThrowUtil.CheckNotNull<ISegment>(segment, nameof (segment)).Offset, segment.Length)
  {
  }

  public AnchorSegment(TextDocument document, int offset, int length)
  {
    this.start = document != null ? document.CreateAnchor(offset) : throw new ArgumentNullException(nameof (document));
    this.start.SurviveDeletion = true;
    this.start.MovementType = AnchorMovementType.AfterInsertion;
    this.end = document.CreateAnchor(offset + length);
    this.end.SurviveDeletion = true;
    this.end.MovementType = AnchorMovementType.BeforeInsertion;
  }

  public override string ToString()
  {
    return $"[Offset={this.Offset.ToString((IFormatProvider) CultureInfo.InvariantCulture)}, EndOffset={this.EndOffset.ToString((IFormatProvider) CultureInfo.InvariantCulture)}]";
  }
}
