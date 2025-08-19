// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.Toolkit.AutoSelectTextBox
// Assembly: Xceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: F4AF6194-6F09-42EF-85B9-511519285C7B
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.Toolkit.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Utilities;

#nullable disable
namespace Xceed.Wpf.Toolkit;

public class AutoSelectTextBox : TextBox
{
  public static readonly DependencyProperty AutoSelectBehaviorProperty = DependencyProperty.Register(nameof (AutoSelectBehavior), typeof (AutoSelectBehavior), typeof (AutoSelectTextBox), (PropertyMetadata) new UIPropertyMetadata((object) AutoSelectBehavior.Never));
  public static readonly DependencyProperty AutoMoveFocusProperty = DependencyProperty.Register(nameof (AutoMoveFocus), typeof (bool), typeof (AutoSelectTextBox), (PropertyMetadata) new UIPropertyMetadata((object) false));
  public static readonly RoutedEvent QueryMoveFocusEvent = EventManager.RegisterRoutedEvent("QueryMoveFocus", RoutingStrategy.Bubble, typeof (QueryMoveFocusEventHandler), typeof (AutoSelectTextBox));

  public AutoSelectBehavior AutoSelectBehavior
  {
    get => (AutoSelectBehavior) this.GetValue(AutoSelectTextBox.AutoSelectBehaviorProperty);
    set => this.SetValue(AutoSelectTextBox.AutoSelectBehaviorProperty, (object) value);
  }

  public bool AutoMoveFocus
  {
    get => (bool) this.GetValue(AutoSelectTextBox.AutoMoveFocusProperty);
    set => this.SetValue(AutoSelectTextBox.AutoMoveFocusProperty, (object) value);
  }

  protected override void OnPreviewKeyDown(KeyEventArgs e)
  {
    if (!this.AutoMoveFocus)
    {
      base.OnPreviewKeyDown(e);
    }
    else
    {
      if (e.Key == Key.Left && (Keyboard.Modifiers == ModifierKeys.None || Keyboard.Modifiers == ModifierKeys.Control))
        e.Handled = this.MoveFocusLeft();
      if (e.Key == Key.Right && (Keyboard.Modifiers == ModifierKeys.None || Keyboard.Modifiers == ModifierKeys.Control))
        e.Handled = this.MoveFocusRight();
      if ((e.Key == Key.Up || e.Key == Key.Prior) && (Keyboard.Modifiers == ModifierKeys.None || Keyboard.Modifiers == ModifierKeys.Control))
        e.Handled = this.MoveFocusUp();
      if ((e.Key == Key.Down || e.Key == Key.Next) && (Keyboard.Modifiers == ModifierKeys.None || Keyboard.Modifiers == ModifierKeys.Control))
        e.Handled = this.MoveFocusDown();
      base.OnPreviewKeyDown(e);
    }
  }

  protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
  {
    base.OnPreviewGotKeyboardFocus(e);
    if (this.AutoSelectBehavior != AutoSelectBehavior.OnFocus || TreeHelper.IsDescendantOf(e.OldFocus as DependencyObject, (DependencyObject) this))
      return;
    this.SelectAll();
  }

  protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
  {
    base.OnPreviewMouseLeftButtonDown(e);
    if (this.AutoSelectBehavior == AutoSelectBehavior.Never || this.IsKeyboardFocusWithin)
      return;
    this.Focus();
    e.Handled = true;
  }

  protected override void OnTextChanged(TextChangedEventArgs e)
  {
    base.OnTextChanged(e);
    if (!this.AutoMoveFocus || this.Text.Length == 0 || this.Text.Length != this.MaxLength || this.CaretIndex != this.MaxLength || !this.CanMoveFocus(FocusNavigationDirection.Right, true))
      return;
    this.MoveFocus(new TraversalRequest(this.FlowDirection == FlowDirection.LeftToRight ? FocusNavigationDirection.Right : FocusNavigationDirection.Left));
  }

  private bool CanMoveFocus(FocusNavigationDirection direction, bool reachedMax)
  {
    QueryMoveFocusEventArgs e = new QueryMoveFocusEventArgs(direction, reachedMax);
    this.RaiseEvent((RoutedEventArgs) e);
    return e.CanMoveFocus;
  }

  private bool MoveFocusLeft()
  {
    if (this.FlowDirection == FlowDirection.LeftToRight)
    {
      if (this.CaretIndex == 0 && this.SelectionLength == 0)
      {
        if (ComponentCommands.MoveFocusBack.CanExecute((object) null, (IInputElement) this))
        {
          ComponentCommands.MoveFocusBack.Execute((object) null, (IInputElement) this);
          return true;
        }
        if (this.CanMoveFocus(FocusNavigationDirection.Left, false))
        {
          this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
          return true;
        }
      }
    }
    else if (this.CaretIndex == this.Text.Length && this.SelectionLength == 0)
    {
      if (ComponentCommands.MoveFocusBack.CanExecute((object) null, (IInputElement) this))
      {
        ComponentCommands.MoveFocusBack.Execute((object) null, (IInputElement) this);
        return true;
      }
      if (this.CanMoveFocus(FocusNavigationDirection.Left, false))
      {
        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
        return true;
      }
    }
    return false;
  }

  private bool MoveFocusRight()
  {
    if (this.FlowDirection == FlowDirection.LeftToRight)
    {
      if (this.CaretIndex == this.Text.Length && this.SelectionLength == 0)
      {
        if (ComponentCommands.MoveFocusForward.CanExecute((object) null, (IInputElement) this))
        {
          ComponentCommands.MoveFocusForward.Execute((object) null, (IInputElement) this);
          return true;
        }
        if (this.CanMoveFocus(FocusNavigationDirection.Right, false))
        {
          this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
          return true;
        }
      }
    }
    else if (this.CaretIndex == 0 && this.SelectionLength == 0)
    {
      if (ComponentCommands.MoveFocusForward.CanExecute((object) null, (IInputElement) this))
      {
        ComponentCommands.MoveFocusForward.Execute((object) null, (IInputElement) this);
        return true;
      }
      if (this.CanMoveFocus(FocusNavigationDirection.Right, false))
      {
        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
        return true;
      }
    }
    return false;
  }

  private bool MoveFocusUp()
  {
    if (this.GetLineIndexFromCharacterIndex(this.SelectionStart) == 0)
    {
      if (ComponentCommands.MoveFocusUp.CanExecute((object) null, (IInputElement) this))
      {
        ComponentCommands.MoveFocusUp.Execute((object) null, (IInputElement) this);
        return true;
      }
      if (this.CanMoveFocus(FocusNavigationDirection.Up, false))
      {
        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
        return true;
      }
    }
    return false;
  }

  private bool MoveFocusDown()
  {
    if (this.GetLineIndexFromCharacterIndex(this.SelectionStart) == this.LineCount - 1)
    {
      if (ComponentCommands.MoveFocusDown.CanExecute((object) null, (IInputElement) this))
      {
        ComponentCommands.MoveFocusDown.Execute((object) null, (IInputElement) this);
        return true;
      }
      if (this.CanMoveFocus(FocusNavigationDirection.Down, false))
      {
        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
        return true;
      }
    }
    return false;
  }
}
