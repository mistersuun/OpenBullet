// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonProperty
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

[PythonType("property")]
public class PythonProperty : PythonTypeDataSlot
{
  private object _fget;
  private object _fset;
  private object _fdel;
  private object _doc;

  public PythonProperty()
  {
  }

  public PythonProperty(params object[] args)
  {
  }

  public PythonProperty([ParamDictionary] IDictionary<object, object> dict, params object[] args)
  {
  }

  public void __init__(object fget = null, object fset = null, object fdel = null, object doc = null)
  {
    this._fget = fget;
    this._fset = fset;
    this._fdel = fdel;
    this._doc = doc;
    if (!(this.GetType() != typeof (PythonProperty)) || !(this._fget is PythonFunction))
      return;
    PythonDictionary dictionary = UserTypeOps.GetDictionary((IPythonObject) this);
    if (dictionary == null)
      throw PythonOps.AttributeError("{0} object has no __doc__ attribute", (object) PythonTypeOps.GetName((object) this));
    dictionary[(object) "__doc__"] = ((PythonFunction) this._fget).__doc__;
  }

  internal override bool TryGetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    out object value)
  {
    value = this.__get__(context, instance, PythonOps.ToPythonType(owner));
    return true;
  }

  internal override bool GetAlwaysSucceeds => true;

  internal override bool TrySetValue(
    CodeContext context,
    object instance,
    PythonType owner,
    object value)
  {
    if (instance == null)
      return false;
    this.__set__(context, instance, value);
    return true;
  }

  internal override bool TryDeleteValue(CodeContext context, object instance, PythonType owner)
  {
    if (instance == null)
      return false;
    this.__delete__(context, instance);
    return true;
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static object Get__doc__(CodeContext context, PythonProperty self)
  {
    if (self._doc == null && PythonOps.HasAttr(context, self._fget, "__doc__"))
      return PythonOps.GetBoundAttr(context, self._fget, "__doc__");
    if (self._doc == null)
      Console.WriteLine("No attribute __doc__");
    return self._doc;
  }

  [PropertyMethod]
  [WrapperDescriptor]
  [SpecialName]
  public static void Set__doc__(PythonProperty self, object value)
  {
    throw PythonOps.TypeError("readonly attribute");
  }

  public object fdel
  {
    get => this._fdel;
    set => throw PythonOps.TypeError("readonly attribute");
  }

  public object fset
  {
    get => this._fset;
    set => throw PythonOps.TypeError("readonly attribute");
  }

  public object fget
  {
    get => this._fget;
    set => throw PythonOps.TypeError("readonly attribute");
  }

  public override object __get__(CodeContext context, object instance, object owner = null)
  {
    if (instance == null)
      return (object) this;
    if (this.fget == null)
      throw PythonOps.UnreadableProperty();
    CallSite<Func<CallSite, CodeContext, object, object, object>> propertyGetSite = context.LanguageContext.PropertyGetSite;
    return propertyGetSite.Target((CallSite) propertyGetSite, context, this.fget, instance);
  }

  public override void __set__(CodeContext context, object instance, object value)
  {
    if (this.fset == null)
      throw PythonOps.UnsetableProperty();
    CallSite<Func<CallSite, CodeContext, object, object, object, object>> propertySetSite = context.LanguageContext.PropertySetSite;
    object obj = propertySetSite.Target((CallSite) propertySetSite, context, this.fset, instance, value);
  }

  public override void __delete__(CodeContext context, object instance)
  {
    if (this.fdel == null)
      throw PythonOps.UndeletableProperty();
    CallSite<Func<CallSite, CodeContext, object, object, object>> propertyDeleteSite = context.LanguageContext.PropertyDeleteSite;
    object obj = propertyDeleteSite.Target((CallSite) propertyDeleteSite, context, this.fdel, instance);
  }

  public PythonProperty getter(object fget)
  {
    PythonProperty pythonProperty = new PythonProperty();
    pythonProperty.__init__(fget, this._fset, this._fdel, this._doc);
    return pythonProperty;
  }

  public PythonProperty setter(object fset)
  {
    PythonProperty pythonProperty = new PythonProperty();
    pythonProperty.__init__(this._fget, fset, this._fdel, this._doc);
    return pythonProperty;
  }

  public PythonProperty deleter(object fdel)
  {
    PythonProperty pythonProperty = new PythonProperty();
    pythonProperty.__init__(this._fget, this._fset, fdel, this._doc);
    return pythonProperty;
  }
}
