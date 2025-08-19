// Decompiled with JetBrains decompiler
// Type: System.SR
// Assembly: System.Net.Http, Version=4.1.1.3, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 084B071E-1637-4B3F-B7CD-6CEF28A6E4AE
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\System.Net.Http.dll

using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace System;

internal static class SR
{
  private static ResourceManager s_resourceManager;
  private const string s_resourcesName = "FxResources.System.Net.Http.SR";

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

  internal static string net_securityprotocolnotsupported
  {
    get => SR.GetResourceString(nameof (net_securityprotocolnotsupported), (string) null);
  }

  internal static string net_http_httpmethod_format_error
  {
    get => SR.GetResourceString(nameof (net_http_httpmethod_format_error), (string) null);
  }

  internal static string net_http_reasonphrase_format_error
  {
    get => SR.GetResourceString(nameof (net_http_reasonphrase_format_error), (string) null);
  }

  internal static string net_http_copyto_array_too_small
  {
    get => SR.GetResourceString(nameof (net_http_copyto_array_too_small), (string) null);
  }

  internal static string net_http_headers_not_found
  {
    get => SR.GetResourceString(nameof (net_http_headers_not_found), (string) null);
  }

  internal static string net_http_headers_single_value_header
  {
    get => SR.GetResourceString(nameof (net_http_headers_single_value_header), (string) null);
  }

  internal static string net_http_headers_invalid_header_name
  {
    get => SR.GetResourceString(nameof (net_http_headers_invalid_header_name), (string) null);
  }

  internal static string net_http_headers_invalid_value
  {
    get => SR.GetResourceString(nameof (net_http_headers_invalid_value), (string) null);
  }

  internal static string net_http_headers_not_allowed_header_name
  {
    get => SR.GetResourceString(nameof (net_http_headers_not_allowed_header_name), (string) null);
  }

  internal static string net_http_headers_invalid_host_header
  {
    get => SR.GetResourceString(nameof (net_http_headers_invalid_host_header), (string) null);
  }

  internal static string net_http_headers_invalid_from_header
  {
    get => SR.GetResourceString(nameof (net_http_headers_invalid_from_header), (string) null);
  }

  internal static string net_http_headers_invalid_etag_name
  {
    get => SR.GetResourceString(nameof (net_http_headers_invalid_etag_name), (string) null);
  }

  internal static string net_http_headers_invalid_range
  {
    get => SR.GetResourceString(nameof (net_http_headers_invalid_range), (string) null);
  }

  internal static string net_http_headers_no_newlines
  {
    get => SR.GetResourceString(nameof (net_http_headers_no_newlines), (string) null);
  }

  internal static string net_http_content_buffersize_exceeded
  {
    get => SR.GetResourceString(nameof (net_http_content_buffersize_exceeded), (string) null);
  }

  internal static string net_http_content_no_task_returned
  {
    get => SR.GetResourceString(nameof (net_http_content_no_task_returned), (string) null);
  }

  internal static string net_http_content_stream_already_read
  {
    get => SR.GetResourceString(nameof (net_http_content_stream_already_read), (string) null);
  }

  internal static string net_http_content_readonly_stream
  {
    get => SR.GetResourceString(nameof (net_http_content_readonly_stream), (string) null);
  }

  internal static string net_http_content_invalid_charset
  {
    get => SR.GetResourceString(nameof (net_http_content_invalid_charset), (string) null);
  }

  internal static string net_http_content_stream_copy_error
  {
    get => SR.GetResourceString(nameof (net_http_content_stream_copy_error), (string) null);
  }

  internal static string net_http_argument_empty_string
  {
    get => SR.GetResourceString(nameof (net_http_argument_empty_string), (string) null);
  }

  internal static string net_http_client_request_already_sent
  {
    get => SR.GetResourceString(nameof (net_http_client_request_already_sent), (string) null);
  }

  internal static string net_http_operation_started
  {
    get => SR.GetResourceString(nameof (net_http_operation_started), (string) null);
  }

  internal static string net_http_client_execution_error
  {
    get => SR.GetResourceString(nameof (net_http_client_execution_error), (string) null);
  }

  internal static string net_http_client_absolute_baseaddress_required
  {
    get
    {
      return SR.GetResourceString(nameof (net_http_client_absolute_baseaddress_required), (string) null);
    }
  }

  internal static string net_http_client_invalid_requesturi
  {
    get => SR.GetResourceString(nameof (net_http_client_invalid_requesturi), (string) null);
  }

  internal static string net_http_client_http_baseaddress_required
  {
    get => SR.GetResourceString(nameof (net_http_client_http_baseaddress_required), (string) null);
  }

