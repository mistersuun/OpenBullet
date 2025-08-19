// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.CodeCompletion.IOverloadProvider
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System.ComponentModel;

#nullable disable
namespace ICSharpCode.AvalonEdit.CodeCompletion;

public interface IOverloadProvider : INotifyPropertyChanged
{
  int SelectedIndex { get; set; }

  int Count { get; }

  string CurrentIndexText { get; }

  object CurrentHeader { get; }

  object CurrentContent { get; }
}
