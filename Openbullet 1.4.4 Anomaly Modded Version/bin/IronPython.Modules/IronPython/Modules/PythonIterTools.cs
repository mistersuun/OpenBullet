// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonIterTools
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Modules;

public static class PythonIterTools
{
  public const string __doc__ = "Provides functions and classes for working with iterable objects.";

  public static object tee(object iterable) => PythonIterTools.tee(iterable, 2);

  public static object tee(object iterable, int n)
  {
    object[] objArray = n >= 0 ? new object[n] : throw PythonOps.ValueError("n cannot be negative");
    if (!(iterable is PythonIterTools.TeeIterator))
    {
      IEnumerator enumerator = PythonOps.GetEnumerator(iterable);
      IronPython.Runtime.List dataList = new IronPython.Runtime.List();
      for (int index = 0; index < n; ++index)
        objArray[index] = (object) new PythonIterTools.TeeIterator(enumerator, dataList);
    }
    else if (n != 0)
    {
      PythonIterTools.TeeIterator teeIterator = iterable as PythonIterTools.TeeIterator;
      objArray[0] = (object) teeIterator;
      for (int index = 1; index < n; ++index)
        objArray[1] = (object) new PythonIterTools.TeeIterator(teeIterator._iter, teeIterator._data);
    }
    return (object) PythonTuple.MakeTuple(objArray);
  }

  private static Exception UnexpectedKeywordArgument(IDictionary<object, object> paramDict)
  {
    using (IEnumerator<object> enumerator = paramDict.Keys.GetEnumerator())
    {
      if (enumerator.MoveNext())
        return PythonOps.TypeError("got unexpected keyword argument {0}", enumerator.Current);
    }
    throw new InvalidOperationException();
  }

  private static int GetR(object r, IronPython.Runtime.List data)
  {
    int r1;
    if (r != null)
    {
      r1 = Converter.ConvertToInt32(r);
      if (r1 < 0)
        throw PythonOps.ValueError("r cannot be negative");
    }
    else
      r1 = data.Count;
    return r1;
  }

