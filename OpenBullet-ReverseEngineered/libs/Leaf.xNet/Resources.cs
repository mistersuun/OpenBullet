// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Resources
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Leaf.xNet;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal Resources()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (Leaf.xNet.Resources.resourceMan == null)
        Leaf.xNet.Resources.resourceMan = new ResourceManager("Leaf.xNet.Resources", typeof (Leaf.xNet.Resources).Assembly);
      return Leaf.xNet.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => Leaf.xNet.Resources.resourceCulture;
    set => Leaf.xNet.Resources.resourceCulture = value;
  }

  internal static string ArgumentException_CanNotReadOrSeek
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_CanNotReadOrSeek), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_EmptyString
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_EmptyString), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_HttpRequest_SetNotAvailableHeader
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_HttpRequest_SetNotAvailableHeader), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_OnlyAbsoluteUri
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_OnlyAbsoluteUri), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_WrongPath
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentException_WrongPath), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_CanNotBeGreater
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeGreater), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_CanNotBeLess
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeLess), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_CanNotBeLessOrGreater
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeLessOrGreater), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_StringHelper_MoreLengthString
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_StringHelper_MoreLengthString), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_StringLengthCanNotBeMore
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_StringLengthCanNotBeMore), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string CookieStorage_SaveToFile_FileAlreadyExists
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (CookieStorage_SaveToFile_FileAlreadyExists), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string DirectoryNotFoundException_DirectoryNotFound
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (DirectoryNotFoundException_DirectoryNotFound), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string FormatException_ProxyClient_WrongPort
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (FormatException_ProxyClient_WrongPort), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ClientError
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_ClientError), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ConnectTimeout
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_ConnectTimeout), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_Default
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_Default), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedConnect
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedConnect), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedGetHostAddresses
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedGetHostAddresses), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedReceiveMessageBody
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedReceiveMessageBody), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedReceiveResponse
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedReceiveResponse), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedSendRequest
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedSendRequest), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedSslConnect
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_FailedSslConnect), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_LimitRedirections
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_LimitRedirections), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ReceivedEmptyResponse
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_ReceivedEmptyResponse), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ReceivedWrongResponse
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_ReceivedWrongResponse), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_SeverError
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_SeverError), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WaitDataTimeout
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_WaitDataTimeout), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WrongChunkedBlockLength
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_WrongChunkedBlockLength), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WrongCookie
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_WrongCookie), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WrongHeader
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (HttpException_WrongHeader), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_HttpResponse_HasError
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_HttpResponse_HasError), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_NotSupportedEncodingFormat
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_NotSupportedEncodingFormat), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongHost
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongHost), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongPassword
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongPassword), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongPort
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongPort), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongUsername
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongUsername), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string NetException_Default
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (NetException_Default), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_CommandError
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_CommandError), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ConnectTimeout
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ConnectTimeout), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_Default
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_Default), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_Error
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_Error), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_FailedConnect
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_FailedConnect), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_FailedGetHostAddresses
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_FailedGetHostAddresses), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_NotSupportedAddressType
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_NotSupportedAddressType), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ReceivedEmptyResponse
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedEmptyResponse), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ReceivedWrongResponse
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedWrongResponse), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ReceivedWrongStatusCode
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedWrongStatusCode), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_Socks5_FailedAuthOn
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_Socks5_FailedAuthOn), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_WaitDataTimeout
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (ProxyException_WaitDataTimeout), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks_UnknownError
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks_UnknownError), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks4_CommandReplyRequestRejectedCannotConnectToIdentd
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedCannotConnectToIdentd), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks4_CommandReplyRequestRejectedDifferentIdentd
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedDifferentIdentd), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks4_CommandReplyRequestRejectedOrFailed
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedOrFailed), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_AuthMethodReplyNoAcceptableMethods
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_AuthMethodReplyNoAcceptableMethods), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyAddressTypeNotSupported
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyAddressTypeNotSupported), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyCommandNotSupported
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyCommandNotSupported), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyConnectionNotAllowedByRuleset
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyConnectionNotAllowedByRuleset), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyConnectionRefused
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyConnectionRefused), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyGeneralSocksServerFailure
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyGeneralSocksServerFailure), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyHostUnreachable
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyHostUnreachable), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyNetworkUnreachable
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyNetworkUnreachable), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyTTLExpired
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyTTLExpired), Leaf.xNet.Resources.resourceCulture);
    }
  }

  internal static string StringExtensions_Substrings_Invalid_Index
  {
    get
    {
      return Leaf.xNet.Resources.ResourceManager.GetString(nameof (StringExtensions_Substrings_Invalid_Index), Leaf.xNet.Resources.resourceCulture);
    }
  }
}
