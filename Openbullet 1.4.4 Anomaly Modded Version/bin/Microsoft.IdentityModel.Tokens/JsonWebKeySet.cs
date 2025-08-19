// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.JsonWebKeySet
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

[JsonObject]
public class JsonWebKeySet
{
  [DefaultValue(true)]
  public static bool DefaultSkipUnresolvedJsonWebKeys = true;

  public static JsonWebKeySet Create(string json)
  {
    return !string.IsNullOrEmpty(json) ? new JsonWebKeySet(json) : throw LogHelper.LogArgumentNullException(nameof (json));
  }

  public JsonWebKeySet()
  {
  }

  public JsonWebKeySet(string json)
    : this(json, (JsonSerializerSettings) null)
  {
  }

  public JsonWebKeySet(string json, JsonSerializerSettings jsonSerializerSettings)
  {
    if (string.IsNullOrEmpty(json))
      throw LogHelper.LogArgumentNullException(nameof (json));
    try
    {
      LogHelper.LogVerbose("IDX10806: Deserializing json: '{0}' into '{1}'.", (object) json, (object) this);
      if (jsonSerializerSettings != null)
        JsonConvert.PopulateObject(json, (object) this, jsonSerializerSettings);
      else
        JsonConvert.PopulateObject(json, (object) this);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10805: Error deserializing json: '{0}' into '{1}'.", (object) json, (object) this.GetType()), ex));
    }
  }

  [JsonExtensionData]
  public virtual IDictionary<string, object> AdditionalData { get; } = (IDictionary<string, object>) new Dictionary<string, object>();

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "keys", Required = Required.Default)]
  public IList<JsonWebKey> Keys { get; private set; } = (IList<JsonWebKey>) new List<JsonWebKey>();

  [DefaultValue(true)]
  public bool SkipUnresolvedJsonWebKeys { get; set; } = JsonWebKeySet.DefaultSkipUnresolvedJsonWebKeys;

  public IList<SecurityKey> GetSigningKeys()
  {
    List<SecurityKey> signingKeys = new List<SecurityKey>();
    foreach (JsonWebKey key1 in (IEnumerable<JsonWebKey>) this.Keys)
    {
      if (!string.IsNullOrEmpty(key1.Use) && !key1.Use.Equals("sig", StringComparison.Ordinal))
      {
        LogHelper.LogInformation(LogHelper.FormatInvariant("IDX10808: The 'use' parameter of a JsonWebKey: '{0}' was expected to be 'sig' or empty, but was '{1}'.", (object) key1, (object) key1.Use));
        if (!this.SkipUnresolvedJsonWebKeys)
          signingKeys.Add((SecurityKey) key1);
      }
      else if ("RSA".Equals(key1.Kty, StringComparison.Ordinal))
      {
        bool flag = true;
        if ((key1.X5c == null || key1.X5c.Count == 0) && string.IsNullOrEmpty(key1.E) && string.IsNullOrEmpty(key1.N))
        {
          flag = false;
        }
        else
        {
          if (key1.X5c != null && key1.X5c.Count != 0)
          {
            SecurityKey key2;
            if (JsonWebKeyConverter.TryConvertToX509SecurityKey(key1, out key2))
              signingKeys.Add(key2);
            else
              flag = false;
          }
          if (!string.IsNullOrEmpty(key1.E) && !string.IsNullOrEmpty(key1.N))
          {
            SecurityKey key3;
            if (JsonWebKeyConverter.TryCreateToRsaSecurityKey(key1, out key3))
              signingKeys.Add(key3);
            else
              flag = false;
          }
        }
        if (!flag && !this.SkipUnresolvedJsonWebKeys)
          signingKeys.Add((SecurityKey) key1);
      }
      else if ("EC".Equals(key1.Kty, StringComparison.Ordinal))
      {
        SecurityKey key4;
        if (JsonWebKeyConverter.TryConvertToECDsaSecurityKey(key1, out key4))
          signingKeys.Add(key4);
        else if (!this.SkipUnresolvedJsonWebKeys)
          signingKeys.Add((SecurityKey) key1);
      }
      else
      {
        LogHelper.LogInformation(LogHelper.FormatInvariant("IDX10810: Unable to convert the JsonWebKey: '{0}' to a X509SecurityKey, RsaSecurityKey or ECDSASecurityKey.", (object) key1));
        if (!this.SkipUnresolvedJsonWebKeys)
          signingKeys.Add((SecurityKey) key1);
      }
    }
    return (IList<SecurityKey>) signingKeys;
  }
}
