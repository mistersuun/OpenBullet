// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.CheckedDictionaryEnumerator
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Utils;

public abstract class CheckedDictionaryEnumerator : 
  IDictionaryEnumerator,
  IEnumerator,
  IEnumerator<KeyValuePair<object, object>>,
  IDisposable
{
  private CheckedDictionaryEnumerator.EnumeratorState _enumeratorState;

  private void CheckEnumeratorState()
  {
    if (this._enumeratorState == CheckedDictionaryEnumerator.EnumeratorState.NotStarted)
      throw Error.EnumerationNotStarted();
    if (this._enumeratorState == CheckedDictionaryEnumerator.EnumeratorState.Ended)
      throw Error.EnumerationFinished();
  }

  public DictionaryEntry Entry
  {
    get
    {
      this.CheckEnumeratorState();
      return new DictionaryEntry(this.Key, this.Value);
    }
  }

  public object Key
  {
    get
    {
      this.CheckEnumeratorState();
      return this.GetKey();
    }
  }

  public object Value
  {
    get
    {
      this.CheckEnumeratorState();
      return this.GetValue();
    }
  }

  public bool MoveNext()
  {
    if (this._enumeratorState == CheckedDictionaryEnumerator.EnumeratorState.Ended)
      throw Error.EnumerationFinished();
    int num = this.DoMoveNext() ? 1 : 0;
    if (num != 0)
    {
      this._enumeratorState = CheckedDictionaryEnumerator.EnumeratorState.Started;
      return num != 0;
    }
    this._enumeratorState = CheckedDictionaryEnumerator.EnumeratorState.Ended;
    return num != 0;
  }

  public object Current => (object) this.Entry;

  public void Reset()
  {
    this.DoReset();
    this._enumeratorState = CheckedDictionaryEnumerator.EnumeratorState.NotStarted;
  }

  KeyValuePair<object, object> IEnumerator<KeyValuePair<object, object>>.Current
  {
    get => new KeyValuePair<object, object>(this.Key, this.Value);
  }

  public void Dispose() => GC.SuppressFinalize((object) this);

  protected abstract object GetKey();

  protected abstract object GetValue();

  protected abstract bool DoMoveNext();

  protected abstract void DoReset();

  private enum EnumeratorState
  {
    NotStarted,
    Started,
    Ended,
  }
}
