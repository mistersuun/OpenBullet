// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.SourceNonStrictDecoderFallbackBuffer
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Text;

#nullable disable
namespace IronPython.Runtime;

internal class SourceNonStrictDecoderFallbackBuffer : DecoderFallbackBuffer
{
  public override bool Fallback(byte[] bytesUnknown, int index)
  {
    throw new BadSourceException(bytesUnknown[index]);
  }

  public override char GetNextChar() => throw new NotImplementedException();

  public override bool MovePrevious() => throw new NotImplementedException();

  public override int Remaining => throw new NotImplementedException();
}
