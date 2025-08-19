// Decompiled with JetBrains decompiler
// Type: Tesseract.TesseractException
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Tesseract;

[Serializable]
public class TesseractException : Exception, ISerializable
{
  public TesseractException()
  {
  }

  public TesseractException(string message)
    : base(message)
  {
  }

  public TesseractException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  protected TesseractException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
