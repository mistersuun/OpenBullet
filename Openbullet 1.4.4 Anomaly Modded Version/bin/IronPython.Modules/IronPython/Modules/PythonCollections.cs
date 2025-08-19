// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonCollections
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace IronPython.Modules;

public class PythonCollections
{
  public const string __doc__ = "High performance data structures\n";

  [PythonType]
  [DontMapIEnumerableToContains]
  [DebuggerDisplay("deque, {__len__()} items")]
  [DebuggerTypeProxy(typeof (CollectionDebugProxy))]
  public class deque : 
    IEnumerable,
    IComparable,
    ICodeFormattable,
    IStructuralEquatable,
    IStructuralComparable,
    ICollection,
    IReversible
  {
    private object[] _data;
    private object _lockObj = new object();
    private int _head;
    private int _tail;
    private int _itemCnt;
    private int _maxLen;
    private int _version;
    public const object __hash__ = null;

    public deque()
    {
      this._maxLen = -1;
      this.clear();
    }

    public deque(object iterable)
      : this()
    {
    }

    public deque(object iterable, object maxLen)
      : this()
    {
    }

    public deque(params object[] args)
      : this()
    {
    }

    public deque([ParamDictionary] IDictionary<object, object> dict, params object[] args)
      : this()
    {
    }

    private deque(int maxLen)
    {
      this._maxLen = maxLen;
      this.clear();
    }

    public void __init__()
    {
      this._maxLen = -1;
      this.clear();
    }

    public void __init__([ParamDictionary] IDictionary<object, object> dict)
    {
      this._maxLen = PythonCollections.deque.VerifyMaxLen(dict);
      this.clear();
    }

    public void __init__(object iterable)
    {
      this._maxLen = -1;
      this.clear();
      this.extend(iterable);
    }

    public void __init__(object iterable, object maxLen)
    {
      this._maxLen = PythonCollections.deque.VerifyMaxLenValue(maxLen);
      this.clear();
      this.extend(iterable);
    }

    public void __init__(object iterable, [ParamDictionary] IDictionary<object, object> dict)
    {
      if (PythonCollections.deque.VerifyMaxLen(dict) < 0)
        this.__init__(iterable);
      else
        this.__init__(iterable, (object) PythonCollections.deque.VerifyMaxLen(dict));
    }

    private static int VerifyMaxLen(IDictionary<object, object> dict)
    {
      if (dict.Count != 1)
        throw PythonOps.TypeError("deque() takes at most 1 keyword argument ({0} given)", (object) dict.Count);
      object obj;
      if (!dict.TryGetValue((object) "maxlen", out obj))
      {
        IEnumerator<object> enumerator = dict.Keys.GetEnumerator();
        if (enumerator.MoveNext())
          throw PythonOps.TypeError("deque(): '{0}' is an invalid keyword argument", enumerator.Current);
      }
      return PythonCollections.deque.VerifyMaxLenValue(obj);
    }

    private static int VerifyMaxLenValue(object value)
    {
      switch (value)
      {
        case null:
          return -1;
        case int _:
        case BigInteger _:
        case double _:
          int num1 = (int) value;
          return num1 >= 0 ? num1 : throw PythonOps.ValueError("maxlen must be non-negative");
        case Extensible<int> _:
          int num2 = ((Extensible<int>) value).Value;
          return num2 >= 0 ? num2 : throw PythonOps.ValueError("maxlen must be non-negative");
        default:
          throw PythonOps.TypeError("deque(): keyword argument 'maxlen' requires integer");
      }
    }

    public void append(object x)
    {
      lock (this._lockObj)
      {
        ++this._version;
        if (this._itemCnt == this._maxLen)
        {
          if (this._maxLen == 0)
            return;
          this._data[this._tail++] = x;
          if (this._tail == this._data.Length)
            this._tail = 0;
          this._head = this._tail;
        }
        else
        {
          if (this._itemCnt == this._data.Length)
            this.GrowArray();
          ++this._itemCnt;
          this._data[this._tail++] = x;
          if (this._tail != this._data.Length)
            return;
          this._tail = 0;
        }
      }
    }

    public void appendleft(object x)
    {
      lock (this._lockObj)
      {
        ++this._version;
        if (this._itemCnt == this._maxLen)
        {
          --this._head;
          if (this._head < 0)
            this._head = this._data.Length - 1;
          this._tail = this._head;
          this._data[this._head] = x;
        }
        else
        {
          if (this._itemCnt == this._data.Length)
            this.GrowArray();
          ++this._itemCnt;
          --this._head;
          if (this._head < 0)
            this._head = this._data.Length - 1;
          this._data[this._head] = x;
        }
      }
    }

    public void clear()
    {
      lock (this._lockObj)
      {
        ++this._version;
        this._head = this._tail = 0;
        this._itemCnt = 0;
        if (this._maxLen < 0)
          this._data = new object[8];
        else
          this._data = new object[Math.Min(this._maxLen, 8)];
      }
    }

    public void extend(object iterable)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
      while (enumerator.MoveNext())
        this.append(enumerator.Current);
    }

