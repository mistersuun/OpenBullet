// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.CompositeAction
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using System.Collections.Generic;

#nullable disable
namespace OpenQA.Selenium.Interactions;

internal class CompositeAction : IAction
{
  private List<IAction> actionsList = new List<IAction>();

  public CompositeAction AddAction(IAction action)
  {
    this.actionsList.Add(action);
    return this;
  }

  public void Perform()
  {
    foreach (IAction actions in this.actionsList)
      actions.Perform();
  }
}
