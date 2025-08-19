// Decompiled with JetBrains decompiler
// Type: System.SR
// Assembly: System.Security.Cryptography.X509Certificates, Version=4.1.1.2, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 804EB91C-1ABE-477A-A4AB-D45C97008A32
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Security.Cryptography.X509Certificates.dll

using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace System;

internal static class SR
{
  private static ResourceManager s_resourceManager;
  private const string s_resourcesName = "FxResources.System.Security.Cryptography.X509Certificates.SR";

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

  internal static string Arg_EmptyOrNullArray
  {
    get => SR.GetResourceString(nameof (Arg_EmptyOrNullArray), (string) null);
  }

  internal static string Arg_EmptyOrNullString
  {
    get => SR.GetResourceString(nameof (Arg_EmptyOrNullString), (string) null);
  }

  internal static string Arg_EnumIllegalVal
  {
    get => SR.GetResourceString(nameof (Arg_EnumIllegalVal), (string) null);
  }

  internal static string Arg_InvalidHandle
  {
    get => SR.GetResourceString(nameof (Arg_InvalidHandle), (string) null);
  }

  internal static string Arg_OutOfRange_NeedNonNegNum
  {
    get => SR.GetResourceString(nameof (Arg_OutOfRange_NeedNonNegNum), (string) null);
  }

  internal static string Arg_RankMultiDimNotSupported
  {
    get => SR.GetResourceString(nameof (Arg_RankMultiDimNotSupported), (string) null);
  }

  internal static string Arg_RemoveArgNotFound
  {
    get => SR.GetResourceString(nameof (Arg_RemoveArgNotFound), (string) null);
  }

  internal static string Argument_InvalidFlag
  {
    get => SR.GetResourceString(nameof (Argument_InvalidFlag), (string) null);
  }

  internal static string Argument_InvalidNameType
  {
    get => SR.GetResourceString(nameof (Argument_InvalidNameType), (string) null);
  }

  internal static string Argument_InvalidOffLen
  {
    get => SR.GetResourceString(nameof (Argument_InvalidOffLen), (string) null);
  }

  internal static string Argument_InvalidOidValue
  {
    get => SR.GetResourceString(nameof (Argument_InvalidOidValue), (string) null);
  }

