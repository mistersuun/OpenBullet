// Decompiled with JetBrains decompiler
// Type: RuriLib.LS.BlockProcessingException
// Assembly: RuriLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 89084C32-DB31-406A-A6F3-F1B323C93989
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\RuriLib.dll

using System;

#nullable disable
namespace RuriLib.LS;

public class BlockProcessingException : Exception
{
  public BlockProcessingException()
  {
  }

  public BlockProcessingException(string message)
    : base(message)
  {
  }

  public BlockProcessingException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
