// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ModuleDictionaryStorage
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Compiler;
using IronPython.Runtime.Types;
using Microsoft.Scripting.Actions;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime;

internal class ModuleDictionaryStorage : GlobalDictionaryStorage
{
  private Type _type;
  private bool _cleared;
  private static readonly Dictionary<string, PythonGlobal> _emptyGlobalDict = new Dictionary<string, PythonGlobal>(0);
  private static readonly PythonGlobal[] _emptyGlobals = new PythonGlobal[0];

  public ModuleDictionaryStorage(Type moduleType)
    : base(ModuleDictionaryStorage._emptyGlobalDict, ModuleDictionaryStorage._emptyGlobals)
  {
    this._type = moduleType;
  }

  public ModuleDictionaryStorage(Type moduleType, Dictionary<string, PythonGlobal> globals)
    : base(globals, ModuleDictionaryStorage._emptyGlobals)
  {
    this._type = moduleType;
  }

  public virtual BuiltinPythonModule Instance => (BuiltinPythonModule) null;

  public override bool Remove(ref DictionaryStorage storage, object key)
  {
    if (!(key is string name))
      return base.Remove(ref storage, key);
    bool flag = base.Remove(ref storage, key);
    if (this.TryGetLazyValue(name, out object _))
    {
      this.Add(key, (object) Uninitialized.Instance);
      flag = true;
    }
    return flag;
  }

  protected virtual void LazyAdd(object name, object value) => this.Add(name, value);

  public override bool Contains(object key) => this.TryGetValue(key, out object _);

  public override void Clear(ref DictionaryStorage storage)
  {
    this._cleared = true;
    base.Clear(ref storage);
  }

  public override List<KeyValuePair<object, object>> GetItems()
  {
    List<KeyValuePair<object, object>> items = new List<KeyValuePair<object, object>>();
    foreach (KeyValuePair<object, object> keyValuePair in base.GetItems())
    {
      if (keyValuePair.Value != Uninitialized.Instance)
        items.Add(keyValuePair);
    }
    foreach (MemberInfo member in this._type.GetMembers())
    {
      object obj;
      if (!base.Contains((object) member.Name) && this.TryGetLazyValue(member.Name, out obj))
        items.Add(new KeyValuePair<object, object>((object) member.Name, obj));
    }
    return items;
  }

  public override int Count
  {
    get
    {
      this.GetItems();
      return base.Count;
    }
  }

  private bool TryGetLazyValue(string name, out object value)
  {
    return this.TryGetLazyValue(name, true, out value);
  }

  private bool TryGetLazyValue(string name, bool publish, out object value)
  {
    if (!this._cleared)
    {
      MemberInfo[] memberInfoArray = ModuleDictionaryStorage.NonHiddenMembers(this.GetMember(name));
      if (memberInfoArray.Length != 0)
      {
        switch (memberInfoArray[0].MemberType)
        {
          case MemberTypes.Field:
            value = ((FieldInfo) memberInfoArray[0]).IsStatic ? ((FieldInfo) memberInfoArray[0]).GetValue((object) null) : throw new InvalidOperationException("instance field declared on module.  Fields should stored as PythonGlobals, should be static readonly, or marked as PythonHidden.");
            if (publish)
              this.LazyAdd((object) name, value);
            return true;
          case MemberTypes.Method:
            if (!((MethodBase) memberInfoArray[0]).IsSpecialName)
            {
              MethodInfo[] infos = new MethodInfo[memberInfoArray.Length];
              FunctionType ft = FunctionType.AlwaysVisible | FunctionType.ModuleMethod;
              for (int index = 0; index < memberInfoArray.Length; ++index)
              {
                MethodInfo methodInfo = (MethodInfo) memberInfoArray[index];
                if (methodInfo.IsStatic)
                  ft |= FunctionType.Function;
                else
                  ft |= FunctionType.Method;
                infos[index] = methodInfo;
              }
              BuiltinFunction builtinFunction = BuiltinFunction.MakeMethod(name, (MethodBase[]) infos, memberInfoArray[0].DeclaringType, ft);
              value = (ft & FunctionType.Method) == FunctionType.None || this.Instance == null ? (object) builtinFunction : (object) builtinFunction.BindToInstance((object) this.Instance);
              if (publish)
                this.LazyAdd((object) name, value);
              return true;
            }
            break;
          case MemberTypes.Property:
            PropertyInfo propertyInfo = (PropertyInfo) memberInfoArray[0];
            MethodInfo methodInfo1 = propertyInfo.GetGetMethod();
            if ((object) methodInfo1 == null)
              methodInfo1 = propertyInfo.GetSetMethod();
            if (!methodInfo1.IsStatic)
              throw new InvalidOperationException("instance property declared on module.  Propreties should be declared as static, marked as PythonHidden, or you should use a PythonGlobal.");
            value = ((PropertyInfo) memberInfoArray[0]).GetValue((object) null, ArrayUtils.EmptyObjects);
            if (publish)
              this.LazyAdd((object) name, value);
            return true;
          case MemberTypes.NestedType:
            if (memberInfoArray.Length == 1)
            {
              value = (object) DynamicHelpers.GetPythonTypeFromType((Type) memberInfoArray[0]);
            }
            else
            {
              TypeTracker existingTypeEntity = (TypeTracker) MemberTracker.FromMemberInfo(memberInfoArray[0]);
              for (int index = 1; index < memberInfoArray.Length; ++index)
                existingTypeEntity = TypeGroup.UpdateTypeEntity(existingTypeEntity, (TypeTracker) MemberTracker.FromMemberInfo(memberInfoArray[index]));
              value = (object) existingTypeEntity;
            }
            if (publish)
              this.LazyAdd((object) name, value);
            return true;
        }
      }
    }
    value = (object) null;
    return false;
  }

  private static MemberInfo[] NonHiddenMembers(MemberInfo[] members)
  {
    List<MemberInfo> memberInfoList = new List<MemberInfo>(members.Length);
    foreach (MemberInfo member in members)
    {
      if (!PythonHiddenAttribute.IsHidden(member))
        memberInfoList.Add(member);
    }
    return memberInfoList.ToArray();
  }

  private MemberInfo[] GetMember(string name)
  {
    return this._type.GetMember(name, (BindingFlags) (18 | (this.Instance == null ? 8 : 12)));
  }

  public override bool TryGetValue(object key, out object value)
  {
    if (base.TryGetValue(key, out value))
      return value != Uninitialized.Instance;
    return key is string name && this.TryGetLazyValue(name, out value);
  }

  public virtual void Reload()
  {
    foreach (KeyValuePair<object, object> keyValuePair in base.GetItems())
    {
      if (keyValuePair.Value == Uninitialized.Instance)
        this.Remove(keyValuePair.Key);
      else if (keyValuePair.Key is string key && this.GetMember(key).Length != 0)
        this.Remove(keyValuePair.Key);
    }
  }
}
