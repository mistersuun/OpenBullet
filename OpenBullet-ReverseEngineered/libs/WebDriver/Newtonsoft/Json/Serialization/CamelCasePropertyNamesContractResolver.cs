// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

#nullable disable
namespace Newtonsoft.Json.Serialization;

internal class CamelCasePropertyNamesContractResolver : DefaultContractResolver
{
  private static readonly object TypeContractCacheLock = new object();
  private static readonly PropertyNameTable NameTable = new PropertyNameTable();
  private static Dictionary<ResolverContractKey, JsonContract> _contractCache;

  public CamelCasePropertyNamesContractResolver()
  {
    CamelCaseNamingStrategy caseNamingStrategy = new CamelCaseNamingStrategy();
    caseNamingStrategy.ProcessDictionaryKeys = true;
    caseNamingStrategy.OverrideSpecifiedNames = true;
    this.NamingStrategy = (NamingStrategy) caseNamingStrategy;
  }

  public override JsonContract ResolveContract(Type type)
  {
    ResolverContractKey key = !(type == (Type) null) ? new ResolverContractKey(this.GetType(), type) : throw new ArgumentNullException(nameof (type));
    Dictionary<ResolverContractKey, JsonContract> contractCache1 = CamelCasePropertyNamesContractResolver._contractCache;
    JsonContract contract;
    if (contractCache1 == null || !contractCache1.TryGetValue(key, out contract))
    {
      contract = this.CreateContract(type);
      lock (CamelCasePropertyNamesContractResolver.TypeContractCacheLock)
      {
        Dictionary<ResolverContractKey, JsonContract> contractCache2 = CamelCasePropertyNamesContractResolver._contractCache;
        Dictionary<ResolverContractKey, JsonContract> dictionary = contractCache2 != null ? new Dictionary<ResolverContractKey, JsonContract>((IDictionary<ResolverContractKey, JsonContract>) contractCache2) : new Dictionary<ResolverContractKey, JsonContract>();
        dictionary[key] = contract;
        CamelCasePropertyNamesContractResolver._contractCache = dictionary;
      }
    }
    return contract;
  }

  internal override PropertyNameTable GetNameTable()
  {
    return CamelCasePropertyNamesContractResolver.NameTable;
  }
}
