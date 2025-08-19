// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JEnumerable`1
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json.Linq;

internal struct JEnumerable<T> : 
  IJEnumerable<T>,
  IEnumerable<T>,
  IEnumerable,
  IEquatable<JEnumerable<T>>
  where T : JToken
{
  public static readonly JEnumerable<T> Empty = new JEnumerable<T>(Enumerable.Empty<T>());
  private readonly IEnumerable<T> _enumerable;

  public JEnumerable(IEnumerable<T> enumerable)
  {
    ValidationUtils.ArgumentNotNull((object) enumerable, nameof (enumerable));
    this._enumerable = enumerable;
  }

  public IEnumerator<T> GetEnumerator()
  {
    return ((IEnumerable<T>) ((object) this._enumerable ?? (object) JEnumerable<T>.Empty)).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  public IJEnumerable<JToken> this[object key]
  {
    get
    {
      return this._enumerable == null ? (IJEnumerable<JToken>) JEnumerable<JToken>.Empty : (IJEnumerable<JToken>) new JEnumerable<JToken>(this._enumerable.Values<T, JToken>(key));
    }
  }

  public bool Equals(JEnumerable<T> other)
  {
    return object.Equals((object) this._enumerable, (object) other._enumerable);
  }

  public override bool Equals(object obj) => obj is JEnumerable<T> other && this.Equals(other);

  public override int GetHashCode()
  {
    return this._enumerable == null ? 0 : this._enumerable.GetHashCode();
  }
}
