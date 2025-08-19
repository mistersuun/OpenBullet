// Decompiled with JetBrains decompiler
// Type: LiteDB.LazyLoad`1
// Assembly: LiteDB, Version=4.1.4.0, Culture=neutral, PublicKeyToken=4ee40123013c9f27
// MVID: 0E7FE05A-5928-4996-8CFE-23DFC2A26585
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\LiteDB.dll

using System;

#nullable disable
namespace LiteDB;

public class LazyLoad<T> where T : class
{
  private readonly object _locker = new object();
  private readonly Func<T> _createValue;
  private bool _isValueCreated;
  private T _value;

  public T Value
  {
    get
    {
      if (!this._isValueCreated)
      {
        lock (this._locker)
        {
          if (!this._isValueCreated)
          {
            this._value = this._createValue();
            this._isValueCreated = true;
          }
        }
      }
      return this._value;
    }
  }

  public bool IsValueCreated
  {
    get
    {
      lock (this._locker)
        return this._isValueCreated;
    }
  }

  public LazyLoad(Func<T> createValue)
  {
    this._createValue = createValue != null ? createValue : throw new ArgumentNullException(nameof (createValue));
  }

  public override string ToString() => this.Value.ToString();
}
