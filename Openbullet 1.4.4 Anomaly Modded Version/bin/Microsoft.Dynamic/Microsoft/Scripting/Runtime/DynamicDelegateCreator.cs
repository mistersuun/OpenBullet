// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Runtime.DynamicDelegateCreator
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Dynamic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Runtime;

public class DynamicDelegateCreator
{
  private readonly LanguageContext _languageContext;
  private Publisher<DelegateSignatureInfo, DelegateInfo> _dynamicDelegateCache = new Publisher<DelegateSignatureInfo, DelegateInfo>();

  public DynamicDelegateCreator(LanguageContext languageContext)
  {
    ContractUtils.RequiresNotNull((object) languageContext, nameof (languageContext));
    this._languageContext = languageContext;
  }

  public Delegate GetDelegate(object callableObject, Type delegateType)
  {
    ContractUtils.RequiresNotNull((object) delegateType, nameof (delegateType));
    switch (callableObject)
    {
      case Delegate @delegate:
        return delegateType.IsAssignableFrom(@delegate.GetType()) ? @delegate : throw ScriptingRuntimeHelpers.SimpleTypeError($"Cannot cast {@delegate.GetType()} to {delegateType}.");
      case IDynamicMetaObjectProvider _:
        MethodInfo method;
        if (!typeof (Delegate).IsAssignableFrom(delegateType) || (method = delegateType.GetMethod("Invoke")) == (MethodInfo) null)
          throw ScriptingRuntimeHelpers.SimpleTypeError("A specific delegate type is required.");
        Delegate forDynamicObject = this.GetOrCreateDelegateForDynamicObject(callableObject, delegateType, method);
        if ((object) forDynamicObject != null)
          return forDynamicObject;
        break;
    }
    throw ScriptingRuntimeHelpers.SimpleTypeError("Object is not callable.");
  }

  public Delegate GetOrCreateDelegateForDynamicObject(
    object dynamicObject,
    Type delegateType,
    MethodInfo invoke)
  {
    DelegateSignatureInfo signatureInfo = new DelegateSignatureInfo(invoke);
    return this._dynamicDelegateCache.GetOrCreateValue(signatureInfo, (Func<DelegateInfo>) (() => new DelegateInfo(this._languageContext, signatureInfo.ReturnType, signatureInfo.ParameterTypes))).CreateDelegate(delegateType, dynamicObject);
  }
}
