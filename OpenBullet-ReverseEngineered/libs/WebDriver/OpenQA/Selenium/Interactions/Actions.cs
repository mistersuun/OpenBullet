// Decompiled with JetBrains decompiler
// Type: OpenQA.Selenium.Interactions.Actions
// Assembly: WebDriver, Version=3.141.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 19027CC8-5014-4409-9501-B87290061165
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WebDriver.dll

using OpenQA.Selenium.Internal;
using System;
using System.Drawing;

#nullable disable
namespace OpenQA.Selenium.Interactions;

public class Actions : IAction
{
  private readonly TimeSpan DefaultMouseMoveDuration = TimeSpan.FromMilliseconds(250.0);
  private ActionBuilder actionBuilder = new ActionBuilder();
  private PointerInputDevice defaultMouse = new PointerInputDevice(PointerKind.Mouse, "default mouse");
  private KeyInputDevice defaultKeyboard = new KeyInputDevice("default keyboard");
  private IKeyboard keyboard;
  private IMouse mouse;
  private IActionExecutor actionExecutor;
  private CompositeAction action = new CompositeAction();

  public Actions(IWebDriver driver)
  {
    IHasInputDevices driverAs1 = this.GetDriverAs<IHasInputDevices>(driver);
    if (driverAs1 == null)
      throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IHasInputDevices.", nameof (driver));
    IActionExecutor driverAs2 = this.GetDriverAs<IActionExecutor>(driver);
    if (driverAs2 == null)
      throw new ArgumentException("The IWebDriver object must implement or wrap a driver that implements IActionExecutor.", nameof (driver));
    this.keyboard = driverAs1.Keyboard;
    this.mouse = driverAs1.Mouse;
    this.actionExecutor = driverAs2;
  }

  public Actions KeyDown(string theKey) => this.KeyDown((IWebElement) null, theKey);