  internal static string net_http_client_send_error
  {
    get => SR.GetResourceString(nameof (net_http_client_send_error), (string) null);
  }

  internal static string net_http_client_send_canceled
  {
    get => SR.GetResourceString(nameof (net_http_client_send_canceled), (string) null);
  }

  internal static string event_ClientSendCompleted
  {
    get => SR.GetResourceString(nameof (event_ClientSendCompleted), (string) null);
  }

  internal static string net_http_parser_invalid_base64_string
  {
    get => SR.GetResourceString(nameof (net_http_parser_invalid_base64_string), (string) null);
  }

  internal static string net_http_handler_noresponse
  {
    get => SR.GetResourceString(nameof (net_http_handler_noresponse), (string) null);
  }

  internal static string net_http_handler_norequest
  {
    get => SR.GetResourceString(nameof (net_http_handler_norequest), (string) null);
  }

  internal static string net_http_message_not_success_statuscode
  {
    get => SR.GetResourceString(nameof (net_http_message_not_success_statuscode), (string) null);
  }

  internal static string net_http_content_field_too_long
  {
    get => SR.GetResourceString(nameof (net_http_content_field_too_long), (string) null);
  }

  internal static string event_HeadersInvalidValue
  {
    get => SR.GetResourceString(nameof (event_HeadersInvalidValue), (string) null);
  }

  internal static string net_http_log_headers_no_newlines
  {
    get => SR.GetResourceString(nameof (net_http_log_headers_no_newlines), (string) null);
  }

  internal static string net_http_log_headers_invalid_quality
  {
    get => SR.GetResourceString(nameof (net_http_log_headers_invalid_quality), (string) null);
  }

  internal static string net_http_log_headers_wrong_email_format
  {
    get => SR.GetResourceString(nameof (net_http_log_headers_wrong_email_format), (string) null);
  }

  internal static string net_http_log_content_no_task_returned_copytoasync
  {
    get
    {
      return SR.GetResourceString(nameof (net_http_log_content_no_task_returned_copytoasync), (string) null);
    }
  }

  internal static string event_ContentNull
  {
    get => SR.GetResourceString(nameof (event_ContentNull), (string) null);
  }

  internal static string net_http_handler_not_assigned
  {
    get => SR.GetResourceString(nameof (net_http_handler_not_assigned), (string) null);
  }

  internal static string net_http_invalid_enable_first
  {
    get => SR.GetResourceString(nameof (net_http_invalid_enable_first), (string) null);
  }

  internal static string net_http_content_buffersize_limit
  {
    get => SR.GetResourceString(nameof (net_http_content_buffersize_limit), (string) null);
  }

  internal static string net_http_value_not_supported
  {
    get => SR.GetResourceString(nameof (net_http_value_not_supported), (string) null);
  }

  internal static string net_http_io_read
  {
    get => SR.GetResourceString(nameof (net_http_io_read), (string) null);
  }

  internal static string net_http_io_read_incomplete
  {
    get => SR.GetResourceString(nameof (net_http_io_read_incomplete), (string) null);
  }

  internal static string net_http_io_write
  {
    get => SR.GetResourceString(nameof (net_http_io_write), (string) null);
  }

  internal static string net_http_chunked_not_allowed_with_empty_content
  {
    get
    {
      return SR.GetResourceString(nameof (net_http_chunked_not_allowed_with_empty_content), (string) null);
    }
  }

  internal static string net_http_chunked_not_allowed_with_content_length
  {
    get
    {
      return SR.GetResourceString(nameof (net_http_chunked_not_allowed_with_content_length), (string) null);
    }
  }

  internal static string net_http_invalid_cookiecontainer
  {
    get => SR.GetResourceString(nameof (net_http_invalid_cookiecontainer), (string) null);
  }

  internal static string net_http_invalid_proxyusepolicy
  {
    get => SR.GetResourceString(nameof (net_http_invalid_proxyusepolicy), (string) null);
  }

  internal static string net_http_invalid_proxy
  {
    get => SR.GetResourceString(nameof (net_http_invalid_proxy), (string) null);
  }

  internal static string net_http_handler_nocontentlength
  {
    get => SR.GetResourceString(nameof (net_http_handler_nocontentlength), (string) null);
  }

  internal static string net_http_value_must_be_greater_than
  {
    get => SR.GetResourceString(nameof (net_http_value_must_be_greater_than), (string) null);
  }

  internal static string MailHeaderFieldInvalidCharacter
  {
    get => SR.GetResourceString(nameof (MailHeaderFieldInvalidCharacter), (string) null);
  }

  internal static string MailAddressInvalidFormat
  {
    get => SR.GetResourceString(nameof (MailAddressInvalidFormat), (string) null);
  }