  internal static string ArgumentOutOfRange_Index
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_Index), (string) null);
  }

  internal static string Chain_NoPolicyMatch
  {
    get => SR.GetResourceString(nameof (Chain_NoPolicyMatch), (string) null);
  }

  internal static string Cryptography_Der_Invalid_Encoding
  {
    get => SR.GetResourceString(nameof (Cryptography_Der_Invalid_Encoding), (string) null);
  }

  internal static string Cryptography_InvalidContextHandle
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidContextHandle), (string) null);
  }

  internal static string Cryptography_InvalidHandle
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidHandle), (string) null);
  }

  internal static string Cryptography_InvalidOID
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidOID), (string) null);
  }

  internal static string Cryptography_InvalidStoreHandle
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidStoreHandle), (string) null);
  }

  internal static string Cryptography_Unix_X509_MachineStoresReadOnly
  {
    get
    {
      return SR.GetResourceString(nameof (Cryptography_Unix_X509_MachineStoresReadOnly), (string) null);
    }
  }

  internal static string Cryptography_Unix_X509_MachineStoresRootOnly
  {
    get
    {
      return SR.GetResourceString(nameof (Cryptography_Unix_X509_MachineStoresRootOnly), (string) null);
    }
  }

  internal static string Cryptography_Unix_X509_PropertyNotSettable
  {
    get => SR.GetResourceString(nameof (Cryptography_Unix_X509_PropertyNotSettable), (string) null);
  }

  internal static string Cryptography_Unix_X509_SerializedExport
  {
    get => SR.GetResourceString(nameof (Cryptography_Unix_X509_SerializedExport), (string) null);
  }

  internal static string Cryptography_X509_ExportFailed
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_ExportFailed), (string) null);
  }

  internal static string Cryptography_X509_ExtensionMismatch
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_ExtensionMismatch), (string) null);
  }

  internal static string Cryptography_X509_InvalidContentType
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_InvalidContentType), (string) null);
  }

  internal static string Cryptography_X509_InvalidFindType
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_InvalidFindType), (string) null);
  }

  internal static string Cryptography_X509_InvalidFindValue
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_InvalidFindValue), (string) null);
  }

  internal static string Cryptography_X509_KeyMismatch
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_KeyMismatch), (string) null);
  }

  internal static string Cryptography_X509_PKCS7_NoSigner
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_PKCS7_NoSigner), (string) null);
  }

  internal static string Cryptography_X509_StoreNoFileAvailable
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_StoreNoFileAvailable), (string) null);
  }

  internal static string Cryptography_X509_StoreNotFound
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_StoreNotFound), (string) null);
  }

  internal static string Cryptography_X509_StoreNotOpen
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_StoreNotOpen), (string) null);
  }

  internal static string Cryptography_X509_StoreReadOnly
  {
    get => SR.GetResourceString(nameof (Cryptography_X509_StoreReadOnly), (string) null);
  }

  internal static string InvalidOperation_EnumNotStarted
  {
    get => SR.GetResourceString(nameof (InvalidOperation_EnumNotStarted), (string) null);
  }

  internal static string InvalidPublicKeyInX509
  {
    get => SR.GetResourceString(nameof (InvalidPublicKeyInX509), (string) null);
  }

  internal static string NotSupported_ECDsa_Csp
  {
    get => SR.GetResourceString(nameof (NotSupported_ECDsa_Csp), (string) null);
  }

  internal static string NotSupported_Export_MultiplePrivateCerts
  {
    get => SR.GetResourceString(nameof (NotSupported_Export_MultiplePrivateCerts), (string) null);
  }

  internal static string NotSupported_InvalidKeyImpl
  {
    get => SR.GetResourceString(nameof (NotSupported_InvalidKeyImpl), (string) null);
  }

  internal static string NotSupported_KeyAlgorithm
  {
    get => SR.GetResourceString(nameof (NotSupported_KeyAlgorithm), (string) null);
  }

  internal static string NotSupported_LegacyBasicConstraints
  {
    get => SR.GetResourceString(nameof (NotSupported_LegacyBasicConstraints), (string) null);
  }

  internal static string PersistedFiles_NoHomeDirectory
  {
    get => SR.GetResourceString(nameof (PersistedFiles_NoHomeDirectory), (string) null);
  }

  internal static string Security_InvalidValue
  {
    get => SR.GetResourceString(nameof (Security_InvalidValue), (string) null);
  }

  internal static string Unknown_Error
  {
    get => SR.GetResourceString(nameof (Unknown_Error), (string) null);
  }

  internal static string Cryptography_FileStatusError
  {
    get => SR.GetResourceString(nameof (Cryptography_FileStatusError), (string) null);
  }

  internal static string Cryptography_InvalidDirectoryPermissions
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidDirectoryPermissions), (string) null);
  }

  internal static string Cryptography_OwnerNotCurrentUser
  {
    get => SR.GetResourceString(nameof (Cryptography_OwnerNotCurrentUser), (string) null);
  }

  internal static string Cryptography_InvalidFilePermissions
  {
    get => SR.GetResourceString(nameof (Cryptography_InvalidFilePermissions), (string) null);
  }

  internal static string Cryptography_Invalid_X500Name
  {
    get => SR.GetResourceString(nameof (Cryptography_Invalid_X500Name), (string) null);
  }

  internal static string Cryptography_Invalid_IA5String
  {
    get => SR.GetResourceString(nameof (Cryptography_Invalid_IA5String), (string) null);
  }

  internal static Type ResourceType => typeof (FxResources.System.Security.Cryptography.X509Certificates.SR);
}