  private static bool MoveNextHelper(IEnumerator move)
  {
    try
    {
      return move.MoveNext();
    }
    catch (IndexOutOfRangeException ex)
    {
      return false;
    }
    catch (StopIterationException ex)
    {
      return false;
    }
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class IterBase : IEnumerator
  {
    private IEnumerator _inner;

    internal IEnumerator InnerEnumerator
    {
      set => this._inner = value;
    }

    object IEnumerator.Current => this._inner.Current;

    bool IEnumerator.MoveNext() => this._inner.MoveNext();

    void IEnumerator.Reset() => this._inner.Reset();

    public object __iter__() => (object) this;
  }

  [PythonType]
  public class chain : PythonIterTools.IterBase
  {
    private chain()
    {
    }

    public chain(params object[] iterables)
    {
      this.InnerEnumerator = (IEnumerator) PythonIterTools.chain.LazyYielder((object) iterables);
    }

    [ClassMethod]
    public static PythonIterTools.chain from_iterable(
      CodeContext context,
      PythonType cls,
      object iterables)
    {
      PythonIterTools.chain chain;
      if (cls == DynamicHelpers.GetPythonTypeFromType(typeof (PythonIterTools.chain)))
      {
        chain = new PythonIterTools.chain();
        chain.InnerEnumerator = (IEnumerator) PythonIterTools.chain.LazyYielder(iterables);
      }
      else
      {
        chain = (PythonIterTools.chain) cls.CreateInstance(context);
        chain.InnerEnumerator = (IEnumerator) PythonIterTools.chain.LazyYielder(iterables);
      }
      return chain;
    }

    private static IEnumerator<object> LazyYielder(object iterables)
    {
      IEnumerator ie = PythonOps.GetEnumerator(iterables);
      while (ie.MoveNext())
      {
        IEnumerator inner = PythonOps.GetEnumerator(ie.Current);
        while (inner.MoveNext())
          yield return inner.Current;
        inner = (IEnumerator) null;
      }
    }
  }

  [PythonType]
  public class compress : PythonIterTools.IterBase
  {
    private compress()
    {
    }

    public compress(CodeContext context, [NotNull] object data, [NotNull] object selectors)
    {
      PythonIterTools.compress.EnsureIterator(context, data);
      PythonIterTools.compress.EnsureIterator(context, selectors);
      this.InnerEnumerator = (IEnumerator) PythonIterTools.compress.LazyYielder(data, selectors);
    }

    private static void EnsureIterator(CodeContext context, object iter)
    {
      switch (iter)
      {
        case IEnumerable _:
          break;
        case IEnumerator _:
          break;
        case IEnumerable<object> _:
          break;
        case IEnumerator<object> _:
          break;
        case null:
          if (iter is OldInstance)
            throw PythonOps.TypeError("iteration over non-sequence");
          throw PythonOps.TypeError("'{0}' object is not iterable", (object) PythonTypeOps.GetName(iter));
        default:
          if (PythonOps.HasAttr(context, iter, "__iter__") || PythonOps.HasAttr(context, iter, "__getitem__"))
            break;
          goto case null;
      }
    }

    private static IEnumerator<object> LazyYielder(object data, object selectors)
    {
      IEnumerator de = PythonOps.GetEnumerator(data);
      IEnumerator se = PythonOps.GetEnumerator(selectors);
      while (de.MoveNext() && se.MoveNext())
      {
        if (PythonOps.IsTrue(se.Current))
          yield return de.Current;
      }
    }
  }

  [PythonType]
  public class count : PythonIterTools.IterBase, ICodeFormattable
  {
    private int _curInt;
    private object _step;
    private object _cur;

    public count()
    {
      this._curInt = 0;
      this._step = (object) 1;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.IntYielder(this, 0, 1);
    }

    public count(int start)
    {
      this._curInt = start;
      this._step = (object) 1;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.IntYielder(this, start, 1);
    }

    public count(BigInteger start)
    {
      this._cur = (object) start;
      this._step = (object) 1;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.BigIntYielder(this, start, 1);
    }

    public count(int start = 0, int step = 1)
    {
      this._curInt = start;
      this._step = (object) step;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.IntYielder(this, start, step);
    }

    public count(int start = 0, BigInteger step)
    {
      this._curInt = start;
      this._step = (object) step;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.IntYielder(this, start, step);
    }

    public count(BigInteger start, int step)
    {
      this._cur = (object) start;
      this._step = (object) step;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.BigIntYielder(this, start, step);
    }

    public count(BigInteger start, BigInteger step)
    {
      this._cur = (object) start;
      this._step = (object) step;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.BigIntYielder(this, start, step);
    }

    public count(CodeContext context, object start = 0, object step = 1)
    {
      PythonIterTools.count.EnsureNumeric(context, start);
      PythonIterTools.count.EnsureNumeric(context, step);
      this._cur = start;
      this._step = step;
      this.InnerEnumerator = (IEnumerator) PythonIterTools.count.ObjectYielder(context.LanguageContext, this, start, step);
    }

    private static void EnsureNumeric(CodeContext context, object num)
    {
      switch (num)
      {
        case int _:
          break;
        case double _:
          break;
        case BigInteger _:
          break;
        case Complex _:
          break;
        case null:
          throw PythonOps.TypeError("a number is required");
        default:
          if (PythonOps.HasAttr(context, num, "__int__") || PythonOps.HasAttr(context, num, "__float__"))
            break;
          goto case null;
      }
    }

    private static IEnumerator<object> IntYielder(PythonIterTools.count c, int start, int step)
    {
      int curInt;
      while (true)
      {
        curInt = c._curInt;
        try
        {
          checked { start += step; }
        }
        catch (OverflowException ex)
        {
          break;
        }
        c._curInt = start;
        yield return (object) curInt;
      }
      BigInteger startBig = (BigInteger) start + (BigInteger) step;
      c._cur = (object) startBig;
      yield return (object) curInt;
      startBig += (BigInteger) step;
      while (true)
      {
        object cur = c._cur;
        c._cur = (object) startBig;
        yield return cur;
        startBig += (BigInteger) step;
      }
    }

    private static IEnumerator<object> IntYielder(
      PythonIterTools.count c,
      int start,
      BigInteger step)
    {
      BigInteger startBig = (BigInteger) start + step;
      c._cur = (object) startBig;
      yield return (object) start;
      startBig += step;
      while (true)
      {
        object cur = c._cur;
        c._cur = (object) startBig;
        yield return cur;
        startBig += step;
      }
    }

    private static IEnumerator<BigInteger> BigIntYielder(
      PythonIterTools.count c,
      BigInteger start,
      int step)
    {
      start += (BigInteger) step;
      while (true)
      {
        BigInteger cur = (BigInteger) c._cur;
        c._cur = (object) start;
        yield return cur;
        start += (BigInteger) step;
      }
    }

    private static IEnumerator<BigInteger> BigIntYielder(
      PythonIterTools.count c,
      BigInteger start,
      BigInteger step)
    {
      start += step;
      while (true)
      {
        BigInteger cur = (BigInteger) c._cur;
        c._cur = (object) start;
        yield return cur;
        start += step;
      }
    }

    private static IEnumerator<object> ObjectYielder(
      PythonContext context,
      PythonIterTools.count c,
      object start,
      object step)
    {
      start = context.Operation(PythonOperationKind.Add, start, step);
      while (true)
      {
        object cur = c._cur;
        c._cur = start;
        yield return cur;
        start = context.Operation(PythonOperationKind.Add, start, step);
      }
    }

    public PythonTuple __reduce__()
    {
      PythonTuple pythonTuple;
      if (this.StepIsOne())
        pythonTuple = PythonOps.MakeTuple(this._cur == null ? (object) this._curInt : this._cur);
      else
        pythonTuple = PythonOps.MakeTuple(this._cur == null ? (object) this._curInt : this._cur, this._step);
      return PythonOps.MakeTuple((object) DynamicHelpers.GetPythonType((object) this), (object) pythonTuple);
    }

    public PythonTuple __reduce_ex__([Optional] int protocol) => this.__reduce__();

    private bool StepIsOne()
    {
      if (this._step is int)
        return (int) this._step == 1;
      return this._step is Extensible<int> step && step.Value == 1;
    }

    public string __repr__(CodeContext context)
    {
      object o = this._cur == null ? (object) this._curInt : this._cur;
      return this.StepIsOne() ? $"count({PythonOps.Repr(context, o)})" : $"count({PythonOps.Repr(context, o)}, {PythonOps.Repr(context, this._step)})";
    }
  }

  [PythonType]
  public class cycle : PythonIterTools.IterBase
  {
    public cycle(object iterable)
    {
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonOps.GetEnumerator(iterable));
    }

    private IEnumerator<object> Yielder(IEnumerator iter)
    {
      IronPython.Runtime.List result = new IronPython.Runtime.List();
      while (PythonIterTools.MoveNextHelper(iter))
      {
        result.AddNoLock(iter.Current);
        yield return iter.Current;
      }
      if (result.__len__() != 0)
      {
label_4:
        for (int i = 0; i < result.__len__(); ++i)
          yield return result[i];
        goto label_4;
      }
    }
  }