    public void extendleft(object iterable)
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
      while (enumerator.MoveNext())
        this.appendleft(enumerator.Current);
    }

    public object pop()
    {
      lock (this._lockObj)
      {
        if (this._itemCnt == 0)
          throw PythonOps.IndexError("pop from an empty deque");
        ++this._version;
        if (this._tail != 0)
          --this._tail;
        else
          this._tail = this._data.Length - 1;
        --this._itemCnt;
        object obj = this._data[this._tail];
        this._data[this._tail] = (object) null;
        return obj;
      }
    }

    public object popleft()
    {
      lock (this._lockObj)
      {
        if (this._itemCnt == 0)
          throw PythonOps.IndexError("pop from an empty deque");
        ++this._version;
        object obj = this._data[this._head];
        this._data[this._head] = (object) null;
        if (this._head != this._data.Length - 1)
          ++this._head;
        else
          this._head = 0;
        --this._itemCnt;
        return obj;
      }
    }

    public void remove(object value)
    {
      lock (this._lockObj)
      {
        int found = -1;
        int version = this._version;
        this.WalkDeque((PythonCollections.deque.DequeWalker) (index =>
        {
          if (!PythonOps.EqualRetBool(this._data[index], value))
            return true;
          found = index;
          return false;
        }));
        if (this._version != version)
          throw PythonOps.IndexError("deque mutated during remove().");
        if (found == this._head)
          this.popleft();
        else if (found == (this._tail > 0 ? this._tail - 1 : this._data.Length - 1))
        {
          this.pop();
        }
        else
        {
          if (found == -1)
            throw PythonOps.ValueError("deque.remove(value): value not in deque");
          ++this._version;
          int head = this._head < this._tail ? this._head : 0;
          bool flag = false;
          object obj1 = this._tail != 0 ? this._data[this._tail - 1] : this._data[this._data.Length - 1];
          for (int index = this._tail - 2; index >= head; --index)
          {
            object obj2 = this._data[index];
            this._data[index] = obj1;
            if (index == found)
            {
              flag = true;
              break;
            }
            obj1 = obj2;
          }
          if (this._head >= this._tail && !flag)
          {
            for (int index = this._data.Length - 1; index >= this._head; --index)
            {
              object obj3 = this._data[index];
              this._data[index] = obj1;
              if (index != found)
                obj1 = obj3;
              else
                break;
            }
          }
          --this._tail;
          --this._itemCnt;
          if (this._tail >= 0)
            return;
          this._tail = this._data.Length - 1;
        }
      }
    }

    public void rotate(CodeContext context) => this.rotate(context, (object) 1);

    public void rotate(CodeContext context, object n)
    {
      lock (this._lockObj)
      {
        if (this._itemCnt == 0)
          return;
        int num = context.LanguageContext.ConvertToInt32(n) % this._itemCnt % this._itemCnt;
        if (num == 0)
          return;
        if (num < 0)
          num += this._itemCnt;
        ++this._version;
        if (this._itemCnt == this._data.Length)
        {
          this._head = this._tail = (this._tail - num + this._data.Length) % this._data.Length;
        }
        else
        {
          object[] newData = new object[this._itemCnt];
          int curWriteIndex = num;
          this.WalkDeque((PythonCollections.deque.DequeWalker) (curIndex =>
          {
            newData[curWriteIndex] = this._data[curIndex];
            curWriteIndex = (curWriteIndex + 1) % this._itemCnt;
            return true;
          }));
          this._head = this._tail = 0;
          this._data = newData;
        }
      }
    }

    public object this[CodeContext context, object index]
    {
      get
      {
        lock (this._lockObj)
          return this._data[this.IndexToSlot(context, index)];
      }
      set
      {
        lock (this._lockObj)
        {
          ++this._version;
          this._data[this.IndexToSlot(context, index)] = value;
        }
      }
    }

    public object __copy__(CodeContext context)
    {
      if (!(this.GetType() == typeof (PythonCollections.deque)))
        return PythonCalls.Call(context, (object) DynamicHelpers.GetPythonType((object) this), (object) ((IEnumerable) this).GetEnumerator());
      PythonCollections.deque deque = new PythonCollections.deque(this._maxLen);
      deque.extend((object) ((IEnumerable) this).GetEnumerator());
      return (object) deque;
    }

    public void __delitem__(CodeContext context, object index)
    {
      lock (this._lockObj)
      {
        int realIndex = this.IndexToSlot(context, index);
        ++this._version;
        if (realIndex == this._head)
          this.popleft();
        else if (realIndex == this._tail - 1 || realIndex == this._data.Length - 1 && this._tail == this._data.Length)
        {
          this.pop();
        }
        else
        {
          object[] newData = new object[this._data.Length];
          int writeIndex = 0;
          this.WalkDeque((PythonCollections.deque.DequeWalker) (curIndex =>
          {
            if (curIndex != realIndex)
              newData[writeIndex++] = this._data[curIndex];
            return true;
          }));
          this._head = 0;
          this._tail = writeIndex;
          this._data = newData;
          --this._itemCnt;
        }
      }
    }

    public PythonTuple __reduce__()
    {
      lock (this._lockObj)
      {
        object[] items = new object[this._itemCnt];
        int curItem = 0;
        this.WalkDeque((PythonCollections.deque.DequeWalker) (curIndex =>
        {
          items[curItem++] = this._data[curIndex];
          return true;
        }));
        return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple((object) IronPython.Runtime.List.FromArrayNoCopy(items)), null);
      }
    }

    public int __len__() => this._itemCnt;

    int IComparable.CompareTo(object obj)
    {
      return obj is PythonCollections.deque otherDeque ? this.CompareToWorker(otherDeque) : throw new ValueErrorException("expected deque");
    }

    private int CompareToWorker(PythonCollections.deque otherDeque)
    {
      return this.CompareToWorker(otherDeque, (IComparer) null);
    }

    private int CompareToWorker(PythonCollections.deque otherDeque, IComparer comparer)
    {
      if (otherDeque._itemCnt == 0 && this._itemCnt == 0 || CompareUtil.Check((object) this))
        return 0;
      CompareUtil.Push((object) this);
      try
      {
        int index1 = otherDeque._head;
        int index2 = this._head;
        do
        {
          int worker = comparer != null ? comparer.Compare(this._data[index2], otherDeque._data[index1]) : PythonOps.Compare(this._data[index2], otherDeque._data[index1]);
          if (worker != 0)
            return worker;
          ++index1;
          if (index1 == otherDeque._data.Length)
            index1 = 0;
          if (index1 != otherDeque._tail)
          {
            ++index2;
            if (index2 == this._data.Length)
              index2 = 0;
          }
          else
            break;
        }
        while (index2 != this._tail);
        return otherDeque._itemCnt == this._itemCnt ? 0 : (this._itemCnt > otherDeque._itemCnt ? 1 : -1);
      }
      finally
      {
        CompareUtil.Pop((object) this);
      }
    }

    int IStructuralComparable.CompareTo(object other, IComparer comparer)
    {
      return other is PythonCollections.deque otherDeque ? this.CompareToWorker(otherDeque, comparer) : throw new ValueErrorException("expected deque");
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new PythonCollections.deque.DequeIterator(this);
    }

    public virtual IEnumerator __reversed__()
    {
      return (IEnumerator) new PythonCollections.deque.deque_reverse_iterator(this);
    }

    private void GrowArray()
    {
      if (this._data.Length == this._maxLen)
        return;
      object[] destinationArray = this._maxLen >= 0 ? new object[Math.Min(this._maxLen, this._data.Length * 2)] : new object[this._data.Length * 2];
      int num;
      int length;
      if (this._head >= this._tail)
      {
        num = this._data.Length - this._head;
        length = this._data.Length - num;
      }
      else
      {
        num = this._tail - this._head;
        length = this._data.Length - num;
      }
      Array.Copy((Array) this._data, this._head, (Array) destinationArray, 0, num);
      Array.Copy((Array) this._data, 0, (Array) destinationArray, num, length);
      this._head = 0;
      this._tail = this._data.Length;
      this._data = destinationArray;
    }

    private int IndexToSlot(CodeContext context, object index)
    {
      if (this._itemCnt == 0)
        throw PythonOps.IndexError("deque index out of range");
      int int32 = context.LanguageContext.ConvertToInt32(index);
      if (int32 >= 0)
      {
        if (int32 >= this._itemCnt)
          throw PythonOps.IndexError("deque index out of range");
        int slot = this._head + int32;
        if (slot >= this._data.Length)
          slot -= this._data.Length;
        return slot;
      }
      if (int32 * -1 > this._itemCnt)
        throw PythonOps.IndexError("deque index out of range");
      int slot1 = this._tail + int32;
      if (slot1 < 0)
        slot1 += this._data.Length;
      return slot1;
    }

    private void WalkDeque(PythonCollections.deque.DequeWalker walker)
    {
      if (this._itemCnt == 0)
        return;
      int num = this._head < this._tail ? this._tail : this._data.Length;
      for (int head = this._head; head < num; ++head)
      {
        if (!walker(head))
          return;
      }
      if (this._head < this._tail)
        return;
      int curIndex = 0;
      while (curIndex < this._tail && walker(curIndex))
        ++curIndex;
    }

    public virtual string __repr__(CodeContext context)
    {
      List<object> andCheckInfinite = PythonOps.GetAndCheckInfinite((object) this);
      if (andCheckInfinite == null)
        return "[...]";
      int count = andCheckInfinite.Count;
      andCheckInfinite.Add((object) this);
      try
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("deque([");
        string comma = "";
        lock (this._lockObj)
          this.WalkDeque((PythonCollections.deque.DequeWalker) (index =>
          {
            sb.Append(comma);
            sb.Append(PythonOps.Repr(context, this._data[index]));
            comma = ", ";
            return true;
          }));
        if (this._maxLen < 0)
        {
          sb.Append("])");
        }
        else
        {
          sb.Append("], maxlen=");
          sb.Append(this._maxLen);
          sb.Append(')');
        }
        return sb.ToString();
      }
      finally
      {
        andCheckInfinite.RemoveAt(count);
      }
    }

    int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    {
      if (CompareUtil.Check((object) this))
        return 0;
      CompareUtil.Push((object) this);
      try
      {
        return ((IStructuralEquatable) new PythonTuple((object) this)).GetHashCode(comparer);
      }
      finally
      {
        CompareUtil.Pop((object) this);
      }
    }

    bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
    {
      return other is PythonCollections.deque && this.EqualsWorker((PythonCollections.deque) other, comparer);
    }

    private bool EqualsWorker(PythonCollections.deque other)
    {
      return this.EqualsWorker(other, (IEqualityComparer) null);
    }

    private bool EqualsWorker(PythonCollections.deque otherDeque, IEqualityComparer comparer)
    {
      if (otherDeque._itemCnt != this._itemCnt)
        return false;
      if (otherDeque._itemCnt == 0 || CompareUtil.Check((object) this))
        return true;
      CompareUtil.Push((object) this);
      try
      {
        int index1 = otherDeque._head;
        int index2 = this._head;
        while (index2 != this._tail)
        {
          if (!(comparer != null ? comparer.Equals(this._data[index2], otherDeque._data[index1]) : PythonOps.EqualRetBool(this._data[index2], otherDeque._data[index1])))
            return false;
          ++index1;
          if (index1 == otherDeque._data.Length)
            index1 = 0;
          ++index2;
          if (index2 == this._data.Length)
            index2 = 0;
        }
        return true;
      }
      finally
      {
        CompareUtil.Pop((object) this);
      }
    }

    public static object operator >(PythonCollections.deque self, object other)
    {
      return !(other is PythonCollections.deque otherDeque) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareToWorker(otherDeque) > 0);
    }

    public static object operator <(PythonCollections.deque self, object other)
    {
      return !(other is PythonCollections.deque otherDeque) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareToWorker(otherDeque) < 0);
    }

    public static object operator >=(PythonCollections.deque self, object other)
    {
      return !(other is PythonCollections.deque otherDeque) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareToWorker(otherDeque) >= 0);
    }

    public static object operator <=(PythonCollections.deque self, object other)
    {
      return !(other is PythonCollections.deque otherDeque) ? (object) NotImplementedType.Value : ScriptingRuntimeHelpers.BooleanToObject(self.CompareToWorker(otherDeque) <= 0);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      int num = 0;
      foreach (object obj in (IEnumerable) this)
        array.SetValue(obj, index + num++);
    }

    int ICollection.Count => this._itemCnt;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => (object) this;

    [PythonType("deque_iterator")]
    private sealed class DequeIterator : IEnumerable, IEnumerator
    {
      private readonly PythonCollections.deque _deque;
      private int _curIndex;
      private int _moveCnt;
      private int _version;

      public DequeIterator(PythonCollections.deque d)
      {
        lock (d._lockObj)
        {
          this._deque = d;
          this._curIndex = d._head - 1;
          this._version = d._version;
        }
      }

      object IEnumerator.Current => this._deque._data[this._curIndex];

      bool IEnumerator.MoveNext()
      {
        lock (this._deque._lockObj)
        {
          if (this._version != this._deque._version)
            throw PythonOps.RuntimeError("deque mutated during iteration");
          if (this._moveCnt >= this._deque._itemCnt)
            return false;
          ++this._curIndex;
          ++this._moveCnt;
          if (this._curIndex == this._deque._data.Length)
            this._curIndex = 0;
          return true;
        }
      }

      void IEnumerator.Reset()
      {
        this._moveCnt = 0;
        this._curIndex = this._deque._head - 1;
      }

      public IEnumerator GetEnumerator() => (IEnumerator) this;
    }

    [PythonType]
    private class deque_reverse_iterator : IEnumerator
    {
      private readonly PythonCollections.deque _deque;
      private int _curIndex;
      private int _moveCnt;
      private int _version;

      public deque_reverse_iterator(PythonCollections.deque d)
      {
        lock (d._lockObj)
        {
          this._deque = d;
          this._curIndex = d._tail;
          this._version = d._version;
        }
      }

      object IEnumerator.Current => this._deque._data[this._curIndex];

      bool IEnumerator.MoveNext()
      {
        lock (this._deque._lockObj)
        {
          if (this._version != this._deque._version)
            throw PythonOps.RuntimeError("deque mutated during iteration");
          if (this._moveCnt >= this._deque._itemCnt)
            return false;
          --this._curIndex;
          ++this._moveCnt;
          if (this._curIndex < 0)
            this._curIndex = this._deque._data.Length - 1;
          return true;
        }
      }

      void IEnumerator.Reset()
      {
        this._moveCnt = 0;
        this._curIndex = this._deque._tail;
      }
    }

    private delegate bool DequeWalker(int curIndex);
  }

  [PythonType]
  public class defaultdict : PythonDictionary
  {
    private object _factory;
    private CallSite<Func<CallSite, CodeContext, object, object>> _missingSite;

    public defaultdict(CodeContext context)
    {
      this._missingSite = CallSite<Func<CallSite, CodeContext, object, object>>.Create((CallSiteBinder) new PythonInvokeBinder(context.LanguageContext, new CallSignature(0)));
    }

    public void __init__(object default_factory) => this._factory = default_factory;

    public void __init__(CodeContext context, object default_factory, params object[] args)
    {
      this._factory = default_factory;
      foreach (object otherø in args)
        this.update(context, otherø);
    }

    public void __init__(
      CodeContext context,
      object default_factory,
      [ParamDictionary] IDictionary<object, object> dict,
      params object[] args)
    {
      this.__init__(context, default_factory, args);
      foreach (KeyValuePair<object, object> keyValuePair in (IEnumerable<KeyValuePair<object, object>>) dict)
        this[keyValuePair.Key] = keyValuePair.Value;
    }

    public object default_factory
    {
      get => this._factory;
      set => this._factory = value;
    }

    public object __missing__(CodeContext context, object key)
    {
      object factory = this._factory;
      if (factory == null)
        throw PythonOps.KeyError(key);
      return this[key] = this._missingSite.Target((CallSite) this._missingSite, context, factory);
    }

    public object __copy__(CodeContext context) => (object) this.copy(context);

    public override PythonDictionary copy(CodeContext context)
    {
      PythonCollections.defaultdict defaultdict = new PythonCollections.defaultdict(context);
      defaultdict.default_factory = this.default_factory;
      defaultdict.update(context, (IDictionary<object, object>) this);
      return (PythonDictionary) defaultdict;
    }

    public override string __repr__(CodeContext context)
    {
      return $"defaultdict({PythonOps.Repr(context, this.default_factory)}, {base.__repr__(context)})";
    }

    public PythonTuple __reduce__()
    {
      return PythonTuple.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) PythonTuple.MakeTuple(this.default_factory), null, null, (object) this.iteritems());
    }
  }
}
