// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DynamicOperations
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public sealed class DynamicOperations
{
  private readonly LanguageContext _lc;
  private Dictionary<DynamicOperations.SiteKey, DynamicOperations.SiteKey> _sites = new Dictionary<DynamicOperations.SiteKey, DynamicOperations.SiteKey>();
  private int LastCleanup;
  private int SitesCreated;
  private const int CleanupThreshold = 20;
  private const int RemoveThreshold = 2;
  private const int StopCleanupThreshold = 10;
  private const int ClearThreshold = 50;
  private Dictionary<int, Func<DynamicOperations, CallSiteBinder, object, object[], object>> _invokers = new Dictionary<int, Func<DynamicOperations, CallSiteBinder, object, object[], object>>();
  private const int PregeneratedInvokerCount = 14;

  public DynamicOperations(LanguageContext lc)
  {
    ContractUtils.RequiresNotNull((object) lc, nameof (lc));
    this._lc = lc;
  }

  public object Invoke(object obj, params object[] parameters)
  {
    return this.GetInvoker(parameters.Length)(this, (CallSiteBinder) this._lc.CreateInvokeBinder(new CallInfo(parameters.Length, new string[0])), obj, parameters);
  }

  public object InvokeMember(object obj, string memberName, params object[] parameters)
  {
    return this.InvokeMember(obj, memberName, false, parameters);
  }

  public object InvokeMember(
    object obj,
    string memberName,
    bool ignoreCase,
    params object[] parameters)
  {
    return this.GetInvoker(parameters.Length)(this, (CallSiteBinder) this._lc.CreateCallBinder(memberName, ignoreCase, new CallInfo(parameters.Length, new string[0])), obj, parameters);
  }

  public object CreateInstance(object obj, params object[] parameters)
  {
    return this.GetInvoker(parameters.Length)(this, (CallSiteBinder) this._lc.CreateCreateBinder(new CallInfo(parameters.Length, new string[0])), obj, parameters);
  }

  public object GetMember(object obj, string name) => this.GetMember(obj, name, false);

  public T GetMember<T>(object obj, string name) => this.GetMember<T>(obj, name, false);

  public bool TryGetMember(object obj, string name, out object value)
  {
    return this.TryGetMember(obj, name, false, out value);
  }

  public bool ContainsMember(object obj, string name) => this.ContainsMember(obj, name, false);

  public void RemoveMember(object obj, string name) => this.RemoveMember(obj, name, false);

  public void SetMember(object obj, string name, object value)
  {
    this.SetMember(obj, name, value, false);
  }

  public void SetMember<T>(object obj, string name, T value)
  {
    this.SetMember<T>(obj, name, value, false);
  }

  public object GetMember(object obj, string name, bool ignoreCase)
  {
    CallSite<Func<CallSite, object, object>> site = this.GetOrCreateSite<object, object>((CallSiteBinder) this._lc.CreateGetMemberBinder(name, ignoreCase));
    return site.Target((CallSite) site, obj);
  }

  public T GetMember<T>(object obj, string name, bool ignoreCase)
  {
    CallSite<Func<CallSite, object, T>> site1 = this.GetOrCreateSite<object, T>((CallSiteBinder) this._lc.CreateConvertBinder(typeof (T), new bool?()));
    CallSite<Func<CallSite, object, object>> site2 = this.GetOrCreateSite<object, object>((CallSiteBinder) this._lc.CreateGetMemberBinder(name, ignoreCase));
    return site1.Target((CallSite) site1, site2.Target((CallSite) site2, obj));
  }

  public bool TryGetMember(object obj, string name, bool ignoreCase, out object value)
  {
    try
    {
      value = this.GetMember(obj, name, ignoreCase);
      return true;
    }
    catch (MissingMemberException ex)
    {
      value = (object) null;
      return false;
    }
  }

  public bool ContainsMember(object obj, string name, bool ignoreCase)
  {
    return this.TryGetMember(obj, name, ignoreCase, out object _);
  }

  public void RemoveMember(object obj, string name, bool ignoreCase)
  {
    CallSite<Action<CallSite, object>> actionSite = this.GetOrCreateActionSite<object>((CallSiteBinder) this._lc.CreateDeleteMemberBinder(name, ignoreCase));
    actionSite.Target((CallSite) actionSite, obj);
  }

  public void SetMember(object obj, string name, object value, bool ignoreCase)
  {
    CallSite<Func<CallSite, object, object, object>> site = this.GetOrCreateSite<object, object, object>((CallSiteBinder) this._lc.CreateSetMemberBinder(name, ignoreCase));
    object obj1 = site.Target((CallSite) site, obj, value);
  }

  public void SetMember<T>(object obj, string name, T value, bool ignoreCase)
  {
    CallSite<Func<CallSite, object, T, object>> site = this.GetOrCreateSite<object, T, object>((CallSiteBinder) this._lc.CreateSetMemberBinder(name, ignoreCase));
    object obj1 = site.Target((CallSite) site, obj, value);
  }

  public T ConvertTo<T>(object obj)
  {
    CallSite<Func<CallSite, object, T>> site = this.GetOrCreateSite<object, T>((CallSiteBinder) this._lc.CreateConvertBinder(typeof (T), new bool?()));
    return site.Target((CallSite) site, obj);
  }

  public object ConvertTo(object obj, Type type)
  {
    if (type.IsInterface || type.IsClass)
    {
      CallSite<Func<CallSite, object, object>> site = this.GetOrCreateSite<object, object>((CallSiteBinder) this._lc.CreateConvertBinder(type, new bool?()));
      return site.Target((CallSite) site, obj);
    }
    foreach (MethodInfo declaredMethod in typeof (DynamicOperations).GetDeclaredMethods(nameof (ConvertTo)))
    {
      if (declaredMethod.IsGenericMethod)
      {
        try
        {
          return declaredMethod.MakeGenericMethod(type).Invoke((object) this, new object[1]
          {
            obj
          });
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }
    throw new InvalidOperationException();
  }

  public bool TryConvertTo<T>(object obj, out T result)
  {
    try
    {
      result = this.ConvertTo<T>(obj);
      return true;
    }
    catch (ArgumentTypeException ex)
    {
      result = default (T);
      return false;
    }
    catch (InvalidCastException ex)
    {
      result = default (T);
      return false;
    }
  }

  public bool TryConvertTo(object obj, Type type, out object result)
  {
    try
    {
      result = this.ConvertTo(obj, type);
      return true;
    }
    catch (ArgumentTypeException ex)
    {
      result = (object) null;
      return false;
    }
    catch (InvalidCastException ex)
    {
      result = (object) null;
      return false;
    }
  }

  public T ExplicitConvertTo<T>(object obj)
  {
    CallSite<Func<CallSite, object, T>> site = this.GetOrCreateSite<object, T>((CallSiteBinder) this._lc.CreateConvertBinder(typeof (T), new bool?(true)));
    return site.Target((CallSite) site, obj);
  }

  public object ExplicitConvertTo(object obj, Type type)
  {
    CallSite<Func<CallSite, object, object>> site = this.GetOrCreateSite<object, object>((CallSiteBinder) this._lc.CreateConvertBinder(type, new bool?(true)));
    return site.Target((CallSite) site, obj);
  }

  public bool TryExplicitConvertTo(object obj, Type type, out object result)
  {
    try
    {
      result = this.ExplicitConvertTo(obj, type);
      return true;
    }
    catch (ArgumentTypeException ex)
    {
      result = (object) null;
      return false;
    }
    catch (InvalidCastException ex)
    {
      result = (object) null;
      return false;
    }
  }

  public bool TryExplicitConvertTo<T>(object obj, out T result)
  {
    try
    {
      result = this.ExplicitConvertTo<T>(obj);
      return true;
    }
    catch (ArgumentTypeException ex)
    {
      result = default (T);
      return false;
    }
    catch (InvalidCastException ex)
    {
      result = default (T);
      return false;
    }
  }

  public T ImplicitConvertTo<T>(object obj)
  {
    CallSite<Func<CallSite, object, T>> site = this.GetOrCreateSite<object, T>((CallSiteBinder) this._lc.CreateConvertBinder(typeof (T), new bool?(false)));
    return site.Target((CallSite) site, obj);
  }

  public object ImplicitConvertTo(object obj, Type type)
  {
    CallSite<Func<CallSite, object, object>> site = this.GetOrCreateSite<object, object>((CallSiteBinder) this._lc.CreateConvertBinder(type, new bool?(false)));
    return site.Target((CallSite) site, obj);
  }

  public bool TryImplicitConvertTo(object obj, Type type, out object result)
  {
    try
    {
      result = this.ImplicitConvertTo(obj, type);
      return true;
    }
    catch (ArgumentTypeException ex)
    {
      result = (object) null;
      return false;
    }
    catch (InvalidCastException ex)
    {
      result = (object) null;
      return false;
    }
  }

  public bool TryImplicitConvertTo<T>(object obj, out T result)
  {
    try
    {
      result = this.ImplicitConvertTo<T>(obj);
      return true;
    }
    catch (ArgumentTypeException ex)
    {
      result = default (T);
      return false;
    }
    catch (InvalidCastException ex)
    {
      result = default (T);
      return false;
    }
  }

  public TResult DoOperation<TTarget, TResult>(ExpressionType operation, TTarget target)
  {
    CallSite<Func<CallSite, TTarget, TResult>> site = this.GetOrCreateSite<TTarget, TResult>((CallSiteBinder) this._lc.CreateUnaryOperationBinder(operation));
    return site.Target((CallSite) site, target);
  }

  public TResult DoOperation<TTarget, TOther, TResult>(
    ExpressionType operation,
    TTarget target,
    TOther other)
  {
    CallSite<Func<CallSite, TTarget, TOther, TResult>> site = this.GetOrCreateSite<TTarget, TOther, TResult>((CallSiteBinder) this._lc.CreateBinaryOperationBinder(operation));
    return site.Target((CallSite) site, target, other);
  }

  public string GetDocumentation(object o) => this._lc.GetDocumentation(o);

  public IList<string> GetCallSignatures(object o) => this._lc.GetCallSignatures(o);

  public bool IsCallable(object o) => this._lc.IsCallable(o);

  public IList<string> GetMemberNames(object obj) => this._lc.GetMemberNames(obj);

  public string Format(object obj) => this._lc.FormatObject(this, obj);

  public CallSite<Func<CallSite, T1, TResult>> GetOrCreateSite<T1, TResult>(
    CallSiteBinder siteBinder)
  {
    return this.GetOrCreateSite<CallSite<Func<CallSite, T1, TResult>>>(siteBinder, new Func<CallSiteBinder, CallSite<Func<CallSite, T1, TResult>>>(CallSite<Func<CallSite, T1, TResult>>.Create));
  }

  public CallSite<Action<CallSite, T1>> GetOrCreateActionSite<T1>(CallSiteBinder siteBinder)
  {
    return this.GetOrCreateSite<CallSite<Action<CallSite, T1>>>(siteBinder, new Func<CallSiteBinder, CallSite<Action<CallSite, T1>>>(CallSite<Action<CallSite, T1>>.Create));
  }

  public CallSite<Func<CallSite, T1, T2, TResult>> GetOrCreateSite<T1, T2, TResult>(
    CallSiteBinder siteBinder)
  {
    return this.GetOrCreateSite<CallSite<Func<CallSite, T1, T2, TResult>>>(siteBinder, new Func<CallSiteBinder, CallSite<Func<CallSite, T1, T2, TResult>>>(CallSite<Func<CallSite, T1, T2, TResult>>.Create));
  }

  public CallSite<Func<CallSite, T1, T2, T3, TResult>> GetOrCreateSite<T1, T2, T3, TResult>(
    CallSiteBinder siteBinder)
  {
    return this.GetOrCreateSite<CallSite<Func<CallSite, T1, T2, T3, TResult>>>(siteBinder, new Func<CallSiteBinder, CallSite<Func<CallSite, T1, T2, T3, TResult>>>(CallSite<Func<CallSite, T1, T2, T3, TResult>>.Create));
  }

  public CallSite<TSiteFunc> GetOrCreateSite<TSiteFunc>(CallSiteBinder siteBinder) where TSiteFunc : class
  {
    return this.GetOrCreateSite<CallSite<TSiteFunc>>(siteBinder, new Func<CallSiteBinder, CallSite<TSiteFunc>>(CallSite<TSiteFunc>.Create));
  }

  private T GetOrCreateSite<T>(CallSiteBinder siteBinder, Func<CallSiteBinder, T> factory) where T : CallSite
  {
    DynamicOperations.SiteKey key = new DynamicOperations.SiteKey(typeof (T), siteBinder);
    lock (this._sites)
    {
      DynamicOperations.SiteKey siteKey;
      if (!this._sites.TryGetValue(key, out siteKey))
      {
        ++this.SitesCreated;
        if (this.SitesCreated < 0)
        {
          this.SitesCreated = 0;
          this.LastCleanup = 0;
        }
        key.Site = (CallSite) factory(key.SiteBinder);
        this._sites[key] = key;
      }
      else
        key = siteKey;
      ++key.HitCount;
      this.CleanupNoLock();
    }
    return (T) key.Site;
  }

  private void CleanupNoLock()
  {
    if (this._sites.Count <= 20 || this.LastCleanup >= this.SitesCreated - 20)
      return;
    this.LastCleanup = this.SitesCreated;
    int num1 = 0;
    foreach (DynamicOperations.SiteKey key in this._sites.Keys)
      num1 += key.HitCount;
    int num2 = num1 / this._sites.Count;
    if (num2 == 1 && this._sites.Count > 50)
    {
      this._sites.Clear();
    }
    else
    {
      List<DynamicOperations.SiteKey> siteKeyList = (List<DynamicOperations.SiteKey>) null;
      foreach (DynamicOperations.SiteKey key in this._sites.Keys)
      {
        if (key.HitCount < num2 - 2)
        {
          if (siteKeyList == null)
            siteKeyList = new List<DynamicOperations.SiteKey>();
          siteKeyList.Add(key);
          if (siteKeyList.Count > 10)
            break;
        }
      }
      if (siteKeyList == null)
        return;
      foreach (DynamicOperations.SiteKey key in siteKeyList)
        this._sites.Remove(key);
      foreach (DynamicOperations.SiteKey key in this._sites.Keys)
        key.HitCount = 0;
    }
  }

  private Func<DynamicOperations, CallSiteBinder, object, object[], object> GetInvoker(
    int paramCount)
  {
    Func<DynamicOperations, CallSiteBinder, object, object[], object> invoker;
    lock (this._invokers)
    {
      if (!this._invokers.TryGetValue(paramCount, out invoker))
        this._invokers[paramCount] = invoker = DynamicOperations.GetPregeneratedInvoker(paramCount) ?? this.EmitInvoker(paramCount);
    }
    return invoker;
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private Func<DynamicOperations, CallSiteBinder, object, object[], object> EmitInvoker(
    int paramCount)
  {
    ParameterExpression instance = Expression.Parameter(typeof (DynamicOperations));
    ParameterExpression parameterExpression5 = Expression.Parameter(typeof (CallSiteBinder));
    ParameterExpression parameterExpression6 = Expression.Parameter(typeof (object));
    ParameterExpression array = Expression.Parameter(typeof (object[]));
    Type type = DelegateUtils.EmitCallSiteDelegateType(paramCount);
    ParameterExpression left = Expression.Parameter(typeof (CallSite<>).MakeGenericType(type));
    Expression[] expressionArray = new Expression[paramCount + 2];
    expressionArray[0] = (Expression) left;
    expressionArray[1] = (Expression) parameterExpression6;
    for (int index = 0; index < paramCount; ++index)
      expressionArray[index + 2] = (Expression) Expression.ArrayIndex((Expression) array, (Expression) Expression.Constant((object) index));
    MethodInfo methodDefinition = new Func<CallSiteBinder, CallSite<Func<object>>>(this.GetOrCreateSite<Func<object>>).GetMethodInfo().GetGenericMethodDefinition();
    return ((Expression<Func<DynamicOperations, CallSiteBinder, object, object[], object>>) ((parameterExpression1, parameterExpression2, parameterExpression3, parameterExpression4) => Expression.Block((IEnumerable<ParameterExpression>) new ParameterExpression[1]
    {
      left
    }, (Expression) Expression.Assign((Expression) left, (Expression) Expression.Call((Expression) instance, methodDefinition.MakeGenericMethod(type), (Expression) parameterExpression5)), (Expression) Expression.Invoke((Expression) Expression.Field((Expression) left, left.Type.GetField("Target")), expressionArray)))).Compile();
  }

  private static Func<DynamicOperations, CallSiteBinder, object, object[], object> GetPregeneratedInvoker(
    int paramCount)
  {
    switch (paramCount)
    {
      case 0:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object>>(binder);
          return site.Target((CallSite) site, target);
        });
      case 1:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0]);
        });
      case 2:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1]);
        });
      case 3:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2]);
        });
      case 4:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3]);
        });
      case 5:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4]);
        });
      case 6:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5]);
        });
      case 7:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
        });
      case 8:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
        });
      case 9:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
        });
      case 10:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
        });
      case 11:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]);
        });
      case 12:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]);
        });
      case 13:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]);
        });
      case 14:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) ((ops, binder, target, args) =>
        {
          CallSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>> site = ops.GetOrCreateSite<Func<CallSite, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>>(binder);
          return site.Target((CallSite) site, target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]);
        });
      default:
        return (Func<DynamicOperations, CallSiteBinder, object, object[], object>) null;
    }
  }

  private class SiteKey : IEquatable<DynamicOperations.SiteKey>
  {
    internal readonly CallSiteBinder SiteBinder;
    private readonly Type _siteType;
    public int HitCount;
    public CallSite Site;

    public SiteKey(Type siteType, CallSiteBinder siteBinder)
    {
      this.SiteBinder = siteBinder;
      this._siteType = siteType;
    }

    public override bool Equals(object obj) => this.Equals(obj as DynamicOperations.SiteKey);

    public override int GetHashCode()
    {
      return this.SiteBinder.GetHashCode() ^ this._siteType.GetHashCode();
    }

    public bool Equals(DynamicOperations.SiteKey other)
    {
      return other != null && other.SiteBinder.Equals((object) this.SiteBinder) && other._siteType == this._siteType;
    }
  }
}
