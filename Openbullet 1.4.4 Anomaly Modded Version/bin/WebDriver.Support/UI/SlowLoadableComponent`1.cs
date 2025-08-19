// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.SlowLoadableComponent`1
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

using System;
using System.Globalization;
using System.Threading;

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public abstract class SlowLoadableComponent<T> : LoadableComponent<T> where T : SlowLoadableComponent<T>
{
  private readonly IClock clock;
  private readonly TimeSpan timeout;
  private TimeSpan sleepInterval = TimeSpan.FromMilliseconds(200.0);

  protected SlowLoadableComponent(TimeSpan timeout)
    : this(timeout, (IClock) new SystemClock())
  {
  }

  protected SlowLoadableComponent(TimeSpan timeout, IClock clock)
  {
    this.clock = clock;
    this.timeout = timeout;
  }

  public TimeSpan SleepInterval
  {
    get => this.sleepInterval;
    set => this.sleepInterval = value;
  }

  public override T Load()
  {
    if (this.IsLoaded)
      return (T) this;
    this.TryLoad();
    DateTime otherDateTime = this.clock.LaterBy(this.timeout);
    while (this.clock.IsNowBefore(otherDateTime))
    {
      if (this.IsLoaded)
        return (T) this;
      this.HandleErrors();
      this.Wait();
    }
    if (this.IsLoaded)
      return (T) this;
    throw new WebDriverTimeoutException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Timed out after {0} seconds.", (object) this.timeout.TotalSeconds));
  }

  protected virtual void HandleErrors()
  {
  }

  private void Wait() => Thread.Sleep(this.sleepInterval);
}
