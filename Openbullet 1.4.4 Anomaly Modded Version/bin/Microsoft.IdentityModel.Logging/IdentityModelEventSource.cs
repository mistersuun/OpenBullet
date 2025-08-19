// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Logging.IdentityModelEventSource
// Assembly: Microsoft.IdentityModel.Logging, Version=5.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C4950808-8A1B-4796-AB1F-9647EE1EB5BA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.IdentityModel.Logging.dll

using System;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Microsoft.IdentityModel.Logging;

[EventSource(Name = "Microsoft.IdentityModel.EventSource")]
public class IdentityModelEventSource : EventSource
{
  private static string _versionLogMessage = "Library version: {0}.";
  private static string _dateLogMessage = "Date: {0}.";
  private static string _piiOffLogMessage = "PII (personally identifiable information) logging is currently turned off. Set IdentityModelEventSource.ShowPII to 'true' to view the full details of exceptions.";
  private static string _piiOnLogMessage = "PII (personally identifiable information) logging is currently turned on. Set IdentityModelEventSource.ShowPII to 'false' to hide PII from log messages.";

  static IdentityModelEventSource()
  {
    IdentityModelEventSource.Logger = new IdentityModelEventSource();
  }

  private IdentityModelEventSource() => this.LogLevel = (EventLevel) 3;

  public static IdentityModelEventSource Logger { get; }

  public static bool ShowPII { get; set; } = false;

  public static string HiddenPIIString { get; } = "[PII is hidden. For more details, see https://aka.ms/IdentityModel/PII.]";

  public static bool HeaderWritten { get; set; } = false;

  [Event(6)]
  public void WriteAlways(string message)
  {
    if (!this.IsEnabled())
      return;
    message = this.PrepareMessage((EventLevel) 0, message);
    this.WriteEvent(6, message);
  }

  [NonEvent]
  public void WriteAlways(string message, params object[] args)
  {
    if (!this.IsEnabled())
      return;
    if (args != null)
      this.WriteAlways(LogHelper.FormatInvariant(message, args));
    else
      this.WriteAlways(message);
  }

  [Event(1)]
  public void WriteVerbose(string message)
  {
    if (!this.IsEnabled() || this.LogLevel < 5)
      return;
    message = this.PrepareMessage((EventLevel) 5, message);
    this.WriteEvent(1, message);
  }

  [NonEvent]
  public void WriteVerbose(string message, params object[] args)
  {
    if (!this.IsEnabled() || this.LogLevel < 5)
      return;
    if (args != null)
      this.WriteVerbose(LogHelper.FormatInvariant(message, args));
    else
      this.WriteVerbose(message);
  }

  [Event(2)]
  public void WriteInformation(string message)
  {
    if (!this.IsEnabled() || this.LogLevel < 4)
      return;
    message = this.PrepareMessage((EventLevel) 4, message);
    this.WriteEvent(2, message);
  }

  [NonEvent]
  public void WriteInformation(string message, params object[] args)
  {
    if (!this.IsEnabled() || this.LogLevel < 4)
      return;
    if (args != null)
      this.WriteInformation(LogHelper.FormatInvariant(message, args));
    else
      this.WriteInformation(message);
  }

  [Event(3)]
  public void WriteWarning(string message)
  {
    if (!this.IsEnabled() || this.LogLevel < 3)
      return;
    message = this.PrepareMessage((EventLevel) 3, message);
    this.WriteEvent(3, message);
  }

  [NonEvent]
  public void WriteWarning(string message, params object[] args)
  {
    if (args != null)
      this.WriteWarning(LogHelper.FormatInvariant(message, args));
    else
      this.WriteWarning(message);
  }

  [Event(4)]
  public void WriteError(string message)
  {
    if (!this.IsEnabled() || this.LogLevel < 2)
      return;
    message = this.PrepareMessage((EventLevel) 2, message);
    this.WriteEvent(4, message);
  }

  [NonEvent]
  public void WriteError(string message, params object[] args)
  {
    if (!this.IsEnabled() || this.LogLevel < 2)
      return;
    if (args != null)
      this.WriteError(LogHelper.FormatInvariant(message, args));
    else
      this.WriteError(message);
  }

  [Event(5)]
  public void WriteCritical(string message)
  {
    if (!this.IsEnabled() || this.LogLevel < 1)
      return;
    message = this.PrepareMessage((EventLevel) 1, message);
    this.WriteEvent(5, message);
  }

  [NonEvent]
  public void WriteCritical(string message, params object[] args)
  {
    if (!this.IsEnabled() || this.LogLevel < 1)
      return;
    if (args != null)
      this.WriteCritical(LogHelper.FormatInvariant(message, args));
    else
      this.WriteCritical(message);
  }

  [NonEvent]
  public void Write(EventLevel level, Exception innerException, string message)
  {
    this.Write(level, innerException, message, (object[]) null);
  }

  [NonEvent]
  public void Write(
    EventLevel level,
    Exception innerException,
    string message,
    params object[] args)
  {
    if (innerException != null)
      message = IdentityModelEventSource.ShowPII || LogHelper.IsCustomException(innerException) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Message: {0}, InnerException: {1}", (object) message, (object) innerException.Message) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Message: {0}, InnerException: {1}", (object) message, (object) innerException.GetType());
    if (!IdentityModelEventSource.HeaderWritten)
    {
      this.WriteAlways(string.Format((IFormatProvider) CultureInfo.InvariantCulture, IdentityModelEventSource._versionLogMessage, (object) ((Type) IntrospectionExtensions.GetTypeInfo(typeof (IdentityModelEventSource))).Assembly.GetName().Version.ToString()));
      this.WriteAlways(string.Format((IFormatProvider) CultureInfo.InvariantCulture, IdentityModelEventSource._dateLogMessage, (object) DateTime.UtcNow));
      if (IdentityModelEventSource.ShowPII)
        this.WriteAlways(IdentityModelEventSource._piiOnLogMessage);
      else
        this.WriteAlways(IdentityModelEventSource._piiOffLogMessage);
      IdentityModelEventSource.HeaderWritten = true;
    }
    switch ((int) level)
    {
      case 0:
        this.WriteAlways(message, args);
        break;
      case 1:
        this.WriteCritical(message, args);
        break;
      case 2:
        this.WriteError(message, args);
        break;
      case 3:
        this.WriteWarning(message, args);
        break;
      case 4:
        this.WriteInformation(message, args);
        break;
      case 5:
        this.WriteVerbose(message, args);
        break;
      default:
        this.WriteError(LogHelper.FormatInvariant("MIML10002: Unknown log level: {0}.", (object) level));
        this.WriteError(message, args);
        break;
    }
  }

  public EventLevel LogLevel { get; set; }

  private string PrepareMessage(EventLevel level, string message, params object[] args)
  {
    if (message == null)
      return string.Empty;
    try
    {
      return args != null && args.Length != 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]{1} {2}", (object) level.ToString(), (object) DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) LogHelper.FormatInvariant(message, args)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]{1} {2}", (object) level.ToString(), (object) DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) message);
    }
    catch
    {
    }
    try
    {
      return LogHelper.FormatInvariant("[{0}]{1} {2}", (object) level.ToString(), (object) DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) message);
    }
    catch (Exception ex)
    {
      return level.ToString() + DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture) + message;
    }
  }
}
