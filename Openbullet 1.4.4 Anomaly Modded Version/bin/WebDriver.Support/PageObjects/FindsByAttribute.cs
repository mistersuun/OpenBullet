// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.PageObjects.FindsByAttribute
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.ComponentModel;

#nullable disable
namespace OpenQA.Selenium.Support.PageObjects;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class FindsByAttribute : Attribute, IComparable
{
  private By finder;

  [DefaultValue(How.Id)]
  public How How { get; set; }

  public string Using { get; set; }

  [DefaultValue(0)]
  public int Priority { get; set; }

  public Type CustomFinderType { get; set; }

  internal By Finder
  {
    get
    {
      if (this.finder == (By) null)
        this.finder = ByFactory.From(this);
      return this.finder;
    }
    set => this.finder = value;
  }

  public static bool operator ==(FindsByAttribute one, FindsByAttribute two)
  {
    if ((object) one == (object) two)
      return true;
    return (object) one != null && (object) two != null && one.Equals((object) two);
  }

  public static bool operator !=(FindsByAttribute one, FindsByAttribute two) => !(one == two);

  public static bool operator >(FindsByAttribute one, FindsByAttribute two)
  {
    if (one == (FindsByAttribute) null)
      throw new ArgumentNullException(nameof (one), "Object to compare cannot be null");
    return one.CompareTo((object) two) > 0;
  }

  public static bool operator <(FindsByAttribute one, FindsByAttribute two)
  {
    if (one == (FindsByAttribute) null)
      throw new ArgumentNullException(nameof (one), "Object to compare cannot be null");
    return one.CompareTo((object) two) < 0;
  }

  public int CompareTo(object obj)
  {
    FindsByAttribute findsByAttribute = obj != null ? obj as FindsByAttribute : throw new ArgumentNullException(nameof (obj), "Object to compare cannot be null");
    if (findsByAttribute == (FindsByAttribute) null)
      throw new ArgumentException("Object to compare must be a FindsByAttribute", nameof (obj));
    return this.Priority != findsByAttribute.Priority ? this.Priority - findsByAttribute.Priority : 0;
  }

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    FindsByAttribute findsByAttribute = obj as FindsByAttribute;
    return !(findsByAttribute == (FindsByAttribute) null) && findsByAttribute.Priority == this.Priority && !(findsByAttribute.Finder != this.Finder);
  }

  public override int GetHashCode() => this.Finder.GetHashCode();
}
