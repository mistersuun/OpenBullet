// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ObjectOperations
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Remoting;

#nullable disable
namespace Microsoft.Scripting.Hosting;

public sealed class ObjectOperations : MarshalByRefObject
{
  private readonly DynamicOperations _ops;

  internal ObjectOperations(DynamicOperations ops, ScriptEngine engine)
  {
    this._ops = ops;
    this.Engine = engine;
  }

  public ScriptEngine Engine { get; }

  public bool IsCallable(object obj) => this._ops.IsCallable(obj);

  public object Invoke(object obj, params object[] parameters) => this._ops.Invoke(obj, parameters);

  public object InvokeMember(object obj, string memberName, params object[] parameters)
  {
    return this._ops.InvokeMember(obj, memberName, parameters);
  }

  public object CreateInstance(object obj, params object[] parameters)
  {
    return this._ops.CreateInstance(obj, parameters);
  }

  public object GetMember(object obj, string name) => this._ops.GetMember(obj, name);

  public T GetMember<T>(object obj, string name) => this._ops.GetMember<T>(obj, name);

  public bool TryGetMember(object obj, string name, out object value)
  {
    return this._ops.TryGetMember(obj, name, out value);
  }

  public bool ContainsMember(object obj, string name) => this._ops.ContainsMember(obj, name);

  public void RemoveMember(object obj, string name) => this._ops.RemoveMember(obj, name);

  public void SetMember(object obj, string name, object value)
  {
    this._ops.SetMember(obj, name, value);
  }

  public void SetMember<T>(object obj, string name, T value)
  {
    this._ops.SetMember<T>(obj, name, value);
  }

  public object GetMember(object obj, string name, bool ignoreCase)
  {
    return this._ops.GetMember(obj, name, ignoreCase);
  }

  public T GetMember<T>(object obj, string name, bool ignoreCase)
  {
    return this._ops.GetMember<T>(obj, name, ignoreCase);
  }

  public bool TryGetMember(object obj, string name, bool ignoreCase, out object value)
  {
    return this._ops.TryGetMember(obj, name, ignoreCase, out value);
  }

  public bool ContainsMember(object obj, string name, bool ignoreCase)
  {
    return this._ops.ContainsMember(obj, name, ignoreCase);
  }

  public void RemoveMember(object obj, string name, bool ignoreCase)
  {
    this._ops.RemoveMember(obj, name, ignoreCase);
  }

  public void SetMember(object obj, string name, object value, bool ignoreCase)
  {
    this._ops.SetMember(obj, name, value, ignoreCase);
  }

  public void SetMember<T>(object obj, string name, T value, bool ignoreCase)
  {
    this._ops.SetMember<T>(obj, name, value, ignoreCase);
  }

  public T ConvertTo<T>(object obj) => this._ops.ConvertTo<T>(obj);

  public object ConvertTo(object obj, Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    return this._ops.ConvertTo(obj, type);
  }

  public bool TryConvertTo<T>(object obj, out T result)
  {
    return this._ops.TryConvertTo<T>(obj, out result);
  }

  public bool TryConvertTo(object obj, Type type, out object result)
  {
    return this._ops.TryConvertTo(obj, type, out result);
  }

  public T ExplicitConvertTo<T>(object obj) => this._ops.ExplicitConvertTo<T>(obj);

  public object ExplicitConvertTo(object obj, Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    return this._ops.ExplicitConvertTo(obj, type);
  }

  public bool TryExplicitConvertTo<T>(object obj, out T result)
  {
    return this._ops.TryExplicitConvertTo<T>(obj, out result);
  }

  public bool TryExplicitConvertTo(object obj, Type type, out object result)
  {
    return this._ops.TryExplicitConvertTo(obj, type, out result);
  }

  public T ImplicitConvertTo<T>(object obj) => this._ops.ImplicitConvertTo<T>(obj);

  public object ImplicitConvertTo(object obj, Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    return this._ops.ImplicitConvertTo(obj, type);
  }

  public bool TryImplicitConvertTo<T>(object obj, out T result)
  {
    return this._ops.TryImplicitConvertTo<T>(obj, out result);
  }

  public bool TryImplicitConvertTo(object obj, Type type, out object result)
  {
    return this._ops.TryImplicitConvertTo(obj, type, out result);
  }

