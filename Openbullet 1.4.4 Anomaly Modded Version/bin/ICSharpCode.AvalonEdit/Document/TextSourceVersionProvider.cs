// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Document.TextSourceVersionProvider
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable disable
namespace ICSharpCode.AvalonEdit.Document;

public class TextSourceVersionProvider
{
  private TextSourceVersionProvider.Version currentVersion;

  public TextSourceVersionProvider()
  {
    this.currentVersion = new TextSourceVersionProvider.Version(this);
  }

  public ITextSourceVersion CurrentVersion => (ITextSourceVersion) this.currentVersion;

  public void AppendChange(TextChangeEventArgs change)
  {
    this.currentVersion.change = change != null ? change : throw new ArgumentNullException(nameof (change));
    this.currentVersion.next = new TextSourceVersionProvider.Version(this.currentVersion);
    this.currentVersion = this.currentVersion.next;
  }

  [DebuggerDisplay("Version #{id}")]
  private sealed class Version : ITextSourceVersion
  {
    private readonly TextSourceVersionProvider provider;
    private readonly int id;
    internal TextChangeEventArgs change;
    internal TextSourceVersionProvider.Version next;

    internal Version(TextSourceVersionProvider provider) => this.provider = provider;

    internal Version(TextSourceVersionProvider.Version prev)
    {
      this.provider = prev.provider;
      this.id = prev.id + 1;
    }

    public bool BelongsToSameDocumentAs(ITextSourceVersion other)
    {
      return other is TextSourceVersionProvider.Version version && this.provider == version.provider;
    }

    public int CompareAge(ITextSourceVersion other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (!(other is TextSourceVersionProvider.Version version) || this.provider != version.provider)
        throw new ArgumentException("Versions do not belong to the same document.");
      return Math.Sign(this.id - version.id);
    }

    public IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other)
    {
      int num = this.CompareAge(other);
      TextSourceVersionProvider.Version other1 = (TextSourceVersionProvider.Version) other;
      if (num < 0)
        return this.GetForwardChanges(other1);
      return num > 0 ? other1.GetForwardChanges(this).Reverse<TextChangeEventArgs>().Select<TextChangeEventArgs, TextChangeEventArgs>((Func<TextChangeEventArgs, TextChangeEventArgs>) (change => change.Invert())) : (IEnumerable<TextChangeEventArgs>) Empty<TextChangeEventArgs>.Array;
    }

    private IEnumerable<TextChangeEventArgs> GetForwardChanges(
      TextSourceVersionProvider.Version other)
    {
      for (TextSourceVersionProvider.Version node = this; node != other; node = node.next)
        yield return node.change;
    }

    public int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement)
    {
      int offset = oldOffset;
      foreach (TextChangeEventArgs textChangeEventArgs in this.GetChangesTo(other))
        offset = textChangeEventArgs.GetNewOffset(offset, movement);
      return offset;
    }
  }
}
