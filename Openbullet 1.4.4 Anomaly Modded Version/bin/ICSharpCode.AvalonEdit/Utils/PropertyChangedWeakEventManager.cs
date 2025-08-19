// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.PropertyChangedWeakEventManager
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.ComponentModel;
using System.Windows;

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

public sealed class PropertyChangedWeakEventManager : 
  WeakEventManagerBase<PropertyChangedWeakEventManager, INotifyPropertyChanged>
{
  protected override void StartListening(INotifyPropertyChanged source)
  {
    source.PropertyChanged += new PropertyChangedEventHandler(((WeakEventManager) this).DeliverEvent);
  }

  protected override void StopListening(INotifyPropertyChanged source)
  {
    source.PropertyChanged -= new PropertyChangedEventHandler(((WeakEventManager) this).DeliverEvent);
  }
}
