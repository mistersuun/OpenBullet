// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.DefaultWait`1
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public class DefaultWait<T> : IWait<T>
{
  private T input;
  private IClock clock;
  private TimeSpan timeout = DefaultWait<T>.DefaultSleepTimeout;
  private TimeSpan sleepInterval = DefaultWait<T>.DefaultSleepTimeout;
  private string message = string.Empty;
  private List<Type> ignoredExceptions = new List<Type>();

  public DefaultWait(T input)
    : this(input, (IClock) new SystemClock())
  {
  }

  public DefaultWait(T input, IClock clock)
  {
    if ((object) input == null)
      throw new ArgumentNullException(nameof (input), "input cannot be null");
    if (clock == null)
      throw new ArgumentNullException(nameof (clock), "clock cannot be null");
    this.input = input;
    this.clock = clock;
  }

  public TimeSpan Timeout
  {
    get => this.timeout;
    set => this.timeout = value;
  }

  public TimeSpan PollingInterval
  {
    get => this.sleepInterval;
    set => this.sleepInterval = value;
  }

  public string Message
  {
    get => this.message;
    set => this.message = value;
  }

  private static TimeSpan DefaultSleepTimeout => TimeSpan.FromMilliseconds(500.0);

  public void IgnoreExceptionTypes(params Type[] exceptionTypes)
  {
    if (exceptionTypes == null)
      throw new ArgumentNullException(nameof (exceptionTypes), "exceptionTypes cannot be null");
    foreach (Type exceptionType in exceptionTypes)
    {
      if (!typeof (Exception).IsAssignableFrom(exceptionType))
        throw new ArgumentException("All types to be ignored must derive from System.Exception", nameof (exceptionTypes));
    }
    this.ignoredExceptions.AddRange((IEnumerable<Type>) exceptionTypes);
  }

  public TResult Until<TResult>(Func<T, TResult> condition)
  {
    if (condition == null)
      throw new ArgumentNullException(nameof (condition), "condition cannot be null");
    Type c = typeof (TResult);
    if (c.IsValueType && c != typeof (bool) || !typeof (object).IsAssignableFrom(c))
      throw new ArgumentException("Can only wait on an object or boolean response, tried to use type: " + c.ToString(), nameof (condition));
    Exception lastException = (Exception) null;
    DateTime otherDateTime = this.clock.LaterBy(this.timeout);
    while (true)
    {
      try
      {
        TResult result = condition(this.input);
        if (c == typeof (bool))
        {
          bool? nullable = (object) result as bool?;
          if (nullable.HasValue)
          {
            if (nullable.Value)
              return result;
          }
        }
        else if ((object) result != null)
          return result;
      }
      catch (Exception ex)
      {
        if (!this.IsIgnoredException(ex))
          throw;
        lastException = ex;
      }
      if (!this.clock.IsNowBefore(otherDateTime))
      {
        string exceptionMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Timed out after {0} seconds", (object) this.timeout.TotalSeconds);
        if (!string.IsNullOrEmpty(this.message))
          exceptionMessage = $"{exceptionMessage}: {this.message}";
        this.ThrowTimeoutException(exceptionMessage, lastException);
      }
      Thread.Sleep(this.sleepInterval);
    }
  }

  protected virtual void ThrowTimeoutException(string exceptionMessage, Exception lastException)
  {
    throw new WebDriverTimeoutException(exceptionMessage, lastException);
  }

  private bool IsIgnoredException(Exception exception)
  {
    return this.ignoredExceptions.Any<Type>((Func<Type, bool>) (type => type.IsAssignableFrom(exception.GetType())));
  }
}