  public Actions KeyDown(IWebElement element, string theKey)
  {
    if (string.IsNullOrEmpty(theKey))
      throw new ArgumentException("The key value must not be null or empty", nameof (theKey));
    this.action.AddAction((IAction) new KeyDownAction(this.keyboard, this.mouse, Actions.GetLocatableFromElement(element), theKey));
    if (element != null)
    {
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(element, 0, 0, this.DefaultMouseMoveDuration));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    }
    this.actionBuilder.AddAction(this.defaultKeyboard.CreateKeyDown(theKey[0]));
    this.actionBuilder.AddAction((Interaction) new PauseInteraction((InputDevice) this.defaultKeyboard, TimeSpan.FromMilliseconds(100.0)));
    return this;
  }

  public Actions KeyUp(string theKey) => this.KeyUp((IWebElement) null, theKey);

  public Actions KeyUp(IWebElement element, string theKey)
  {
    if (string.IsNullOrEmpty(theKey))
      throw new ArgumentException("The key value must not be null or empty", nameof (theKey));
    this.action.AddAction((IAction) new KeyUpAction(this.keyboard, this.mouse, Actions.GetLocatableFromElement(element), theKey));
    if (element != null)
    {
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(element, 0, 0, this.DefaultMouseMoveDuration));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    }
    this.actionBuilder.AddAction(this.defaultKeyboard.CreateKeyUp(theKey[0]));
    return this;
  }

  public Actions SendKeys(string keysToSend) => this.SendKeys((IWebElement) null, keysToSend);

  public Actions SendKeys(IWebElement element, string keysToSend)
  {
    if (string.IsNullOrEmpty(keysToSend))
      throw new ArgumentException("The key value must not be null or empty", nameof (keysToSend));
    this.action.AddAction((IAction) new SendKeysAction(this.keyboard, this.mouse, Actions.GetLocatableFromElement(element), keysToSend));
    if (element != null)
    {
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(element, 0, 0, this.DefaultMouseMoveDuration));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    }
    foreach (char codePoint in keysToSend)
    {
      this.actionBuilder.AddAction(this.defaultKeyboard.CreateKeyDown(codePoint));
      this.actionBuilder.AddAction(this.defaultKeyboard.CreateKeyUp(codePoint));
    }
    return this;
  }

  public Actions ClickAndHold(IWebElement onElement)
  {
    this.MoveToElement(onElement).ClickAndHold();
    return this;
  }

  public Actions ClickAndHold()
  {
    this.action.AddAction((IAction) new ClickAndHoldAction(this.mouse, (ILocatable) null));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
    return this;
  }

  public Actions Release(IWebElement onElement)
  {
    this.MoveToElement(onElement).Release();
    return this;
  }

  public Actions Release()
  {
    this.action.AddAction((IAction) new ButtonReleaseAction(this.mouse, (ILocatable) null));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    return this;
  }

  public Actions Click(IWebElement onElement)
  {
    this.MoveToElement(onElement).Click();
    return this;
  }

  public Actions Click()
  {
    this.action.AddAction((IAction) new ClickAction(this.mouse, (ILocatable) null));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    return this;
  }

  public Actions DoubleClick(IWebElement onElement)
  {
    this.MoveToElement(onElement).DoubleClick();
    return this;
  }

  public Actions DoubleClick()
  {
    this.action.AddAction((IAction) new DoubleClickAction(this.mouse, (ILocatable) null));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Left));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Left));
    return this;
  }

  public Actions MoveToElement(IWebElement toElement)
  {
    if (toElement == null)
      throw new ArgumentException("MoveToElement cannot move to a null element with no offset.", nameof (toElement));
    this.action.AddAction((IAction) new MoveMouseAction(this.mouse, Actions.GetLocatableFromElement(toElement)));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(toElement, 0, 0, this.DefaultMouseMoveDuration));
    return this;
  }

  public Actions MoveToElement(IWebElement toElement, int offsetX, int offsetY)
  {
    return this.MoveToElement(toElement, offsetX, offsetY, MoveToElementOffsetOrigin.TopLeft);
  }

  public Actions MoveToElement(
    IWebElement toElement,
    int offsetX,
    int offsetY,
    MoveToElementOffsetOrigin offsetOrigin)
  {
    ILocatable locatableFromElement = Actions.GetLocatableFromElement(toElement);
    Size size = toElement.Size;
    Point location = toElement.Location;
    if (offsetOrigin == MoveToElementOffsetOrigin.TopLeft)
    {
      int xOffset = offsetX - size.Width / 2;
      int yOffset = offsetY - size.Height / 2;
      this.action.AddAction((IAction) new MoveToOffsetAction(this.mouse, locatableFromElement, offsetX, offsetY));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(toElement, xOffset, yOffset, this.DefaultMouseMoveDuration));
    }
    else
    {
      int offsetX1 = offsetX + size.Width / 2;
      int offsetY1 = offsetY + size.Height / 2;
      this.action.AddAction((IAction) new MoveToOffsetAction(this.mouse, locatableFromElement, offsetX1, offsetY1));
      this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(toElement, offsetX, offsetY, this.DefaultMouseMoveDuration));
    }
    return this;
  }

  public Actions MoveByOffset(int offsetX, int offsetY)
  {
    this.action.AddAction((IAction) new MoveToOffsetAction(this.mouse, (ILocatable) null, offsetX, offsetY));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerMove(CoordinateOrigin.Pointer, offsetX, offsetY, this.DefaultMouseMoveDuration));
    return this;
  }

  public Actions ContextClick(IWebElement onElement)
  {
    this.MoveToElement(onElement).ContextClick();
    return this;
  }

  public Actions ContextClick()
  {
    this.action.AddAction((IAction) new ContextClickAction(this.mouse, (ILocatable) null));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerDown(MouseButton.Right));
    this.actionBuilder.AddAction(this.defaultMouse.CreatePointerUp(MouseButton.Right));
    return this;
  }

  public Actions DragAndDrop(IWebElement source, IWebElement target)
  {
    this.ClickAndHold(source).MoveToElement(target).Release(target);
    return this;
  }

  public Actions DragAndDropToOffset(IWebElement source, int offsetX, int offsetY)
  {
    this.ClickAndHold(source).MoveByOffset(offsetX, offsetY).Release();
    return this;
  }

  public IAction Build() => (IAction) this;

  public void Perform()
  {
    if (this.actionExecutor.IsActionExecutor)
      this.actionExecutor.PerformActions(this.actionBuilder.ToActionSequenceList());
    else
      this.action.Perform();
  }

  protected static ILocatable GetLocatableFromElement(IWebElement element)
  {
    if (element == null)
      return (ILocatable) null;
    ILocatable locatable = (ILocatable) null;
    for (IWrapsElement wrapsElement = element as IWrapsElement; wrapsElement != null; wrapsElement = wrapsElement.WrappedElement as IWrapsElement)
      locatable = wrapsElement.WrappedElement as ILocatable;
    if (locatable == null)
      locatable = element as ILocatable;
    return locatable != null ? locatable : throw new ArgumentException("The IWebElement object must implement or wrap an element that implements ILocatable.", nameof (element));
  }

  protected void AddAction(IAction actionToAdd) => this.action.AddAction(actionToAdd);

  private T GetDriverAs<T>(IWebDriver driver) where T : class
  {
    if (!(driver is T driverAs1))
    {
      for (IWrapsDriver wrapsDriver = driver as IWrapsDriver; wrapsDriver != null; wrapsDriver = wrapsDriver.WrappedDriver as IWrapsDriver)
      {
        if (wrapsDriver.WrappedDriver is T driverAs1)
        {
          driver = wrapsDriver.WrappedDriver;
          break;
        }
      }
    }
    return driverAs1;
  }
}