  public object DoOperation(ExpressionType operation, object target)
  {
    return this._ops.DoOperation<object, object>(operation, target);
  }

  public TResult DoOperation<TTarget, TResult>(ExpressionType operation, TTarget target)
  {
    return this._ops.DoOperation<TTarget, TResult>(operation, target);
  }

  public object DoOperation(ExpressionType operation, object target, object other)
  {
    return this._ops.DoOperation<object, object, object>(operation, target, other);
  }

  public TResult DoOperation<TTarget, TOther, TResult>(
    ExpressionType operation,
    TTarget target,
    TOther other)
  {
    return this._ops.DoOperation<TTarget, TOther, TResult>(operation, target, other);
  }

  public object Add(object self, object other) => this.DoOperation(ExpressionType.Add, self, other);

  public object Subtract(object self, object other)
  {
    return this.DoOperation(ExpressionType.Subtract, self, other);
  }

  public object Power(object self, object other)
  {
    return this.DoOperation(ExpressionType.Power, self, other);
  }

  public object Multiply(object self, object other)
  {
    return this.DoOperation(ExpressionType.Multiply, self, other);
  }

  public object Divide(object self, object other)
  {
    return this.DoOperation(ExpressionType.Divide, self, other);
  }

  public object Modulo(object self, object other)
  {
    return this.DoOperation(ExpressionType.Modulo, self, other);
  }

  public object LeftShift(object self, object other)
  {
    return this.DoOperation(ExpressionType.LeftShift, self, other);
  }

  public object RightShift(object self, object other)
  {
    return this.DoOperation(ExpressionType.RightShift, self, other);
  }

  public object BitwiseAnd(object self, object other)
  {
    return this.DoOperation(ExpressionType.And, self, other);
  }

  public object BitwiseOr(object self, object other)
  {
    return this.DoOperation(ExpressionType.Or, self, other);
  }

  public object ExclusiveOr(object self, object other)
  {
    return this.DoOperation(ExpressionType.ExclusiveOr, self, other);
  }

  public bool LessThan(object self, object other)
  {
    return this.ConvertTo<bool>(this._ops.DoOperation<object, object, object>(ExpressionType.LessThan, self, other));
  }

  public bool GreaterThan(object self, object other)
  {
    return this.ConvertTo<bool>(this._ops.DoOperation<object, object, object>(ExpressionType.GreaterThan, self, other));
  }

  public bool LessThanOrEqual(object self, object other)
  {
    return this.ConvertTo<bool>(this._ops.DoOperation<object, object, object>(ExpressionType.LessThanOrEqual, self, other));
  }

  public bool GreaterThanOrEqual(object self, object other)
  {
    return this.ConvertTo<bool>(this._ops.DoOperation<object, object, object>(ExpressionType.GreaterThanOrEqual, self, other));
  }

  public bool Equal(object self, object other)
  {
    return this.ConvertTo<bool>(this._ops.DoOperation<object, object, object>(ExpressionType.Equal, self, other));
  }

  public bool NotEqual(object self, object other)
  {
    return this.ConvertTo<bool>(this._ops.DoOperation<object, object, object>(ExpressionType.NotEqual, self, other));
  }

  [Obsolete("Use Format method instead.")]
  public string GetCodeRepresentation(object obj) => obj.ToString();

  public string Format(object obj) => this._ops.Format(obj);

  public IList<string> GetMemberNames(object obj) => this._ops.GetMemberNames(obj);

  public string GetDocumentation(object obj) => this._ops.GetDocumentation(obj);

  public IList<string> GetCallSignatures(object obj) => this._ops.GetCallSignatures(obj);

  public bool IsCallable([NotNull] ObjectHandle obj)
  {
    return this.IsCallable(ObjectOperations.GetLocalObject(obj));
  }

  public ObjectHandle Invoke([NotNull] ObjectHandle obj, params ObjectHandle[] parameters)
  {
    ContractUtils.RequiresNotNull((object) parameters, nameof (parameters));
    return new ObjectHandle(this.Invoke(ObjectOperations.GetLocalObject(obj), ObjectOperations.GetLocalObjects(parameters)));
  }

  public ObjectHandle Invoke([NotNull] ObjectHandle obj, params object[] parameters)
  {
    return new ObjectHandle(this.Invoke(ObjectOperations.GetLocalObject(obj), parameters));
  }