  [PythonType]
  public class dropwhile : PythonIterTools.IterBase
  {
    private readonly CodeContext _context;

    public dropwhile(CodeContext context, object predicate, object iterable)
    {
      this._context = context;
      this.InnerEnumerator = (IEnumerator) this.Yielder(predicate, PythonOps.GetEnumerator(iterable));
    }

    private IEnumerator<object> Yielder(object predicate, IEnumerator iter)
    {
      PythonContext languageContext = this._context.LanguageContext;
      while (PythonIterTools.MoveNextHelper(iter))
      {
        if (!Converter.ConvertToBoolean(languageContext.CallSplat(predicate, iter.Current)))
        {
          yield return iter.Current;
          break;
        }
      }
      while (PythonIterTools.MoveNextHelper(iter))
        yield return iter.Current;
    }
  }

  [PythonType]
  public class groupby : PythonIterTools.IterBase
  {
    private static readonly object _starterKey = new object();
    private bool _fFinished;
    private object _key;
    private readonly CodeContext _context;

    public groupby(CodeContext context, object iterable)
    {
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonOps.GetEnumerator(iterable));
      this._context = context;
    }

    public groupby(CodeContext context, object iterable, object key)
    {
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonOps.GetEnumerator(iterable));
      this._context = context;
      if (key == null)
        return;
      this._key = key;
    }

    private IEnumerator<object> Yielder(IEnumerator iter)
    {
      object curKey = PythonIterTools.groupby._starterKey;
      if (PythonIterTools.MoveNextHelper(iter))
      {
        while (!this._fFinished)
        {
          while (PythonContext.Equal(this.GetKey(iter.Current), curKey))
          {
            if (!PythonIterTools.MoveNextHelper(iter))
            {
              this._fFinished = true;
              yield break;
            }
          }
          curKey = this.GetKey(iter.Current);
          yield return (object) PythonTuple.MakeTuple(curKey, (object) this.Grouper(iter, curKey));
        }
      }
    }

    private IEnumerator<object> Grouper(IEnumerator iter, object curKey)
    {
      while (PythonContext.Equal(this.GetKey(iter.Current), curKey))
      {
        yield return iter.Current;
        if (!PythonIterTools.MoveNextHelper(iter))
        {
          this._fFinished = true;
          break;
        }
      }
    }

    private object GetKey(object val)
    {
      if (this._key == null)
        return val;
      return this._context.LanguageContext.CallSplat(this._key, val);
    }
  }

  [PythonType]
  public class ifilter : PythonIterTools.IterBase
  {
    private readonly CodeContext _context;

    public ifilter(CodeContext context, object predicate, object iterable)
    {
      this._context = context;
      this.InnerEnumerator = (IEnumerator) this.Yielder(predicate, PythonOps.GetEnumerator(iterable));
    }

    private IEnumerator<object> Yielder(object predicate, IEnumerator iter)
    {
      while (PythonIterTools.MoveNextHelper(iter))
      {
        if (this.ShouldYield(predicate, iter.Current))
          yield return iter.Current;
      }
    }

    private bool ShouldYield(object predicate, object current)
    {
      if (predicate == null)
        return PythonOps.IsTrue(current);
      return Converter.ConvertToBoolean(this._context.LanguageContext.CallSplat(predicate, current));
    }
  }

  [PythonType]
  public class ifilterfalse : PythonIterTools.IterBase
  {
    private readonly CodeContext _context;

    public ifilterfalse(CodeContext context, object predicate, object iterable)
    {
      this._context = context;
      this.InnerEnumerator = (IEnumerator) this.Yielder(predicate, PythonOps.GetEnumerator(iterable));
    }

    private IEnumerator<object> Yielder(object predicate, IEnumerator iter)
    {
      while (PythonIterTools.MoveNextHelper(iter))
      {
        if (this.ShouldYield(predicate, iter.Current))
          yield return iter.Current;
      }
    }

    private bool ShouldYield(object predicate, object current)
    {
      if (predicate == null)
        return !PythonOps.IsTrue(current);
      return !Converter.ConvertToBoolean(this._context.LanguageContext.CallSplat(predicate, current));
    }
  }

  [PythonType]
  public class imap : IEnumerator
  {
    private object _function;
    private IEnumerator[] _iterables;
    private readonly CodeContext _context;

    public imap(CodeContext context, object function, params object[] iterables)
    {
      if (iterables.Length < 1)
        throw PythonOps.TypeError("imap() must have at least two arguments");
      this._function = function;
      this._context = context;
      this._iterables = new IEnumerator[iterables.Length];
      for (int index = 0; index < iterables.Length; ++index)
        this._iterables[index] = PythonOps.GetEnumerator(iterables[index]);
    }

    object IEnumerator.Current
    {
      get
      {
        object[] objArray = new object[this._iterables.Length];
        for (int index = 0; index < objArray.Length; ++index)
          objArray[index] = this._iterables[index].Current;
        return this._function == null ? (object) PythonTuple.MakeTuple(objArray) : this._context.LanguageContext.CallSplat(this._function, objArray);
      }
    }

    bool IEnumerator.MoveNext()
    {
      for (int index = 0; index < this._iterables.Length; ++index)
      {
        if (!PythonIterTools.MoveNextHelper(this._iterables[index]))
          return false;
      }
      return true;
    }

    void IEnumerator.Reset()
    {
      for (int index = 0; index < this._iterables.Length; ++index)
        this._iterables[index].Reset();
    }

    public object __iter__() => (object) this;
  }

  [PythonType]
  public class islice : PythonIterTools.IterBase
  {
    public islice(object iterable, object stop)
      : this(iterable, (object) 0, stop, (object) 1)
    {
    }

    public islice(object iterable, object start, object stop)
      : this(iterable, start, stop, (object) 1)
    {
    }

    public islice(object iterable, object start, object stop, object step)
    {
      int result1 = 0;
      int result2 = -1;
      if (start != null && !Converter.TryConvertToInt32(start, out result1) || result1 < 0)
        throw PythonOps.ValueError("start argument must be non-negative integer, ({0})", start);
      if (stop != null && (!Converter.TryConvertToInt32(stop, out result2) || result2 < 0))
        throw PythonOps.ValueError("stop argument must be non-negative integer ({0})", stop);
      int result3 = 1;
      if (step != null && !Converter.TryConvertToInt32(step, out result3) || result3 <= 0)
        throw PythonOps.ValueError("step must be 1 or greater for islice");
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonOps.GetEnumerator(iterable), result1, result2, result3);
    }

    private IEnumerator<object> Yielder(IEnumerator iter, int start, int stop, int step)
    {
      if (PythonIterTools.MoveNextHelper(iter))
      {
        int cur;
        for (cur = 0; cur < start; ++cur)
        {
          if (!PythonIterTools.MoveNextHelper(iter))
            yield break;
        }
        while (cur < stop || stop == -1)
        {
          yield return iter.Current;
          if (cur + step < 0)
            break;
          for (int index = 0; index < step; ++index)
          {
            if (stop != -1 && ++cur >= stop || !PythonIterTools.MoveNextHelper(iter))
              yield break;
          }
        }
      }
    }
  }

  [PythonType]
  public class izip : IEnumerator
  {
    private readonly IEnumerator[] _iters;
    private PythonTuple _current;

    public izip(params object[] iterables)
    {
      this._iters = new IEnumerator[iterables.Length];
      for (int index = 0; index < iterables.Length; ++index)
        this._iters[index] = PythonOps.GetEnumerator(iterables[index]);
    }

    object IEnumerator.Current => (object) this._current;

    bool IEnumerator.MoveNext()
    {
      if (this._iters.Length == 0)
        return false;
      object[] objArray = new object[this._iters.Length];
      for (int index = 0; index < this._iters.Length; ++index)
      {
        if (!PythonIterTools.MoveNextHelper(this._iters[index]))
          return false;
        objArray[index] = this._iters[index].Current;
      }
      this._current = PythonTuple.MakeTuple(objArray);
      return true;
    }

    void IEnumerator.Reset()
    {
      throw new NotImplementedException("The method or operation is not implemented.");
    }

    public object __iter__() => (object) this;
  }

  [PythonType]
  public class izip_longest : IEnumerator
  {
    private readonly IEnumerator[] _iters;
    private readonly object _fill;
    private PythonTuple _current;

    public izip_longest(params object[] iterables)
    {
      this._iters = new IEnumerator[iterables.Length];
      for (int index = 0; index < iterables.Length; ++index)
        this._iters[index] = PythonOps.GetEnumerator(iterables[index]);
    }

    public izip_longest([ParamDictionary] IDictionary<object, object> paramDict, params object[] iterables)
    {
      object obj;
      if (paramDict.TryGetValue((object) "fillvalue", out obj))
      {
        this._fill = obj;
        if (paramDict.Count != 1)
        {
          paramDict.Remove((object) "fillvalue");
          throw PythonIterTools.UnexpectedKeywordArgument(paramDict);
        }
      }
      else if (paramDict.Count != 0)
        throw PythonIterTools.UnexpectedKeywordArgument(paramDict);
      this._iters = new IEnumerator[iterables.Length];
      for (int index = 0; index < iterables.Length; ++index)
        this._iters[index] = PythonOps.GetEnumerator(iterables[index]);
    }

    object IEnumerator.Current => (object) this._current;

    bool IEnumerator.MoveNext()
    {
      if (this._iters.Length == 0)
        return false;
      object[] objArray = new object[this._iters.Length];
      bool flag = false;
      for (int index = 0; index < this._iters.Length; ++index)
      {
        if (!PythonIterTools.MoveNextHelper(this._iters[index]))
        {
          objArray[index] = this._fill;
        }
        else
        {
          flag = true;
          objArray[index] = this._iters[index].Current;
        }
      }
      if (!flag)
        return false;
      this._current = PythonTuple.MakeTuple(objArray);
      return true;
    }

    void IEnumerator.Reset()
    {
      throw new NotImplementedException("The method or operation is not implemented.");
    }

    public object __iter__() => (object) this;
  }

  [PythonType]
  public class product : PythonIterTools.IterBase
  {
    public product(params object[] iterables)
    {
      this.InnerEnumerator = (IEnumerator) this.Yielder(ArrayUtils.ConvertAll<object, IronPython.Runtime.List>(iterables, (Func<object, IronPython.Runtime.List>) (x => new IronPython.Runtime.List((object) PythonOps.GetEnumerator(x)))));
    }

    public product([ParamDictionary] IDictionary<object, object> paramDict, params object[] iterables)
    {
      num = 1;
      object obj;
      if (paramDict.TryGetValue((object) "repeat", out obj))
      {
        if (!(obj is int num))
          throw PythonOps.TypeError("an integer is required");
        if (paramDict.Count != 1)
        {
          paramDict.Remove((object) "repeat");
          throw PythonIterTools.UnexpectedKeywordArgument(paramDict);
        }
      }
      else if (paramDict.Count != 0)
        throw PythonIterTools.UnexpectedKeywordArgument(paramDict);
      IronPython.Runtime.List[] iterables1 = new IronPython.Runtime.List[iterables.Length * num];
      for (int index1 = 0; index1 < num; ++index1)
      {
        for (int index2 = 0; index2 < iterables.Length; ++index2)
          iterables1[index1 * iterables.Length + index2] = new IronPython.Runtime.List(iterables[index2]);
      }
      this.InnerEnumerator = (IEnumerator) this.Yielder(iterables1);
    }

    private IEnumerator<object> Yielder(IronPython.Runtime.List[] iterables)
    {
      if (iterables.Length != 0)
      {
        IEnumerator[] enums = new IEnumerator[iterables.Length];
        enums[0] = iterables[0].GetEnumerator();
        int curDepth = 0;
        do
        {
          if (enums[curDepth].MoveNext())
          {
            if (curDepth == enums.Length - 1)
            {
              object[] objArray = new object[enums.Length];
              for (int index = 0; index < enums.Length; ++index)
                objArray[index] = enums[index].Current;
              yield return (object) PythonTuple.MakeTuple(objArray);
            }
            else
            {
              ++curDepth;
              enums[curDepth] = iterables[curDepth].GetEnumerator();
            }
          }
          else
            --curDepth;
        }
        while (curDepth != -1);
        enums = (IEnumerator[]) null;
      }
      else
        yield return (object) PythonTuple.EMPTY;
    }
  }

  [PythonType]
  public class combinations : PythonIterTools.IterBase
  {
    private readonly IronPython.Runtime.List _data;

    public combinations(object iterable, object r)
    {
      this._data = new IronPython.Runtime.List(iterable);
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonIterTools.GetR(r, this._data));
    }

    private IEnumerator<object> Yielder(int r)
    {
      IEnumerator[] enums = new IEnumerator[r];
      if (r > 0)
      {
        enums[0] = this._data.GetEnumerator();
        int curDepth = 0;
        int[] curIndices = new int[enums.Length];
        do
        {
          if (enums[curDepth].MoveNext())
          {
            ++curIndices[curDepth];
            bool flag = false;
            for (int index = 0; index < curDepth; ++index)
            {
              if (curIndices[index] >= curIndices[curDepth])
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              if (curDepth == enums.Length - 1)
              {
                object[] objArray = new object[r];
                for (int index = 0; index < enums.Length; ++index)
                  objArray[index] = enums[index].Current;
                yield return (object) PythonTuple.MakeTuple(objArray);
              }
              else
              {
                ++curDepth;
                enums[curDepth] = this._data.GetEnumerator();
                curIndices[curDepth] = 0;
              }
            }
          }
          else
            --curDepth;
        }
        while (curDepth != -1);
        curIndices = (int[]) null;
      }
      else
        yield return (object) PythonTuple.EMPTY;
    }
  }

  [PythonType]
  public class combinations_with_replacement : PythonIterTools.IterBase
  {
    private readonly IronPython.Runtime.List _data;

    public combinations_with_replacement(object iterable, object r)
    {
      this._data = new IronPython.Runtime.List(iterable);
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonIterTools.GetR(r, this._data));
    }

    private IEnumerator<object> Yielder(int r)
    {
      IEnumerator[] enums = new IEnumerator[r];
      if (r > 0)
      {
        enums[0] = this._data.GetEnumerator();
        int curDepth = 0;
        int[] curIndices = new int[enums.Length];
        do
        {
          if (enums[curDepth].MoveNext())
          {
            ++curIndices[curDepth];
            bool flag = false;
            for (int index = 0; index < curDepth; ++index)
            {
              if (curIndices[index] > curIndices[curDepth])
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              if (curDepth == enums.Length - 1)
              {
                object[] objArray = new object[r];
                for (int index = 0; index < enums.Length; ++index)
                  objArray[index] = enums[index].Current;
                yield return (object) PythonTuple.MakeTuple(objArray);
              }
              else
              {
                ++curDepth;
                enums[curDepth] = this._data.GetEnumerator();
                curIndices[curDepth] = 0;
              }
            }
          }
          else
            --curDepth;
        }
        while (curDepth != -1);
        curIndices = (int[]) null;
      }
      else
        yield return (object) PythonTuple.EMPTY;
    }
  }

  [PythonType]
  public class permutations : PythonIterTools.IterBase
  {
    private readonly IronPython.Runtime.List _data;

    public permutations(object iterable)
    {
      this._data = new IronPython.Runtime.List(iterable);
      this.InnerEnumerator = (IEnumerator) this.Yielder(this._data.Count);
    }

    public permutations(object iterable, object r)
    {
      this._data = new IronPython.Runtime.List(iterable);
      this.InnerEnumerator = (IEnumerator) this.Yielder(PythonIterTools.GetR(r, this._data));
    }

    private IEnumerator<object> Yielder(int r)
    {
      if (r > 0)
      {
        IEnumerator[] enums = new IEnumerator[r];
        enums[0] = this._data.GetEnumerator();
        int curDepth = 0;
        int[] curIndices = new int[enums.Length];
        do
        {
          if (enums[curDepth].MoveNext())
          {
            ++curIndices[curDepth];
            bool flag = false;
            for (int index = 0; index < curDepth; ++index)
            {
              if (curIndices[index] == curIndices[curDepth])
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              if (curDepth == enums.Length - 1)
              {
                object[] objArray = new object[r];
                for (int index = 0; index < enums.Length; ++index)
                  objArray[index] = enums[index].Current;
                yield return (object) PythonTuple.MakeTuple(objArray);
              }
              else
              {
                ++curDepth;
                enums[curDepth] = this._data.GetEnumerator();
                curIndices[curDepth] = 0;
              }
            }
          }
          else
            --curDepth;
        }
        while (curDepth != -1);
        enums = (IEnumerator[]) null;
        curIndices = (int[]) null;
      }
      else
        yield return (object) PythonTuple.EMPTY;
    }
  }

  [PythonType]
  [DontMapICollectionToLen]
  public class repeat : PythonIterTools.IterBase, ICodeFormattable, ICollection, IEnumerable
  {
    private int _remaining;
    private bool _fInfinite;
    private object _obj;

    public repeat(object @object)
    {
      this._obj = @object;
      this.InnerEnumerator = (IEnumerator) this.Yielder();
      this._fInfinite = true;
    }

    public repeat(object @object, int times)
    {
      this._obj = @object;
      this.InnerEnumerator = (IEnumerator) this.Yielder();
      this._remaining = times;
    }

    private IEnumerator<object> Yielder()
    {
      while (this._fInfinite || this._remaining > 0)
      {
        --this._remaining;
        yield return this._obj;
      }
    }

    public int __length_hint__()
    {
      if (this._fInfinite)
        throw PythonOps.TypeError("len() of unsized object");
      return Math.Max(this._remaining, 0);
    }

    public virtual string __repr__(CodeContext context)
    {
      return this._fInfinite ? $"{PythonOps.GetPythonTypeName((object) this)}({PythonOps.Repr(context, this._obj)})" : $"{PythonOps.GetPythonTypeName((object) this)}({PythonOps.Repr(context, this._obj)}, {this._remaining})";
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (this._fInfinite)
        throw new InvalidOperationException();
      if (this._remaining > array.Length - index)
        throw new IndexOutOfRangeException();
      for (int index1 = 0; index1 < this._remaining; ++index1)
        array.SetValue(this._obj, index + index1);
      this._remaining = 0;
    }

    int ICollection.Count => this.__length_hint__();

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => (object) this;

    IEnumerator IEnumerable.GetEnumerator()
    {
      while (this._fInfinite || this._remaining > 0)
      {
        --this._remaining;
        yield return this._obj;
      }
    }
  }

  [PythonType]
  public class starmap : PythonIterTools.IterBase
  {
    public starmap(CodeContext context, object function, object iterable)
    {
      this.InnerEnumerator = (IEnumerator) this.Yielder(context, function, PythonOps.GetEnumerator(iterable));
    }

    private IEnumerator<object> Yielder(CodeContext context, object function, IEnumerator iter)
    {
      PythonContext pc = context.LanguageContext;
      while (PythonIterTools.MoveNextHelper(iter))
      {
        object[] objArray;
        if (iter.Current is PythonTuple current)
        {
          objArray = new object[current.__len__()];
          for (int index = 0; index < objArray.Length; ++index)
            objArray[index] = current[index];
        }
        else
          objArray = ArrayUtils.ToArray<object>((ICollection<object>) new IronPython.Runtime.List((object) PythonOps.GetEnumerator(iter.Current)));
        yield return pc.CallSplat(function, objArray);
      }
    }
  }

  [PythonType]
  public class takewhile : PythonIterTools.IterBase
  {
    private readonly CodeContext _context;

    public takewhile(CodeContext context, object predicate, object iterable)
    {
      this._context = context;
      this.InnerEnumerator = (IEnumerator) this.Yielder(predicate, PythonOps.GetEnumerator(iterable));
    }

    private IEnumerator<object> Yielder(object predicate, IEnumerator iter)
    {
      while (PythonIterTools.MoveNextHelper(iter))
      {
        if (!Converter.ConvertToBoolean(this._context.LanguageContext.CallSplat(predicate, iter.Current)))
          break;
        yield return iter.Current;
      }
    }
  }

  [PythonHidden(new PlatformID[] {})]
  public class TeeIterator : IEnumerator, IWeakReferenceable
  {
    internal IEnumerator _iter;
    internal IronPython.Runtime.List _data;
    private int _curIndex = -1;
    private WeakRefTracker _weakRef;

    public TeeIterator(object iterable)
    {
      if (iterable is PythonIterTools.TeeIterator teeIterator)
      {
        this._iter = teeIterator._iter;
        this._data = teeIterator._data;
      }
      else
      {
        this._iter = PythonOps.GetEnumerator(iterable);
        this._data = new IronPython.Runtime.List();
      }
    }

    public TeeIterator(IEnumerator iter, IronPython.Runtime.List dataList)
    {
      this._iter = iter;
      this._data = dataList;
    }

    object IEnumerator.Current => this._data[this._curIndex];

    bool IEnumerator.MoveNext()
    {
      lock (this._data)
      {
        ++this._curIndex;
        if (this._curIndex >= this._data.__len__() && PythonIterTools.MoveNextHelper(this._iter))
          this._data.append(this._iter.Current);
        return this._curIndex < this._data.__len__();
      }
    }

    void IEnumerator.Reset()
    {
      throw new NotImplementedException("The method or operation is not implemented.");
    }

    public object __iter__() => (object) this;

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._weakRef;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      this._weakRef = value;
      return true;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._weakRef = value;
  }
}
