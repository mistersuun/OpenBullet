// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.CalendarSelectionChangedEventArgs
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class CalendarSelectionChangedEventArgs(
  RoutedEvent eventId,
  IList removedItems,
  IList addedItems) : SelectionChangedEventArgs(eventId, removedItems, addedItems)
{
  protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
  {
    if (genericHandler is EventHandler<SelectionChangedEventArgs> eventHandler)
      eventHandler(genericTarget, (SelectionChangedEventArgs) this);
    else
      base.InvokeEventHandler(genericHandler, genericTarget);
  }
}
