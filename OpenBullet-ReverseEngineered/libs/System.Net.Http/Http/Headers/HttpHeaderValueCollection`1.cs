// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpHeaderValueCollection`1
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace System.Net.Http.Headers;

public sealed class HttpHeaderValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
{
  private string _headerName;
  private HttpHeaders _store;
  private T _specialValue;
  private Action<HttpHeaderValueCollection<T>, T> _validator;

  public int Count => this.GetCount();

  public bool IsReadOnly => false;

  internal bool IsSpecialValueSet
  {
    get
    {
      return (object) this._specialValue != null && this._store.ContainsParsedValue(this._headerName, (object) this._specialValue);
    }
  }

  internal HttpHeaderValueCollection(string headerName, HttpHeaders store)
    : this(headerName, store, default (T), (Action<HttpHeaderValueCollection<T>, T>) null)
  {
  }

  internal HttpHeaderValueCollection(
    string headerName,
    HttpHeaders store,
    Action<HttpHeaderValueCollection<T>, T> validator)
    : this(headerName, store, default (T), validator)
  {
  }

  internal HttpHeaderValueCollection(string headerName, HttpHeaders store, T specialValue)
    : this(headerName, store, specialValue, (Action<HttpHeaderValueCollection<T>, T>) null)
  {
  }

  internal HttpHeaderValueCollection(
    string headerName,
    HttpHeaders store,
    T specialValue,
    Action<HttpHeaderValueCollection<T>, T> validator)
  {
    this._store = store;
    this._headerName = headerName;
    this._specialValue = specialValue;
    this._validator = validator;
  }

  public void Add(T item)
  {
    this.CheckValue(item);
    this._store.AddParsedValue(this._headerName, (object) item);
  }

  public void ParseAdd(string input) => this._store.Add(this._headerName, input);

  public bool TryParseAdd(string input) => this._store.TryParseAndAddValue(this._headerName, input);

  public void Clear() => this._store.Remove(this._headerName);

  public bool Contains(T item)
  {
    this.CheckValue(item);
    return this._store.ContainsParsedValue(this._headerName, (object) item);
  }

  public void CopyTo(T[] array, int arrayIndex)
  {
    if (array == null)
      throw new ArgumentNullException(nameof (array));
    if (arrayIndex < 0 || arrayIndex > array.Length)
      throw new ArgumentOutOfRangeException(nameof (arrayIndex));
    object parsedValues = this._store.GetParsedValues(this._headerName);
    if (parsedValues == null)
      return;
    if (!(parsedValues is List<object> objectList))
    {
      if (arrayIndex == array.Length)
        throw new ArgumentException(SR.net_http_copyto_array_too_small);
      array[arrayIndex] = parsedValues as T;
    }
    else
      objectList.CopyTo((object[]) array, arrayIndex);
  }

  public bool Remove(T item)
  {
    this.CheckValue(item);
    return this._store.RemoveParsedValue(this._headerName, (object) item);
  }

  public IEnumerator<T> GetEnumerator()
  {
    object parsedValues = this._store.GetParsedValues(this._headerName);
    if (parsedValues != null)
    {
      if (!(parsedValues is List<object> storeValues))
      {
        yield return parsedValues as T;
      }
      else
      {
        foreach (object obj in storeValues)
          yield return obj as T;
      }
    }
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public override string ToString() => this._store.GetHeaderString(this._headerName);

  internal string GetHeaderStringWithoutSpecial()
  {
    return !this.IsSpecialValueSet ? this.ToString() : this._store.GetHeaderString(this._headerName, (object) this._specialValue);
  }

  internal void SetSpecialValue()
  {
    if (this._store.ContainsParsedValue(this._headerName, (object) this._specialValue))
      return;
    this._store.AddParsedValue(this._headerName, (object) this._specialValue);
  }

  internal void RemoveSpecialValue()
  {
    this._store.RemoveParsedValue(this._headerName, (object) this._specialValue);
  }

  private void CheckValue(T item)
  {
    if ((object) item == null)
      throw new ArgumentNullException(nameof (item));
    if (this._validator == null)
      return;
    this._validator(this, item);
  }

  private int GetCount()
  {
    object parsedValues = this._store.GetParsedValues(this._headerName);
    if (parsedValues == null)
      return 0;
    return !(parsedValues is List<object> objectList) ? 1 : objectList.Count;
  }
}
