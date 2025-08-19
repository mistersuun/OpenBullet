// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Editing.DragDropException
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Editing;

[Serializable]
public class DragDropException : Exception
{
  public DragDropException()
  {
  }

  public DragDropException(string message)
    : base(message)
  {
  }

  public DragDropException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected DragDropException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
