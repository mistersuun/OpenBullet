// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Utils.Boxes
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

#nullable disable
namespace ICSharpCode.AvalonEdit.Utils;

internal static class Boxes
{
  public static readonly object True = (object) true;
  public static readonly object False = (object) false;

  public static object Box(bool value) => !value ? Boxes.False : Boxes.True;
}
