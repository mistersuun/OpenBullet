// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Support.UI.LoadableComponent`1
// Assembly: WebDriver.Support, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A861AD7F-E5EF-4AEB-8F2E-DA4D9518ABA6
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.Support.dll

#nullable disable
namespace OpenQA.Selenium.Support.UI;

public abstract class LoadableComponent<T> : ILoadableComponent where T : LoadableComponent<T>
{
  public virtual string UnableToLoadMessage { get; set; }

  protected bool IsLoaded
  {
    get
    {
      try
      {
        return this.EvaluateLoadedStatus();
      }
      catch (WebDriverException ex)
      {
        return false;
      }
    }
  }

  public virtual T Load()
  {
    if (this.IsLoaded)
      return (T) this;
    this.TryLoad();
    if (!this.IsLoaded)
      throw new LoadableComponentException(this.UnableToLoadMessage);
    return (T) this;
  }

  ILoadableComponent ILoadableComponent.Load() => (ILoadableComponent) this.Load();

  protected virtual void HandleLoadError(WebDriverException ex)
  {
  }

  protected abstract void ExecuteLoad();

  protected abstract bool EvaluateLoadedStatus();

  protected T TryLoad()
  {
    try
    {
      this.ExecuteLoad();
    }
    catch (WebDriverException ex)
    {
      this.HandleLoadError(ex);
    }
    return (T) this;
  }
}
