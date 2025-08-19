// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.JsonDynamicContract
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class JsonDynamicContract : JsonContainerContract
{
  private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>> _callSiteGetters = new ThreadSafeStore<string, CallSite<Func<CallSite, object, object>>>(new Func<string, CallSite<Func<CallSite, object, object>>>(JsonDynamicContract.CreateCallSiteGetter));
  private readonly ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>> _callSiteSetters = new ThreadSafeStore<string, CallSite<Func<CallSite, object, object, object>>>(new Func<string, CallSite<Func<CallSite, object, object, object>>>(JsonDynamicContract.CreateCallSiteSetter));

  public JsonPropertyCollection Properties { get; }

  public Func<string, string> PropertyNameResolver { get; set; }

  private static CallSite<Func<CallSite, object, object>> CreateCallSiteGetter(string name)
  {
    return CallSite<Func<CallSite, object, object>>.Create((CallSiteBinder) new NoThrowGetBinderMember((GetMemberBinder) DynamicUtils.BinderWrapper.GetMember(name, typeof (DynamicUtils))));
  }

  private static CallSite<Func<CallSite, object, object, object>> CreateCallSiteSetter(string name)
  {
    return CallSite<Func<CallSite, object, object, object>>.Create((CallSiteBinder) new NoThrowSetBinderMember((SetMemberBinder) DynamicUtils.BinderWrapper.SetMember(name, typeof (DynamicUtils))));
  }

  public JsonDynamicContract(Type underlyingType)
    : base(underlyingType)
  {
    this.ContractType = JsonContractType.Dynamic;
    this.Properties = new JsonPropertyCollection(this.UnderlyingType);
  }

  internal bool TryGetMember(
    IDynamicMetaObjectProvider dynamicProvider,
    string name,
    out object value)
  {
    ValidationUtils.ArgumentNotNull((object) dynamicProvider, nameof (dynamicProvider));
    CallSite<Func<CallSite, object, object>> callSite = this._callSiteGetters.Get(name);
    object obj = callSite.Target((CallSite) callSite, (object) dynamicProvider);
    if (obj != NoThrowExpressionVisitor.ErrorResult)
    {
      value = obj;
      return true;
    }
    value = (object) null;
    return false;
  }

  internal bool TrySetMember(IDynamicMetaObjectProvider dynamicProvider, string name, object value)
  {
    ValidationUtils.ArgumentNotNull((object) dynamicProvider, nameof (dynamicProvider));
    CallSite<Func<CallSite, object, object, object>> callSite = this._callSiteSetters.Get(name);
    return callSite.Target((CallSite) callSite, (object) dynamicProvider, value) != NoThrowExpressionVisitor.ErrorResult;
  }
}
