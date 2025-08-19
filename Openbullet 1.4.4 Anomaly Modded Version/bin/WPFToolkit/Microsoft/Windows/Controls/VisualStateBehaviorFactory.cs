// Decompiled with JetBrains decompiler
// Type: Microsoft.Windows.Controls.VisualStateBehaviorFactory
// Assembly: WPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 23F18150-F0E6-45EF-A159-A068722819DA
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\WPFToolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Windows.Controls;

internal class VisualStateBehaviorFactory : TypeHandlerFactory<VisualStateBehavior>
{
  [ThreadStatic]
  private static VisualStateBehaviorFactory _instance;
  [ThreadStatic]
  private static bool _registeredKnownTypes;

  internal static VisualStateBehaviorFactory Instance
  {
    get
    {
      if (VisualStateBehaviorFactory._instance == null)
        VisualStateBehaviorFactory._instance = new VisualStateBehaviorFactory();
      return VisualStateBehaviorFactory._instance;
    }
  }

  private VisualStateBehaviorFactory()
  {
  }

  internal static void AttachBehavior(Control control)
  {
    if (DependencyPropertyHelper.GetValueSource((DependencyObject) control, VisualStateBehavior.VisualStateBehaviorProperty).BaseValueSource != BaseValueSource.Default)
      return;
    if (!VisualStateBehaviorFactory._registeredKnownTypes)
    {
      VisualStateBehaviorFactory._registeredKnownTypes = true;
      VisualStateBehaviorFactory.RegisterControlBehavior((VisualStateBehavior) new ButtonBaseBehavior());
      VisualStateBehaviorFactory.RegisterControlBehavior((VisualStateBehavior) new ToggleButtonBehavior());
      VisualStateBehaviorFactory.RegisterControlBehavior((VisualStateBehavior) new ListBoxItemBehavior());
      VisualStateBehaviorFactory.RegisterControlBehavior((VisualStateBehavior) new TextBoxBaseBehavior());
      VisualStateBehaviorFactory.RegisterControlBehavior((VisualStateBehavior) new ProgressBarBehavior());
    }
    VisualStateBehavior handler = VisualStateBehaviorFactory.Instance.GetHandler(control.GetType());
    if (handler == null)
      return;
    VisualStateBehavior.SetVisualStateBehavior((DependencyObject) control, handler);
  }

  internal static void RegisterControlBehavior(VisualStateBehavior behavior)
  {
    VisualStateBehaviorFactory.Instance.RegisterHandler(behavior);
  }

  protected override Type GetBaseType(VisualStateBehavior behavior) => behavior.TargetType;
}
