// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ObjectCollection`1
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace System.Net.Http.Headers;

internal sealed class ObjectCollection<T> : Collection<T> where T : class
{
  private static readonly Action<T> s_defaultValidator = new Action<T>(ObjectCollection<T>.CheckNotNull);
  private readonly Action<T> _validator;

  public ObjectCollection()
    : this(ObjectCollection<T>.s_defaultValidator)
  {
  }

  public ObjectCollection(Action<T> validator)
    : base((IList<T>) new List<T>())
  {
    this._validator = validator;
  }

  public List<T>.Enumerator GetEnumerator() => ((List<T>) this.Items).GetEnumerator();

  protected override void InsertItem(int index, T item)
  {
    if (this._validator != null)
      this._validator(item);
    base.InsertItem(index, item);
  }

  protected override void SetItem(int index, T item)
  {
    if (this._validator != null)
      this._validator(item);
    base.SetItem(index, item);
  }

  private static void CheckNotNull(T item)
  {
    if ((object) item == null)
      throw new ArgumentNullException(nameof (item));
  }
}
