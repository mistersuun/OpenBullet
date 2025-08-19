// Decompiled with JetBrains decompiler
// Type: Xceed.Wpf.AvalonDock.Layout.ILayoutGroup
// Assembly: Xceed.Wpf.AvalonDock, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4
// MVID: DE8939C0-6E8D-44B9-AFAA-F7E7CD945D58
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Xceed.Wpf.AvalonDock.dll

using System;
using System.ComponentModel;

#nullable disable
namespace Xceed.Wpf.AvalonDock.Layout;

public interface ILayoutGroup : 
  ILayoutContainer,
  ILayoutElement,
  INotifyPropertyChanged,
  INotifyPropertyChanging
{
  int IndexOfChild(ILayoutElement element);

  void InsertChildAt(int index, ILayoutElement element);

  void RemoveChildAt(int index);

  void ReplaceChildAt(int index, ILayoutElement element);

  event EventHandler ChildrenCollectionChanged;
}