  public ObjectHandle CreateInstance([NotNull] ObjectHandle obj, [NotNull] params ObjectHandle[] parameters)
  {
    return new ObjectHandle(this.CreateInstance(ObjectOperations.GetLocalObject(obj), ObjectOperations.GetLocalObjects(parameters)));
  }

  public ObjectHandle CreateInstance([NotNull] ObjectHandle obj, params object[] parameters)
  {
    return new ObjectHandle(this.CreateInstance(ObjectOperations.GetLocalObject(obj), parameters));
  }

  public void SetMember([NotNull] ObjectHandle obj, string name, [NotNull] ObjectHandle value)
  {
    this.SetMember(ObjectOperations.GetLocalObject(obj), name, ObjectOperations.GetLocalObject(value));
  }

  public void SetMember<T>([NotNull] ObjectHandle obj, string name, T value)
  {
    this.SetMember<T>(ObjectOperations.GetLocalObject(obj), name, value);
  }

  public ObjectHandle GetMember([NotNull] ObjectHandle obj, string name)
  {
    return new ObjectHandle(this.GetMember(ObjectOperations.GetLocalObject(obj), name));
  }

  public T GetMember<T>([NotNull] ObjectHandle obj, string name)
  {
    return this.GetMember<T>(ObjectOperations.GetLocalObject(obj), name);
  }

  public bool TryGetMember([NotNull] ObjectHandle obj, string name, out ObjectHandle value)
  {
    object o;
    if (this.TryGetMember(ObjectOperations.GetLocalObject(obj), name, out o))
    {
      value = new ObjectHandle(o);
      return true;
    }
    value = (ObjectHandle) null;
    return false;
  }

  public bool ContainsMember([NotNull] ObjectHandle obj, string name)
  {
    return this.ContainsMember(ObjectOperations.GetLocalObject(obj), name);
  }

  public void RemoveMember([NotNull] ObjectHandle obj, string name)
  {
    this.RemoveMember(ObjectOperations.GetLocalObject(obj), name);
  }

  public ObjectHandle ConvertTo<T>([NotNull] ObjectHandle obj)
  {
    return new ObjectHandle((object) this.ConvertTo<T>(ObjectOperations.GetLocalObject(obj)));
  }

  public ObjectHandle ConvertTo([NotNull] ObjectHandle obj, Type type)
  {
    return new ObjectHandle(this.ConvertTo(ObjectOperations.GetLocalObject(obj), type));
  }

  public bool TryConvertTo<T>([NotNull] ObjectHandle obj, out ObjectHandle result)
  {
    T result1;
    if (this.TryConvertTo<T>(ObjectOperations.GetLocalObject(obj), out result1))
    {
      result = new ObjectHandle((object) result1);
      return true;
    }
    result = (ObjectHandle) null;
    return false;
  }

  public bool TryConvertTo([NotNull] ObjectHandle obj, Type type, out ObjectHandle result)
  {
    object result1;
    if (this.TryConvertTo(ObjectOperations.GetLocalObject(obj), type, out result1))
    {
      result = new ObjectHandle(result1);
      return true;
    }
    result = (ObjectHandle) null;
    return false;
  }

  public ObjectHandle ExplicitConvertTo<T>([NotNull] ObjectHandle obj)
  {
    return new ObjectHandle((object) this._ops.ExplicitConvertTo<T>(ObjectOperations.GetLocalObject(obj)));
  }

  public ObjectHandle ExplicitConvertTo([NotNull] ObjectHandle obj, Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    return new ObjectHandle(this._ops.ExplicitConvertTo(ObjectOperations.GetLocalObject(obj), type));
  }

  public bool TryExplicitConvertTo<T>([NotNull] ObjectHandle obj, out ObjectHandle result)
  {
    bool flag = this._ops.TryExplicitConvertTo<T>(ObjectOperations.GetLocalObject(obj), out T _);
    result = flag ? new ObjectHandle((object) obj) : (ObjectHandle) null;
    return flag;
  }

  public bool TryExplicitConvertTo([NotNull] ObjectHandle obj, Type type, out ObjectHandle result)
  {
    bool flag = this._ops.TryExplicitConvertTo(ObjectOperations.GetLocalObject(obj), type, out object _);
    result = flag ? new ObjectHandle((object) obj) : (ObjectHandle) null;
    return flag;
  }

