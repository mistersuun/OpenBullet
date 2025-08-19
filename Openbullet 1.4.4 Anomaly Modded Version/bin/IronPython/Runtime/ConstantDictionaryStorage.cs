// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ConstantDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace IronPython.Runtime;

[Serializable]
internal class ConstantDictionaryStorage : DictionaryStorage, IExpressionSerializable
{
  private readonly CommonDictionaryStorage _storage;

  public ConstantDictionaryStorage(CommonDictionaryStorage storage) => this._storage = storage;

  public override void Add(ref DictionaryStorage storage, object key, object value)
  {
    lock (this)
    {
      if (storage == this)
      {
        CommonDictionaryStorage into = new CommonDictionaryStorage();
        this._storage.CopyTo((DictionaryStorage) into);
        into.AddNoLock(key, value);
        storage = (DictionaryStorage) into;
        return;
      }
    }
    storage.Add(ref storage, key, value);
  }

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    if (!this._storage.Contains(key))
      return false;
    lock (this)
    {
      if (storage == this)
      {
        CommonDictionaryStorage into = new CommonDictionaryStorage();
        this._storage.CopyTo((DictionaryStorage) into);
        into.Remove(key);
        storage = (DictionaryStorage) into;
        return true;
      }
    }
    return storage.Remove(ref storage, key);
  }

  public override void Clear(ref DictionaryStorage storage)
  {
    lock (this)
    {
      if (storage == this)
      {
        storage = (DictionaryStorage) EmptyDictionaryStorage.Instance;
        return;
      }
    }
    storage.Clear(ref storage);
  }

  public override bool Contains(object key) => this._storage.Contains(key);

  public override bool TryGetValue(object key, out object value)
  {
    return this._storage.TryGetValue(key, out value);
  }

  public override int Count => this._storage.Count;

  public override List<KeyValuePair<object, object>> GetItems() => this._storage.GetItems();

  public override DictionaryStorage Clone() => this._storage.Clone();

  public override bool HasNonStringAttributes() => this._storage.HasNonStringAttributes();

  public Expression CreateExpression()
  {
    Expression[] expressionArray1 = new Expression[this.Count * 2];
    int num1 = 0;
    foreach (KeyValuePair<object, object> keyValuePair in this.GetItems())
    {
      Expression[] expressionArray2 = expressionArray1;
      int index1 = num1;
      int num2 = index1 + 1;
      Expression expression1 = Utils.Convert(Utils.Constant(keyValuePair.Value), typeof (object));
      expressionArray2[index1] = expression1;
      Expression[] expressionArray3 = expressionArray1;
      int index2 = num2;
      num1 = index2 + 1;
      Expression expression2 = Utils.Convert(Utils.Constant(keyValuePair.Key), typeof (object));
      expressionArray3[index2] = expression2;
    }
    return (Expression) Expression.Call(typeof (PythonOps).GetMethod("MakeConstantDictStorage"), (Expression) Expression.NewArrayInit(typeof (object), expressionArray1));
  }
}
