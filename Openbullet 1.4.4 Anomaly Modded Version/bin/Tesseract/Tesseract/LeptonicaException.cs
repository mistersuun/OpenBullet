// Decompiled with JetBrains decompiler
// Type: Tesseract.LeptonicaException
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Tesseract;

[Serializable]
public class LeptonicaException : Exception
{
  public LeptonicaException()
  {
  }

  public LeptonicaException(string message)
    : base(message)
  {
  }

  public LeptonicaException(string message, Exception inner)
    : base(message, inner)
  {
  }

  protected LeptonicaException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
