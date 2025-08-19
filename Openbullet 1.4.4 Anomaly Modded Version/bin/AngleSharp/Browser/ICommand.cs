// Decompiled with JetBrains decompiler
// Type: AngleSharp.Browser.ICommand
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Dom;

#nullable disable
namespace AngleSharp.Browser;

public interface ICommand
{
  string CommandId { get; }

  bool Execute(IDocument document, bool showUserInterface, string value);

  bool IsEnabled(IDocument document);

  bool IsIndeterminate(IDocument document);

  bool IsExecuted(IDocument document);

  bool IsSupported(IDocument document);

  string GetValue(IDocument document);
}
