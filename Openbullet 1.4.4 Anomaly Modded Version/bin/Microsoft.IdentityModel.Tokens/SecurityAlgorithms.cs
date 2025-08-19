﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Tokens.SecurityAlgorithms
// Assembly: Microsoft.IdentityModel.Tokens, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20B56468-1C95-4BEE-81F0-0C47AA115548
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Tokens.dll

#nullable disable
namespace Microsoft.IdentityModel.Tokens;

public static class SecurityAlgorithms
{
  public const string Aes128Encryption = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
  public const string Aes192Encryption = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";
  public const string Aes256Encryption = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";
  public const string DesEncryption = "http://www.w3.org/2001/04/xmlenc#des-cbc";
  public const string Aes128KeyWrap = "http://www.w3.org/2001/04/xmlenc#kw-aes128";
  public const string Aes192KeyWrap = "http://www.w3.org/2001/04/xmlenc#kw-aes192";
  public const string Aes256KeyWrap = "http://www.w3.org/2001/04/xmlenc#kw-aes256";
  public const string RsaV15KeyWrap = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
  public const string Ripemd160Digest = "http://www.w3.org/2001/04/xmlenc#ripemd160";
  public const string RsaOaepKeyWrap = "http://www.w3.org/2001/04/xmlenc#rsa-oaep";
  public const string Aes128KW = "A128KW";
  public const string Aes256KW = "A256KW";
  public const string RsaPKCS1 = "RSA1_5";
  public const string RsaOAEP = "RSA-OAEP";
  public const string ExclusiveC14n = "http://www.w3.org/2001/10/xml-exc-c14n#";
  public const string ExclusiveC14nWithComments = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
  public const string EnvelopedSignature = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
  public const string Sha256Digest = "http://www.w3.org/2001/04/xmlenc#sha256";
  public const string Sha384Digest = "http://www.w3.org/2001/04/xmldsig-more#sha384";
  public const string Sha512Digest = "http://www.w3.org/2001/04/xmlenc#sha512";
  public const string Sha256 = "SHA256";
  public const string Sha384 = "SHA384";
  public const string Sha512 = "SHA512";
  public const string EcdsaSha256Signature = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256";
  public const string EcdsaSha384Signature = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384";
  public const string EcdsaSha512Signature = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512";
  public const string HmacSha256Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
  public const string HmacSha384Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
  public const string HmacSha512Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
  public const string RsaSha256Signature = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
  public const string RsaSha384Signature = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";
  public const string RsaSha512Signature = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
  public const string RsaSsaPssSha256Signature = "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1";
  public const string RsaSsaPssSha384Signature = "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1";
  public const string RsaSsaPssSha512Signature = "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1";
  public const string EcdsaSha256 = "ES256";
  public const string EcdsaSha384 = "ES384";
  public const string EcdsaSha512 = "ES512";
  public const string HmacSha256 = "HS256";
  public const string HmacSha384 = "HS384";
  public const string HmacSha512 = "HS512";
  public const string None = "none";
  public const string RsaSha256 = "RS256";
  public const string RsaSha384 = "RS384";
  public const string RsaSha512 = "RS512";
  public const string RsaSsaPssSha256 = "PS256";
  public const string RsaSsaPssSha384 = "PS384";
  public const string RsaSsaPssSha512 = "PS512";
  public const string Aes128CbcHmacSha256 = "A128CBC-HS256";
  public const string Aes192CbcHmacSha384 = "A192CBC-HS384";
  public const string Aes256CbcHmacSha512 = "A256CBC-HS512";
  internal const string DefaultAsymmetricKeyWrapAlgorithm = "http://www.w3.org/2001/04/xmlenc#rsa-oaep";
  internal const string DefaultSymmetricEncryptionAlgorithm = "A128CBC-HS256";
}
