// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.FunctionTools
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Binding;
using IronPython.Runtime.Operations;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace IronPython.Modules;

public static class FunctionTools
{
  public const string __doc__ = "provides functionality for manipulating callable objects";

  public static object reduce(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object, object, object>>> siteData,
    object func,
    object seq)
  {
    return Builtin.reduce(context, siteData, func, seq);
  }

  public static object reduce(
    CodeContext context,
    SiteLocalStorage<CallSite<Func<CallSite, CodeContext, object, object, object, object>>> siteData,
    object func,
    object seq,
    object initializer)
  {
    return Builtin.reduce(context, siteData, func, seq, initializer);
  }

  [PythonType]
  public class partial : IWeakReferenceable
  {
    private const string _defaultDoc = "partial(func, *args, **keywords) - new function with partial application\n    of the given arguments and keywords.\n";
    private object _function;
    private object[] _args;
    private IDictionary<object, object> _keywordArgs;
    private CodeContext _context;
    private CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>> _dictSite;
    private CallSite<Func<CallSite, CodeContext, object, object[], object>> _splatSite;
    private PythonDictionary _dict;
    private WeakRefTracker _tracker;
    private string _doc;

    public partial(CodeContext context, object func, [NotNull] params object[] args)
      : this(context, func, (IDictionary<object, object>) null, args)
    {
    }

    public partial(
      CodeContext context,
      object func,
      [ParamDictionary] IDictionary<object, object> keywords,
      [NotNull] params object[] args)
    {
      this._function = PythonOps.IsCallable(context, func) ? func : throw PythonOps.TypeError("the first argument must be callable");
      this._keywordArgs = keywords;
      this._args = args;
      this._context = context;
    }

    [PropertyMethod]
    [WrapperDescriptor]
    [SpecialName]
    public static object Get__doc__(CodeContext context, FunctionTools.partial self)
    {
      return (object) self._doc ?? (object) "partial(func, *args, **keywords) - new function with partial application\n    of the given arguments and keywords.\n";
    }

    [PropertyMethod]
    [WrapperDescriptor]
    [SpecialName]
    public static void Set__doc__(FunctionTools.partial self, object value)
    {
      self._doc = value as string;
    }

    public object func => this._function;

    public object args => (object) PythonTuple.MakeTuple(this._args);

    public object keywords => (object) this._keywordArgs;

    public PythonDictionary __dict__
    {
      get => this.EnsureDict();
      set => this._dict = value;
    }

    [PropertyMethod]
    [SpecialName]
    public void Delete__dict__()
    {
      throw PythonOps.TypeError("partial's dictionary may not be deleted");
    }

    public void __delattr__(string name)
    {
      if (name == "__dict__")
        this.Delete__dict__();
      if (this._dict == null)
        return;
      this._dict.Remove((object) name);
    }

    [SpecialName]
    public object Call(CodeContext context, params object[] args)
    {
      if (this._keywordArgs == null)
      {
        this.EnsureSplatSite();
        return this._splatSite.Target((CallSite) this._splatSite, context, this._function, ArrayUtils.AppendRange<object>(this._args, (IList<object>) args));
      }
      this.EnsureDictSplatSite();
      return this._dictSite.Target((CallSite) this._dictSite, context, this._function, ArrayUtils.AppendRange<object>(this._args, (IList<object>) args), this._keywordArgs);
    }

    [SpecialName]
    public object Call(CodeContext context, [ParamDictionary] IDictionary<object, object> dict, params object[] args)
    {
      IDictionary<object, object> dictionary;
      if (this._keywordArgs != null)
      {
        PythonDictionary pythonDictionary = new PythonDictionary();
        pythonDictionary.update(context, this._keywordArgs);
        pythonDictionary.update(context, dict);
        dictionary = (IDictionary<object, object>) pythonDictionary;
      }
      else
        dictionary = dict;
      this.EnsureDictSplatSite();
      return this._dictSite.Target((CallSite) this._dictSite, context, this._function, ArrayUtils.AppendRange<object>(this._args, (IList<object>) args), dictionary);
    }

    [SpecialName]
    public void SetMemberAfter(CodeContext context, string name, object value)
    {
      this.EnsureDict();
      this._dict[(object) name] = value;
    }

    [SpecialName]
    public object GetBoundMember(CodeContext context, string name)
    {
      object obj;
      return this._dict != null && this._dict.TryGetValue((object) name, out obj) ? obj : (object) OperationFailed.Value;
    }

    [SpecialName]
    public bool DeleteMember(CodeContext context, string name)
    {
      if (name == "__dict__")
        this.Delete__dict__();
      return this._dict != null && this._dict.Remove((object) name);
    }

    private void EnsureSplatSite()
    {
      if (this._splatSite != null)
        return;
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object[], object>>>(ref this._splatSite, CallSite<Func<CallSite, CodeContext, object, object[], object>>.Create((CallSiteBinder) Binders.InvokeSplat(this._context.LanguageContext)), (CallSite<Func<CallSite, CodeContext, object, object[], object>>) null);
    }

    private void EnsureDictSplatSite()
    {
      if (this._dictSite != null)
        return;
      Interlocked.CompareExchange<CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>>(ref this._dictSite, CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>.Create((CallSiteBinder) Binders.InvokeKeywords(this._context.LanguageContext)), (CallSite<Func<CallSite, CodeContext, object, object[], IDictionary<object, object>, object>>) null);
    }

    private PythonDictionary EnsureDict()
    {
      if (this._dict == null)
        this._dict = PythonDictionary.MakeSymbolDictionary();
      return this._dict;
    }

    WeakRefTracker IWeakReferenceable.GetWeakRef() => this._tracker;

    bool IWeakReferenceable.SetWeakRef(WeakRefTracker value)
    {
      return Interlocked.CompareExchange<WeakRefTracker>(ref this._tracker, value, (WeakRefTracker) null) == null;
    }

    void IWeakReferenceable.SetFinalizer(WeakRefTracker value) => this._tracker = value;
  }
}
