// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Logging.LogHelper
// Assembly: Microsoft.IdentityModel.Logging, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C4950808-8A1B-4796-AB1F-9647EE1EB5BA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Logging.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;

#nullable disable
namespace Microsoft.IdentityModel.Logging;

public class LogHelper
{
  private static readonly List<string> CustomExceptionTypePrefixes = new List<string>()
  {
    "Microsoft.IdentityModel.Protocols",
    "Microsoft.IdentityModel.Tokens.SecurityToken",
    "Microsoft.IdentityModel.Tokens.Saml",
    "Microsoft.IdentityModel.Xml"
  };

  public static ArgumentNullException LogArgumentNullException(string argument)
  {
    return LogHelper.LogArgumentException<ArgumentNullException>((EventLevel) 2, argument, "IDX10000: The parameter '{0}' cannot be a 'null' or an empty object.", (object) argument);
  }

  public static T LogException<T>(string message) where T : Exception
  {
    return LogHelper.LogException<T>((EventLevel) 2, (Exception) null, message, (object[]) null);
  }

  public static T LogArgumentException<T>(string argumentName, string message) where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>((EventLevel) 2, argumentName, (Exception) null, message, (object[]) null);
  }

  public static T LogException<T>(string format, params object[] args) where T : Exception
  {
    return LogHelper.LogException<T>((EventLevel) 2, (Exception) null, format, args);
  }

  public static T LogArgumentException<T>(string argumentName, string format, params object[] args) where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>((EventLevel) 2, argumentName, (Exception) null, format, args);
  }

  public static T LogException<T>(Exception innerException, string message) where T : Exception
  {
    return LogHelper.LogException<T>((EventLevel) 2, innerException, message, (object[]) null);
  }

  public static T LogArgumentException<T>(
    string argumentName,
    Exception innerException,
    string message)
    where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>((EventLevel) 2, argumentName, innerException, message, (object[]) null);
  }

  public static T LogException<T>(Exception innerException, string format, params object[] args) where T : Exception
  {
    return LogHelper.LogException<T>((EventLevel) 2, innerException, format, args);
  }

  public static T LogArgumentException<T>(
    string argumentName,
    Exception innerException,
    string format,
    params object[] args)
    where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>((EventLevel) 2, argumentName, innerException, format, args);
  }

  public static T LogException<T>(EventLevel eventLevel, string message) where T : Exception
  {
    return LogHelper.LogException<T>(eventLevel, (Exception) null, message, (object[]) null);
  }

  public static T LogArgumentException<T>(
    EventLevel eventLevel,
    string argumentName,
    string message)
    where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>(eventLevel, argumentName, (Exception) null, message, (object[]) null);
  }

  public static T LogException<T>(EventLevel eventLevel, string format, params object[] args) where T : Exception
  {
    return LogHelper.LogException<T>(eventLevel, (Exception) null, format, args);
  }

  public static T LogArgumentException<T>(
    EventLevel eventLevel,
    string argumentName,
    string format,
    params object[] args)
    where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>(eventLevel, argumentName, (Exception) null, format, args);
  }

  public static T LogException<T>(EventLevel eventLevel, Exception innerException, string message) where T : Exception
  {
    return LogHelper.LogException<T>(eventLevel, innerException, message, (object[]) null);
  }

  public static T LogArgumentException<T>(
    EventLevel eventLevel,
    string argumentName,
    Exception innerException,
    string message)
    where T : ArgumentException
  {
    return LogHelper.LogArgumentException<T>(eventLevel, argumentName, innerException, message, (object[]) null);
  }

  public static T LogException<T>(
    EventLevel eventLevel,
    Exception innerException,
    string format,
    params object[] args)
    where T : Exception
  {
    return LogHelper.LogExceptionImpl<T>(eventLevel, (string) null, innerException, format, args);
  }

  public static T LogArgumentException<T>(
    EventLevel eventLevel,
    string argumentName,
    Exception innerException,
    string format,
    params object[] args)
    where T : ArgumentException
  {
    return LogHelper.LogExceptionImpl<T>(eventLevel, argumentName, innerException, format, args);
  }

  public static Exception LogExceptionMessage(Exception exception)
  {
    return LogHelper.LogExceptionMessage((EventLevel) 2, exception);
  }

  public static Exception LogExceptionMessage(EventLevel eventLevel, Exception exception)
  {
    if (IdentityModelEventSource.Logger.IsEnabled() && IdentityModelEventSource.Logger.LogLevel >= eventLevel)
      IdentityModelEventSource.Logger.Write(eventLevel, exception.InnerException, exception.Message);
    return exception;
  }

  public static void LogInformation(string message, params object[] args)
  {
    if (!IdentityModelEventSource.Logger.IsEnabled())
      return;
    IdentityModelEventSource.Logger.WriteInformation(message, args);
  }

  public static void LogVerbose(string message, params object[] args)
  {
    if (!IdentityModelEventSource.Logger.IsEnabled())
      return;
    IdentityModelEventSource.Logger.WriteVerbose(message, args);
  }

  public static void LogWarning(string message, params object[] args)
  {
    if (!IdentityModelEventSource.Logger.IsEnabled())
      return;
    IdentityModelEventSource.Logger.WriteWarning(message, args);
  }

  private static T LogExceptionImpl<T>(
    EventLevel eventLevel,
    string argumentName,
    Exception innerException,
    string format,
    params object[] args)
    where T : Exception
  {
    string message = args == null ? format : string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
    if (IdentityModelEventSource.Logger.IsEnabled() && IdentityModelEventSource.Logger.LogLevel >= eventLevel)
      IdentityModelEventSource.Logger.Write(eventLevel, innerException, message);
    return innerException != null ? (string.IsNullOrEmpty(argumentName) ? (T) Activator.CreateInstance(typeof (T), (object) message, (object) innerException) : (T) Activator.CreateInstance(typeof (T), (object) argumentName, (object) message, (object) innerException)) : (string.IsNullOrEmpty(argumentName) ? (T) Activator.CreateInstance(typeof (T), (object) message) : (T) Activator.CreateInstance(typeof (T), (object) argumentName, (object) message));
  }

  public static string FormatInvariant(string format, params object[] args)
  {
    return !IdentityModelEventSource.ShowPII ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object[]) ((IEnumerable<object>) args).Select<object, string>(new Func<object, string>(LogHelper.RemovePII)).ToArray<string>()) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args);
  }

  private static string RemovePII(object arg)
  {
    if (!(arg is Exception))
      return IdentityModelEventSource.HiddenPIIString;
    Exception ex = arg as Exception;
    return LogHelper.IsCustomException(ex) ? ex.ToString() : ex.GetType().ToString();
  }

  internal static bool IsCustomException(Exception ex)
  {
    return LogHelper.CustomExceptionTypePrefixes.Exists((Predicate<string>) (e => ex.GetType().FullName.Contains(e)));
  }
}
