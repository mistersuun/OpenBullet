// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonThread
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class PythonThread
{
  public const string __doc__ = "Provides low level primitives for threading.";
  private static readonly object _stackSizeKey = new object();
  private static object _threadCountKey = new object();
  public static readonly PythonType LockType = DynamicHelpers.GetPythonTypeFromType(typeof (PythonThread.@lock));

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    context.SetModuleState(PythonThread._stackSizeKey, (object) 0);
    context.EnsureModuleException((object) "threaderror", dict, "error", "thread");
  }

  [Documentation("start_new_thread(function, [args, [kwDict]]) -> thread id\nCreates a new thread running the given function")]
  public static object start_new_thread(
    CodeContext context,
    object function,
    object args,
    object kwDict)
  {
    if (!(args is PythonTuple args1))
      throw PythonOps.TypeError("2nd arg must be a tuple");
    Thread thread = PythonThread.CreateThread(context, new ThreadStart(new PythonThread.ThreadObj(context, function, args1, kwDict).Start));
    thread.Start();
    return (object) thread.ManagedThreadId;
  }

  [Documentation("start_new_thread(function, args, [kwDict]) -> thread id\nCreates a new thread running the given function")]
  public static object start_new_thread(CodeContext context, object function, object args)
  {
    if (!(args is PythonTuple args1))
      throw PythonOps.TypeError("2nd arg must be a tuple");
    Thread thread = PythonThread.CreateThread(context, new ThreadStart(new PythonThread.ThreadObj(context, function, args1, (object) null).Start));
    thread.IsBackground = true;
    thread.Start();
    return (object) thread.ManagedThreadId;
  }

  public static void interrupt_main(CodeContext context)
  {
    (context.LanguageContext.MainThread ?? throw PythonOps.SystemError("no main thread has been registered")).Abort((object) new KeyboardInterruptException(""));
  }

  public static void exit() => PythonOps.SystemExit();

  [Documentation("allocate_lock() -> lock object\nAllocates a new lock object that can be used for synchronization")]
  public static object allocate_lock() => (object) new PythonThread.@lock();

  public static object get_ident() => (object) Thread.CurrentThread.ManagedThreadId;

  public static int stack_size(CodeContext context) => PythonThread.GetStackSize(context);

  public static int stack_size(CodeContext context, int size)
  {
    if (size < 32768 /*0x8000*/ && size != 0)
      throw PythonOps.ValueError("size too small: {0}", (object) size);
    int stackSize = PythonThread.GetStackSize(context);
    PythonThread.SetStackSize(context, size);
    return stackSize;
  }

  [Documentation("start_new(function, [args, [kwDict]]) -> thread id\nCreates a new thread running the given function")]
  public static object start_new(CodeContext context, object function, object args)
  {
    return PythonThread.start_new_thread(context, function, args);
  }

  public static void exit_thread() => PythonThread.exit();

  public static object allocate() => PythonThread.allocate_lock();

  public static int _count(CodeContext context)
  {
    return (int) context.LanguageContext.GetOrCreateModuleState<object>(PythonThread._threadCountKey, (Func<object>) (() => (object) 0));
  }

  private static Thread CreateThread(CodeContext context, ThreadStart start)
  {
    int stackSize = PythonThread.GetStackSize(context);
    return stackSize == 0 ? new Thread(start) : new Thread(start, stackSize);
  }

  private static int GetStackSize(CodeContext context)
  {
    return (int) context.LanguageContext.GetModuleState(PythonThread._stackSizeKey);
  }

  private static void SetStackSize(CodeContext context, int stackSize)
  {
    context.LanguageContext.SetModuleState(PythonThread._stackSizeKey, (object) stackSize);
  }

  [PythonType]
  [PythonHidden(new PlatformID[] {})]
  public class @lock
  {
    private AutoResetEvent blockEvent;
    private Thread curHolder;

    public object __enter__()
    {
      this.acquire();
      return (object) this;
    }

    public void __exit__(CodeContext context, params object[] args) => this.release(context);

    public object acquire() => this.acquire(ScriptingRuntimeHelpers.True);

    public object acquire(object waitflag)
    {
      bool flag = PythonOps.IsTrue(waitflag);
      while (Interlocked.CompareExchange<Thread>(ref this.curHolder, Thread.CurrentThread, (Thread) null) != null)
      {
        if (!flag)
          return ScriptingRuntimeHelpers.False;
        if (this.blockEvent == null)
        {
          this.CreateBlockEvent();
        }
        else
        {
          this.blockEvent.WaitOne();
          GC.KeepAlive((object) this);
        }
      }
      return ScriptingRuntimeHelpers.True;
    }

    public void release(CodeContext context, params object[] param) => this.release(context);

    public void release(CodeContext context)
    {
      if (Interlocked.Exchange<Thread>(ref this.curHolder, (Thread) null) == null)
        throw PythonExceptions.CreateThrowable((PythonType) context.LanguageContext.GetModuleState((object) "threaderror"), (object) "lock isn't held", null);
      if (this.blockEvent == null)
        return;
      this.blockEvent.Set();
      GC.KeepAlive((object) this);
    }

    public bool locked() => this.curHolder != null;

    private void CreateBlockEvent()
    {
      AutoResetEvent autoResetEvent = new AutoResetEvent(false);
      if (Interlocked.CompareExchange<AutoResetEvent>(ref this.blockEvent, autoResetEvent, (AutoResetEvent) null) == null)
        return;
      autoResetEvent.Dispose();
    }
  }

  private class ThreadObj
  {
    private readonly object _func;
    private readonly object _kwargs;
    private readonly PythonTuple _args;
    private readonly CodeContext _context;

    public ThreadObj(CodeContext context, object function, PythonTuple args, object kwargs)
    {
      this._func = function;
      this._kwargs = kwargs;
      this._args = args;
      this._context = context;
    }

    public void Start()
    {
      lock (PythonThread._threadCountKey)
      {
        int moduleState = (int) this._context.LanguageContext.GetOrCreateModuleState<object>(PythonThread._threadCountKey, (Func<object>) (() => (object) 0));
        this._context.LanguageContext.SetModuleState(PythonThread._threadCountKey, (object) (moduleState + 1));
      }
      try
      {
        if (this._kwargs != null)
          PythonOps.CallWithArgsTupleAndKeywordDictAndContext(this._context, this._func, ArrayUtils.EmptyObjects, ArrayUtils.EmptyStrings, (object) this._args, this._kwargs);
        else
          PythonOps.CallWithArgsTuple(this._func, ArrayUtils.EmptyObjects, (object) this._args);
      }
      catch (SystemExitException ex)
      {
      }
      catch (Exception ex)
      {
        PythonOps.PrintWithDest(this._context, this._context.LanguageContext.SystemStandardError, (object) "Unhandled exception on thread");
        PythonOps.PrintWithDest(this._context, this._context.LanguageContext.SystemStandardError, (object) this._context.LanguageContext.FormatException(ex));
      }
      finally
      {
        lock (PythonThread._threadCountKey)
        {
          int moduleState = (int) this._context.LanguageContext.GetModuleState(PythonThread._threadCountKey);
          this._context.LanguageContext.SetModuleState(PythonThread._threadCountKey, (object) (moduleState - 1));
        }
      }
    }
  }

  [PythonType]
  public class _local
  {
    private readonly PythonDictionary _dict = new PythonDictionary((DictionaryStorage) new PythonThread._local.ThreadLocalDictionaryStorage());

    [SpecialName]
    public object GetCustomMember(string name)
    {
      return this._dict.get((object) name, (object) OperationFailed.Value);
    }

    [SpecialName]
    public void SetMemberAfter(string name, object value) => this._dict[(object) name] = value;

    [SpecialName]
    public void DeleteMember(string name) => this._dict.__delitem__((object) name);

    public PythonDictionary __dict__ => this._dict;

    private class ThreadLocalDictionaryStorage : DictionaryStorage
    {
      private readonly Microsoft.Scripting.Utils.ThreadLocal<CommonDictionaryStorage> _storage = new Microsoft.Scripting.Utils.ThreadLocal<CommonDictionaryStorage>();

      public override void Add(ref DictionaryStorage storage, object key, object value)
      {
        this.GetStorage().Add(key, value);
      }

      public override bool Contains(object key) => this.GetStorage().Contains(key);

      public override bool Remove(ref DictionaryStorage storage, object key)
      {
        return this.GetStorage().Remove(ref storage, key);
      }

      public override bool TryGetValue(object key, out object value)
      {
        return this.GetStorage().TryGetValue(key, out value);
      }

      public override int Count => this.GetStorage().Count;

      public override void Clear(ref DictionaryStorage storage)
      {
        this.GetStorage().Clear(ref storage);
      }

      public override List<KeyValuePair<object, object>> GetItems() => this.GetStorage().GetItems();

      private CommonDictionaryStorage GetStorage()
      {
        return this._storage.GetOrCreate((Func<CommonDictionaryStorage>) (() => new CommonDictionaryStorage()));
      }
    }
  }
}
