// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.IndentationException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting;
using Microsoft.Scripting.Utils;
using System;
using System.Runtime.Serialization;
using System.Security;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
internal class IndentationException : SyntaxErrorException
{
  public IndentationException(string message)
    : base(message)
  {
  }

  public IndentationException(
    string message,
    SourceUnit sourceUnit,
    SourceSpan span,
    int errorCode,
    Severity severity)
    : base(message, sourceUnit, span, errorCode, severity)
  {
  }

  protected IndentationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  [SecurityCritical]
  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    ContractUtils.RequiresNotNull((object) info, nameof (info));
    base.GetObjectData(info, context);
  }
}
