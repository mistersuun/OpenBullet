// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdReference`1
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;

#nullable disable
namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public struct XshdReference<T> : IEquatable<XshdReference<T>> where T : XshdElement
{
  private string referencedDefinition;
  private string referencedElement;
  private T inlineElement;

  public string ReferencedDefinition => this.referencedDefinition;

  public string ReferencedElement => this.referencedElement;

  public T InlineElement => this.inlineElement;

  public XshdReference(string referencedDefinition, string referencedElement)
  {
    if (referencedElement == null)
      throw new ArgumentNullException(nameof (referencedElement));
    this.referencedDefinition = referencedDefinition;
    this.referencedElement = referencedElement;
    this.inlineElement = default (T);
  }

  public XshdReference(T inlineElement)
  {
    if ((object) inlineElement == null)
      throw new ArgumentNullException(nameof (inlineElement));
    this.referencedDefinition = (string) null;
    this.referencedElement = (string) null;
    this.inlineElement = inlineElement;
  }

  public object AcceptVisitor(IXshdVisitor visitor)
  {
    return (object) this.inlineElement != null ? this.inlineElement.AcceptVisitor(visitor) : (object) null;
  }

  public override bool Equals(object obj) => obj is XshdReference<T> other && this.Equals(other);

  public bool Equals(XshdReference<T> other)
  {
    return this.referencedDefinition == other.referencedDefinition && this.referencedElement == other.referencedElement && (object) this.inlineElement == (object) other.inlineElement;
  }

  public override int GetHashCode()
  {
    return XshdReference<T>.GetHashCode((object) this.referencedDefinition) ^ XshdReference<T>.GetHashCode((object) this.referencedElement) ^ XshdReference<T>.GetHashCode((object) this.inlineElement);
  }

  private static int GetHashCode(object o) => o == null ? 0 : o.GetHashCode();

  public static bool operator ==(XshdReference<T> left, XshdReference<T> right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(XshdReference<T> left, XshdReference<T> right)
  {
    return !left.Equals(right);
  }
}
