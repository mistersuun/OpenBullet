﻿// Decompiled with JetBrains decompiler
// Type: XamlGeneratedNamespace.GeneratedInternalTypeHelper
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;

#nullable disable
namespace XamlGeneratedNamespace;

[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerNonUserCode]
public sealed class GeneratedInternalTypeHelper : InternalTypeHelper
{
  protected override object CreateInstance(Type type, CultureInfo culture)
  {
    return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, (object[]) null, culture);
  }

  protected override object GetPropertyValue(
    PropertyInfo propertyInfo,
    object target,
    CultureInfo culture)
  {
    return propertyInfo.GetValue(target, BindingFlags.Default, (Binder) null, (object[]) null, culture);
  }

  protected override void SetPropertyValue(
    PropertyInfo propertyInfo,
    object target,
    object value,
    CultureInfo culture)
  {
    propertyInfo.SetValue(target, value, BindingFlags.Default, (Binder) null, (object[]) null, culture);
  }

  protected override Delegate CreateDelegate(Type delegateType, object target, string handler)
  {
    return (Delegate) target.GetType().InvokeMember("_CreateDelegate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder) null, target, new object[2]
    {
      (object) delegateType,
      (object) handler
    }, (CultureInfo) null);
  }

  protected override void AddEventHandler(EventInfo eventInfo, object target, Delegate handler)
  {
    eventInfo.AddEventHandler(target, handler);
  }
}
