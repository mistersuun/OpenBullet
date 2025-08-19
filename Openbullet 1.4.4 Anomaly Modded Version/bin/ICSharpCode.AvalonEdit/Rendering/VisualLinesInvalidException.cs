// Decompiled with JetBrains decompiler
// Type: ICSharpCode.AvalonEdit.Rendering.VisualLinesInvalidException
// Assembly: ICSharpCode.AvalonEdit, Version=5.0.3.0, Culture=neutral, PublicKeyToken=9cc39be672370310
// MVID: 0CDAE4EE-B402-4B03-A15F-9E1877AEC22C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\ICSharpCode.AvalonEdit.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace ICSharpCode.AvalonEdit.Rendering;

[Serializable]
public class VisualLinesInvalidException : Exception
{
  public VisualLinesInvalidException()
  {
  }

  public VisualLinesInvalidException(string message)
    : base(message)
  {
  }

  public VisualLinesInvalidException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected VisualLinesInvalidException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
