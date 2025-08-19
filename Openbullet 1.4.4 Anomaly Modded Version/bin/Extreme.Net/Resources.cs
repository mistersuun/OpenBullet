// Decompiled with JetBrains decompiler
// Type: Extreme.Resources
// Assembly: Extreme.Net, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B85A5720-FE8B-4A1B-9FD2-F7651D37B15B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Extreme.Net.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace Extreme;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
      if (Extreme.Resources.resourceMan == null)
        Extreme.Resources.resourceMan = new ResourceManager("Extreme.Resources", typeof (Extreme.Resources).Assembly);
      return Extreme.Resources.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => Extreme.Resources.resourceCulture;
    set => Extreme.Resources.resourceCulture = value;
  }

  internal static string ArgumentException_CanNotReadOrSeek
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentException_CanNotReadOrSeek), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_EmptyString
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentException_EmptyString), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_HttpRequest_SetNotAvailableHeader
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentException_HttpRequest_SetNotAvailableHeader), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_OnlyAbsoluteUri
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentException_OnlyAbsoluteUri), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentException_WrongPath
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentException_WrongPath), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_CanNotBeGreater
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeGreater), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_CanNotBeLess
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeLess), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_CanNotBeLessOrGreater
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_CanNotBeLessOrGreater), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_StringHelper_MoreLengthString
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_StringHelper_MoreLengthString), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ArgumentOutOfRangeException_StringLengthCanNotBeMore
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ArgumentOutOfRangeException_StringLengthCanNotBeMore), Extreme.Resources.resourceCulture);
    }
  }

  internal static string DirectoryNotFoundException_DirectoryNotFound
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (DirectoryNotFoundException_DirectoryNotFound), Extreme.Resources.resourceCulture);
    }
  }

  internal static string FormatException_ProxyClient_WrongPort
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (FormatException_ProxyClient_WrongPort), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ClientError
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_ClientError), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ConnectTimeout
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_ConnectTimeout), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_Default
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_Default), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedConnect
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_FailedConnect), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedGetHostAddresses
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_FailedGetHostAddresses), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedReceiveMessageBody
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_FailedReceiveMessageBody), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedReceiveResponse
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_FailedReceiveResponse), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedSendRequest
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_FailedSendRequest), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_FailedSslConnect
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_FailedSslConnect), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_LimitRedirections
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_LimitRedirections), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ReceivedEmptyResponse
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_ReceivedEmptyResponse), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_ReceivedWrongResponse
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_ReceivedWrongResponse), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_SeverError
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_SeverError), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WaitDataTimeout
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_WaitDataTimeout), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WrongChunkedBlockLength
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_WrongChunkedBlockLength), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WrongCookie
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_WrongCookie), Extreme.Resources.resourceCulture);
    }
  }

  internal static string HttpException_WrongHeader
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (HttpException_WrongHeader), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ChainProxyClient_NotProxies
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ChainProxyClient_NotProxies), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_HttpResponse_HasError
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_HttpResponse_HasError), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_NotSupportedEncodingFormat
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_NotSupportedEncodingFormat), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongHost
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongHost), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongPassword
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongPassword), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongPort
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongPort), Extreme.Resources.resourceCulture);
    }
  }

  internal static string InvalidOperationException_ProxyClient_WrongUsername
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (InvalidOperationException_ProxyClient_WrongUsername), Extreme.Resources.resourceCulture);
    }
  }

  internal static string NetException_Default
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (NetException_Default), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_CommandError
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_CommandError), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ConnectTimeout
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_ConnectTimeout), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_Default
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_Default), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_Error
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_Error), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_FailedConnect
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_FailedConnect), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_FailedGetHostAddresses
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_FailedGetHostAddresses), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_NotSupportedAddressType
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_NotSupportedAddressType), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ReceivedEmptyResponse
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedEmptyResponse), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ReceivedWrongResponse
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedWrongResponse), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_ReceivedWrongStatusCode
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_ReceivedWrongStatusCode), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_Socks5_FailedAuthOn
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_Socks5_FailedAuthOn), Extreme.Resources.resourceCulture);
    }
  }

  internal static string ProxyException_WaitDataTimeout
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (ProxyException_WaitDataTimeout), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks_UnknownError
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks_UnknownError), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks4_CommandReplyRequestRejectedCannotConnectToIdentd
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedCannotConnectToIdentd), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks4_CommandReplyRequestRejectedDifferentIdentd
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedDifferentIdentd), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks4_CommandReplyRequestRejectedOrFailed
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks4_CommandReplyRequestRejectedOrFailed), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_AuthMethodReplyNoAcceptableMethods
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_AuthMethodReplyNoAcceptableMethods), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyAddressTypeNotSupported
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyAddressTypeNotSupported), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyCommandNotSupported
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyCommandNotSupported), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyConnectionNotAllowedByRuleset
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyConnectionNotAllowedByRuleset), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyConnectionRefused
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyConnectionRefused), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyGeneralSocksServerFailure
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyGeneralSocksServerFailure), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyHostUnreachable
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyHostUnreachable), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyNetworkUnreachable
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyNetworkUnreachable), Extreme.Resources.resourceCulture);
    }
  }

  internal static string Socks5_CommandReplyTTLExpired
  {
    get
    {
      return Extreme.Resources.ResourceManager.GetString(nameof (Socks5_CommandReplyTTLExpired), Extreme.Resources.resourceCulture);
    }
  }
}
