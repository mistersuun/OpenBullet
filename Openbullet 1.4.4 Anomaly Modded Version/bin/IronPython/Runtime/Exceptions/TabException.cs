// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.TabException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using System;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
internal sealed class TabException : IndentationException
{
  public TabException(string message)
    : base(message)
  {
  }

  public TabException(
    string message,
    SourceUnit sourceUnit,
    SourceSpan span,
    int errorCode,
    Severity severity)
    : base(message, sourceUnit, span, errorCode, severity)
  {
  }
}
