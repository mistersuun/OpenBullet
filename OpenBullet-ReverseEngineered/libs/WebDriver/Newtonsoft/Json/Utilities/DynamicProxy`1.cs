// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.DynamicProxy`1
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;
using System.Dynamic;

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal class DynamicProxy<T>
{
  public virtual IEnumerable<string> GetDynamicMemberNames(T instance)
  {
    return (IEnumerable<string>) CollectionUtils.ArrayEmpty<string>();
  }

  public virtual bool TryBinaryOperation(
    T instance,
    BinaryOperationBinder binder,
    object arg,
    out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TryConvert(T instance, ConvertBinder binder, out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TryCreateInstance(
    T instance,
    CreateInstanceBinder binder,
    object[] args,
    out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TryDeleteIndex(T instance, DeleteIndexBinder binder, object[] indexes)
  {
    return false;
  }

  public virtual bool TryDeleteMember(T instance, DeleteMemberBinder binder) => false;

  public virtual bool TryGetIndex(
    T instance,
    GetIndexBinder binder,
    object[] indexes,
    out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TryGetMember(T instance, GetMemberBinder binder, out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TryInvoke(T instance, InvokeBinder binder, object[] args, out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TryInvokeMember(
    T instance,
    InvokeMemberBinder binder,
    object[] args,
    out object result)
  {
    result = (object) null;
    return false;
  }

  public virtual bool TrySetIndex(
    T instance,
    SetIndexBinder binder,
    object[] indexes,
    object value)
  {
    return false;
  }

  public virtual bool TrySetMember(T instance, SetMemberBinder binder, object value) => false;

  public virtual bool TryUnaryOperation(T instance, UnaryOperationBinder binder, out object result)
  {
    result = (object) null;
    return false;
  }
}