  public ObjectHandle ImplicitConvertTo<T>([NotNull] ObjectHandle obj)
  {
    return new ObjectHandle((object) this._ops.ImplicitConvertTo<T>(ObjectOperations.GetLocalObject(obj)));
  }

  public ObjectHandle ImplicitConvertTo([NotNull] ObjectHandle obj, Type type)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    return new ObjectHandle(this._ops.ImplicitConvertTo(ObjectOperations.GetLocalObject(obj), type));
  }

  public bool TryImplicitConvertTo<T>([NotNull] ObjectHandle obj, out ObjectHandle result)
  {
    bool flag = this._ops.TryImplicitConvertTo<T>(ObjectOperations.GetLocalObject(obj), out T _);
    result = flag ? new ObjectHandle((object) obj) : (ObjectHandle) null;
    return flag;
  }

  public bool TryImplicitConvertTo([NotNull] ObjectHandle obj, Type type, out ObjectHandle result)
  {
    bool flag = this._ops.TryImplicitConvertTo(ObjectOperations.GetLocalObject(obj), type, out object _);
    result = flag ? new ObjectHandle((object) obj) : (ObjectHandle) null;
    return flag;
  }

  public T Unwrap<T>([NotNull] ObjectHandle obj)
  {
    return this.ConvertTo<T>(ObjectOperations.GetLocalObject(obj));
  }

  public ObjectHandle DoOperation(ExpressionType op, [NotNull] ObjectHandle target)
  {
    return new ObjectHandle(this.DoOperation(op, ObjectOperations.GetLocalObject(target)));
  }

  public ObjectHandle DoOperation(ExpressionType op, ObjectHandle target, ObjectHandle other)
  {
    return new ObjectHandle(this.DoOperation(op, ObjectOperations.GetLocalObject(target), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle Add([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.Add(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle Subtract([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.Subtract(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle Power([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.Power(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle Multiply([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.Multiply(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle Divide([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.Divide(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle Modulo([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.Modulo(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle LeftShift([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.LeftShift(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle RightShift([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.RightShift(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle BitwiseAnd([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.BitwiseAnd(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle BitwiseOr([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.BitwiseOr(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public ObjectHandle ExclusiveOr([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return new ObjectHandle(this.ExclusiveOr(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other)));
  }

  public bool LessThan([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return this.LessThan(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other));
  }

  public bool GreaterThan([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return this.GreaterThan(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other));
  }

  public bool LessThanOrEqual([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return this.LessThanOrEqual(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other));
  }

  public bool GreaterThanOrEqual([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return this.GreaterThanOrEqual(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other));
  }

  public bool Equal([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return this.Equal(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other));
  }

  public bool NotEqual([NotNull] ObjectHandle self, [NotNull] ObjectHandle other)
  {
    return this.NotEqual(ObjectOperations.GetLocalObject(self), ObjectOperations.GetLocalObject(other));
  }

  public string Format([NotNull] ObjectHandle obj)
  {
    return this.Format(ObjectOperations.GetLocalObject(obj));
  }

  public IList<string> GetMemberNames([NotNull] ObjectHandle obj)
  {
    return this.GetMemberNames(ObjectOperations.GetLocalObject(obj));
  }

  public string GetDocumentation([NotNull] ObjectHandle obj)
  {
    return this.GetDocumentation(ObjectOperations.GetLocalObject(obj));
  }

  public IList<string> GetCallSignatures([NotNull] ObjectHandle obj)
  {
    return this.GetCallSignatures(ObjectOperations.GetLocalObject(obj));
  }

  private static object GetLocalObject([NotNull] ObjectHandle obj)
  {
    ContractUtils.RequiresNotNull((object) obj, nameof (obj));
    return obj.Unwrap();
  }

  private static object[] GetLocalObjects(ObjectHandle[] ohs)
  {
    object[] localObjects = new object[ohs.Length];
    for (int index = 0; index < localObjects.Length; ++index)
      localObjects[index] = ObjectOperations.GetLocalObject(ohs[index]);
    return localObjects;
  }

  public override object InitializeLifetimeService() => (object) null;
}
