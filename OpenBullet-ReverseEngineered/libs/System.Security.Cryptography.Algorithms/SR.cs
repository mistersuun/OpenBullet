// Decompiled with JetBrains decompiler
// Type: System.SR
// Assembly: System.Security.Cryptography.Algorithms, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ECE29162-DF05-4A45-A6F3-DE36C417868A
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Security.Cryptography.Algorithms.dll

using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace System;

internal static class SR
{
  private static ResourceManager s_resourceManager;
  private const string s_resourcesName = "FxResources.System.Security.Cryptography.Algorithms.SR";

  private static ResourceManager ResourceManager
  {
    get
    {
      if (SR.s_resourceManager == null)
        SR.s_resourceManager = new ResourceManager(SR.ResourceType);
      return SR.s_resourceManager;
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static bool UsingResourceKeys() => false;

  internal static string GetResourceString(string resourceKey, string defaultString)
  {
    string str = (string) null;
    try
    {
      str = SR.ResourceManager.GetString(resourceKey);
    }
    catch (MissingManifestResourceException ex)
    {
    }
    return defaultString != null && resourceKey.Equals(str, StringComparison.Ordinal) ? defaultString : str;
  }

  internal static string Format(string resourceFormat, params object[] args)
  {
    if (args == null)
      return resourceFormat;
    return SR.UsingResourceKeys() ? resourceFormat + string.Join(", ", args) : string.Format(resourceFormat, args);
  }

  internal static string Format(string resourceFormat, object p1)
  {
    if (!SR.UsingResourceKeys())
      return string.Format(resourceFormat, p1);
    return string.Join(", ", (object) resourceFormat, p1);
  }

  internal static string Format(string resourceFormat, object p1, object p2)
  {
    if (!SR.UsingResourceKeys())
      return string.Format(resourceFormat, p1, p2);
    return string.Join(", ", (object) resourceFormat, p1, p2);
  }

  internal static string Format(string resourceFormat, object p1, object p2, object p3)
  {
    if (!SR.UsingResourceKeys())
      return string.Format(resourceFormat, p1, p2, p3);
    return string.Join(", ", (object) resourceFormat, p1, p2, p3);
  }

  internal static string ArgumentOutOfRange_NeedNonNegNum
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_NeedNonNegNum), (string) null);
  }

  internal static string ArgumentOutOfRange_NeedPosNum
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_NeedPosNum), (string) null);
  }

  internal static string Argument_InvalidOffLen
  {
    get => SR.GetResourceString(nameof (Argument_InvalidOffLen), (string) null);
  }

  internal static string Argument_InvalidOidValue
  {
    get => SR.GetResourceString(nameof (Argument_InvalidOidValue), (string) null);
  }

  internal static string Argument_InvalidValue
  {
    get => SR.GetResourceString(nameof (Argument_InvalidValue), (string) null);
  }

  internal static string ArgumentNull_Buffer
  {
    get => SR.GetResourceString(nameof (ArgumentNull_Buffer), (string) null);
  }

  internal static string Arg_CryptographyException
  {
    get => SR.GetResourceString(nameof (Arg_CryptographyException), (string) null);
  }

  internal static string Cryptography_CSP_NoPrivateKey
  {
    get => SR.GetResourceString(nameof (Cryptography_CSP_NoPrivateKey), (string) null);
  }

  internal static string Cryptography_HashAlgorithmNameNullOrEmpty
  {
    get => SR.GetResourceString(nameof (Cryptography_HashAlgorithmNameNullOrEmpty), (string) null);
  }

  internal static string Cryptography_CurveNotSupported
  {
    get => SR.GetResourceString(nameof (Cryptography_CurveNotSupported), (string) null);
  }

  internal static string Cryptography_Der_Invalid_Encoding
  {
    get => SR.GetResourceString(nameof (Cryptography_Der_Invalid_Encoding), (string) null);
  }

  internal static string Cryptography_InvalidCurveOid
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidCurveOid), (string) null);
  }

  internal static string Cryptography_InvalidCurveKeyParameters
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidCurveKeyParameters), (string) null);
  }

  internal static string Cryptography_InvalidECCharacteristic2Curve
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidECCharacteristic2Curve), (string) null);
  }

  internal static string Cryptography_InvalidECPrimeCurve
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidECPrimeCurve), (string) null);
  }

  internal static string Cryptography_InvalidECNamedCurve
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidECNamedCurve), (string) null);
  }

  internal static string Cryptography_InvalidKeySize
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidKeySize), (string) null);
  }

  internal static string Cryptography_InvalidKey_SemiWeak
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidKey_SemiWeak), (string) null);
  }

  internal static string Cryptography_InvalidKey_Weak
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidKey_Weak), (string) null);
  }

  internal static string Cryptography_InvalidIVSize
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidIVSize), (string) null);
  }

  internal static string Cryptography_InvalidPadding
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidPadding), (string) null);
  }

  internal static string Cryptography_InvalidRsaParameters
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidRsaParameters), (string) null);
  }

  internal static string Cryptography_InvalidPaddingMode
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidPaddingMode), (string) null);
  }

  internal static string Cryptography_Invalid_IA5String
  {
    get => SR.GetResourceString(nameof (Cryptography_Invalid_IA5String), (string) null);
  }

  internal static string Cryptography_MissingIV
  {
    get => SR.GetResourceString(nameof (Cryptography_MissingIV), (string) null);
  }

  internal static string Cryptography_MustTransformWholeBlock
  {
    get => SR.GetResourceString(nameof (Cryptography_MustTransformWholeBlock), (string) null);
  }

  internal static string Cryptography_NotValidPrivateKey
  {
    get => SR.GetResourceString(nameof (Cryptography_NotValidPrivateKey), (string) null);
  }

  internal static string Cryptography_NotValidPublicOrPrivateKey
  {
    get => SR.GetResourceString(nameof (Cryptography_NotValidPublicOrPrivateKey), (string) null);
  }

  internal static string Cryptography_OpenInvalidHandle
  {
    get => SR.GetResourceString(nameof (Cryptography_OpenInvalidHandle), (string) null);
  }

  internal static string Cryptography_PartialBlock
  {
    get => SR.GetResourceString(nameof (Cryptography_PartialBlock), (string) null);
  }

  internal static string Cryptography_PasswordDerivedBytes_FewBytesSalt
  {
    get
    {
      return SR.GetResourceString(nameof (Cryptography_PasswordDerivedBytes_FewBytesSalt), (string) null);
    }
  }

  internal static string Cryptography_PasswordDerivedBytes_InvalidAlgorithm
  {
    get
    {
      return SR.GetResourceString(nameof (Cryptography_PasswordDerivedBytes_InvalidAlgorithm), (string) null);
    }
  }

  internal static string Cryptography_PasswordDerivedBytes_InvalidIV
  {
    get
    {
      return SR.GetResourceString(nameof (Cryptography_PasswordDerivedBytes_InvalidIV), (string) null);
    }
  }

  internal static string Cryptography_RC2_EKS40
  {
    get => SR.GetResourceString(nameof (Cryptography_RC2_EKS40), (string) null);
  }

  internal static string Cryptography_RC2_EKSKS
  {
    get => SR.GetResourceString(nameof (Cryptography_RC2_EKSKS), (string) null);
  }

  internal static string Cryptography_TransformBeyondEndOfBuffer
  {
    get => SR.GetResourceString(nameof (Cryptography_TransformBeyondEndOfBuffer), (string) null);
  }

  internal static string Cryptography_CipherModeNotSupported
  {
    get => SR.GetResourceString(nameof (Cryptography_CipherModeNotSupported), (string) null);
  }

  internal static string Cryptography_UnknownHashAlgorithm
  {
    get => SR.GetResourceString(nameof (Cryptography_UnknownHashAlgorithm), (string) null);
  }

  internal static string Cryptography_UnknownPaddingMode
  {
    get => SR.GetResourceString(nameof (Cryptography_UnknownPaddingMode), (string) null);
  }

  internal static string Cryptography_UnexpectedTransformTruncation
  {
    get => SR.GetResourceString(nameof (Cryptography_UnexpectedTransformTruncation), (string) null);
  }

  internal static string Cryptography_Unmapped_System_Typed_Error
  {
    get => SR.GetResourceString(nameof (Cryptography_Unmapped_System_Typed_Error), (string) null);
  }

  internal static string Cryptography_UnsupportedPaddingMode
  {
    get => SR.GetResourceString(nameof (Cryptography_UnsupportedPaddingMode), (string) null);
  }

  internal static string NotSupported_SubclassOverride
  {
    get => SR.GetResourceString(nameof (NotSupported_SubclassOverride), (string) null);
  }

  internal static Type ResourceType => typeof (FxResources.System.Security.Cryptography.Algorithms.SR);
}
