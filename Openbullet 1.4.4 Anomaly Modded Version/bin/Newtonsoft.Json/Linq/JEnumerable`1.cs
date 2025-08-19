// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JEnumerable`1
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Newtonsoft.Json.Linq;

public readonly struct JEnumerable<T> : 
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