  internal static string MailHeaderFieldMalformedHeader
  {
    get => SR.GetResourceString(nameof (MailHeaderFieldMalformedHeader), (string) null);
  }

  internal static string InvalidHeaderName
  {
    get => SR.GetResourceString(nameof (InvalidHeaderName), (string) null);
  }

  internal static string net_cookie_attribute
  {
    get => SR.GetResourceString(nameof (net_cookie_attribute), (string) null);
  }

  internal static string net_http_invalid_cookiecontainer_unix
  {
    get => SR.GetResourceString(nameof (net_http_invalid_cookiecontainer_unix), (string) null);
  }

  internal static string net_http_unix_invalid_certcallback_option
  {
    get => SR.GetResourceString(nameof (net_http_unix_invalid_certcallback_option), (string) null);
  }

  internal static string net_http_unix_invalid_credential
  {
    get => SR.GetResourceString(nameof (net_http_unix_invalid_credential), (string) null);
  }

  internal static string net_http_unix_https_support_unavailable_libcurl
  {
    get
    {
      return SR.GetResourceString(nameof (net_http_unix_https_support_unavailable_libcurl), (string) null);
    }
  }

  internal static string ArgumentOutOfRange_FileLengthTooBig
  {
    get => SR.GetResourceString(nameof (ArgumentOutOfRange_FileLengthTooBig), (string) null);
  }

  internal static string IO_FileExists_Name
  {
    get => SR.GetResourceString(nameof (IO_FileExists_Name), (string) null);
  }

  internal static string IO_FileNotFound
  {
    get => SR.GetResourceString(nameof (IO_FileNotFound), (string) null);
  }

  internal static string IO_FileNotFound_FileName
  {
    get => SR.GetResourceString(nameof (IO_FileNotFound_FileName), (string) null);
  }

  internal static string IO_PathNotFound_NoPathName
  {
    get => SR.GetResourceString(nameof (IO_PathNotFound_NoPathName), (string) null);
  }

  internal static string IO_PathNotFound_Path
  {
    get => SR.GetResourceString(nameof (IO_PathNotFound_Path), (string) null);
  }

  internal static string IO_PathTooLong
  {
    get => SR.GetResourceString(nameof (IO_PathTooLong), (string) null);
  }

  internal static string IO_SharingViolation_File
  {
    get => SR.GetResourceString(nameof (IO_SharingViolation_File), (string) null);
  }

  internal static string IO_SharingViolation_NoFileName
  {
    get => SR.GetResourceString(nameof (IO_SharingViolation_NoFileName), (string) null);
  }

  internal static string UnauthorizedAccess_IODenied_NoPathName
  {
    get => SR.GetResourceString(nameof (UnauthorizedAccess_IODenied_NoPathName), (string) null);
  }

  internal static string UnauthorizedAccess_IODenied_Path
  {
    get => SR.GetResourceString(nameof (UnauthorizedAccess_IODenied_Path), (string) null);
  }

  internal static string net_http_content_no_concurrent_reads
  {
    get => SR.GetResourceString(nameof (net_http_content_no_concurrent_reads), (string) null);
  }

  internal static string net_http_username_empty_string
  {
    get => SR.GetResourceString(nameof (net_http_username_empty_string), (string) null);
  }

  internal static string net_http_no_concurrent_io_allowed
  {
    get => SR.GetResourceString(nameof (net_http_no_concurrent_io_allowed), (string) null);
  }

  internal static string net_http_unix_invalid_response
  {
    get => SR.GetResourceString(nameof (net_http_unix_invalid_response), (string) null);
  }

  internal static string net_http_unix_handler_disposed
  {
    get => SR.GetResourceString(nameof (net_http_unix_handler_disposed), (string) null);
  }

  internal static string net_http_buffer_insufficient_length
  {
    get => SR.GetResourceString(nameof (net_http_buffer_insufficient_length), (string) null);
  }

  internal static string net_http_response_headers_exceeded_length
  {
    get => SR.GetResourceString(nameof (net_http_response_headers_exceeded_length), (string) null);
  }

  internal static string net_http_libcurl_callback_notsupported
  {
    get => SR.GetResourceString(nameof (net_http_libcurl_callback_notsupported), (string) null);
  }

  internal static string net_http_libcurl_clientcerts_notsupported
  {
    get => SR.GetResourceString(nameof (net_http_libcurl_clientcerts_notsupported), (string) null);
  }

  internal static string net_http_libcurl_revocation_notsupported
  {
    get => SR.GetResourceString(nameof (net_http_libcurl_revocation_notsupported), (string) null);
  }

  internal static Type ResourceType => typeof (FxResources.System.Net.Http.SR);
}
