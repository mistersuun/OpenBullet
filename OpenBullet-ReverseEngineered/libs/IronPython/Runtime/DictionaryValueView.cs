// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DictionaryValueView
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

[PythonType("dict_values")]
public sealed class DictionaryValueView : IEnumerable, IEnumerable<object>, ICodeFormattable
{
  private readonly PythonDictionary _dict;

  internal DictionaryValueView(PythonDictionary dict) => this._dict = dict;

  [PythonHidden(new PlatformID[] {})]
  public IEnumerator GetEnumerator() => this._dict.itervalues();

  IEnumerator<object> IEnumerable<object>.GetEnumerator()
  {
    return (IEnumerator<object>) new DictionaryValueEnumerator(this._dict._storage);
  }

  public int __len__() => this._dict.Count;

  public string __repr__(CodeContext context)
  {
    StringBuilder stringBuilder = new StringBuilder(20);
    stringBuilder.Append("dict_values([");
    string str = "";
    foreach (object o in this)
    {
      stringBuilder.Append(str);
      str = ", ";
      stringBuilder.Append(PythonOps.Repr(context, o));
    }
    stringBuilder.Append("])");
    return stringBuilder.ToString();
  }
}
