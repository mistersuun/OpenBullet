// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.JsonWebKey
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

[JsonObject]
public class JsonWebKey : SecurityKey
{
  private string _kid;

  public static JsonWebKey Create(string json)
  {
    return !string.IsNullOrEmpty(json) ? new JsonWebKey(json) : throw LogHelper.LogArgumentNullException(nameof (json));
  }

  public JsonWebKey()
  {
  }

  public JsonWebKey(string json)
  {
    if (string.IsNullOrEmpty(json))
      throw LogHelper.LogArgumentNullException(nameof (json));
    try
    {
      LogHelper.LogVerbose("IDX10806: Deserializing json: '{0}' into '{1}'.", (object) json, (object) this);
      JsonConvert.PopulateObject(json, (object) this);
    }
    catch (Exception ex)
    {
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10805: Error deserializing json: '{0}' into '{1}'.", (object) json, (object) this.GetType()), ex));
    }
  }

  [JsonIgnore]
  internal SecurityKey ConvertedSecurityKey { get; set; }

  [JsonExtensionData]
  public virtual IDictionary<string, object> AdditionalData { get; } = (IDictionary<string, object>) new Dictionary<string, object>();

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "alg", Required = Required.Default)]
  public string Alg { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "crv", Required = Required.Default)]
  public string Crv { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "d", Required = Required.Default)]
  public string D { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "dp", Required = Required.Default)]
  public string DP { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "dq", Required = Required.Default)]
  public string DQ { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "e", Required = Required.Default)]
  public string E { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "k", Required = Required.Default)]
  public string K { get; set; }

  [JsonIgnore]
  public override string KeyId
  {
    get => this._kid;
    set => this._kid = value;
  }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "key_ops", Required = Required.Default)]
  public IList<string> KeyOps { get; private set; } = (IList<string>) new List<string>();

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "kid", Required = Required.Default)]
  public string Kid
  {
    get => this._kid;
    set => this._kid = value;
  }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "kty", Required = Required.Default)]
  public string Kty { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "n", Required = Required.Default)]
  public string N { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "oth", Required = Required.Default)]
  public IList<string> Oth { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "p", Required = Required.Default)]
  public string P { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "q", Required = Required.Default)]
  public string Q { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "qi", Required = Required.Default)]
  public string QI { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "use", Required = Required.Default)]
  public string Use { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "x", Required = Required.Default)]
  public string X { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "x5c", Required = Required.Default)]
  public IList<string> X5c { get; private set; } = (IList<string>) new List<string>();

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "x5t", Required = Required.Default)]
  public string X5t { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "x5t#S256", Required = Required.Default)]
  public string X5tS256 { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "x5u", Required = Required.Default)]
  public string X5u { get; set; }

  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore, PropertyName = "y", Required = Required.Default)]
  public string Y { get; set; }

  [JsonIgnore]
  public override int KeySize
  {
    get
    {
      if (this.Kty == "RSA" && !string.IsNullOrEmpty(this.N))
        return Base64UrlEncoder.DecodeBytes(this.N).Length * 8;
      if (this.Kty == "EC" && !string.IsNullOrEmpty(this.X))
        return Base64UrlEncoder.DecodeBytes(this.X).Length * 8;
      return this.Kty == "oct" && !string.IsNullOrEmpty(this.K) ? Base64UrlEncoder.DecodeBytes(this.K).Length * 8 : 0;
    }
  }

  [JsonIgnore]
  public bool HasPrivateKey
  {
    get
    {
      return this.Kty == "RSA" ? this.D != null && this.DP != null && this.DQ != null && this.P != null && this.Q != null && this.QI != null : this.Kty == "EC" && this.D != null;
    }
  }

  public bool ShouldSerializeKeyOps() => this.KeyOps.Count > 0;

  public bool ShouldSerializeX5c() => this.X5c.Count > 0;

  internal RSAParameters CreateRsaParameters()
  {
    if (string.IsNullOrEmpty(this.N))
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10700: {0} is unable to use 'rsaParameters'. {1} is null.", (object) this, (object) "Modulus")));
    if (string.IsNullOrEmpty(this.E))
      throw LogHelper.LogExceptionMessage((Exception) new ArgumentException(LogHelper.FormatInvariant("IDX10700: {0} is unable to use 'rsaParameters'. {1} is null.", (object) this), "Exponent"));
    return new RSAParameters()
    {
      Modulus = Base64UrlEncoder.DecodeBytes(this.N),
      Exponent = Base64UrlEncoder.DecodeBytes(this.E),
      D = string.IsNullOrEmpty(this.D) ? (byte[]) null : Base64UrlEncoder.DecodeBytes(this.D),
      P = string.IsNullOrEmpty(this.P) ? (byte[]) null : Base64UrlEncoder.DecodeBytes(this.P),
      Q = string.IsNullOrEmpty(this.Q) ? (byte[]) null : Base64UrlEncoder.DecodeBytes(this.Q),
      DP = string.IsNullOrEmpty(this.DP) ? (byte[]) null : Base64UrlEncoder.DecodeBytes(this.DP),
      DQ = string.IsNullOrEmpty(this.DQ) ? (byte[]) null : Base64UrlEncoder.DecodeBytes(this.DQ),
      InverseQ = string.IsNullOrEmpty(this.QI) ? (byte[]) null : Base64UrlEncoder.DecodeBytes(this.QI)
    };
  }

  public override string ToString()
  {
    return $"{this.GetType()}, Use: '{this.Use}',  Kid: '{this.Kid}', Kty: '{this.Kty}', InternalId: '{this.InternalId}'.";
  }
}
