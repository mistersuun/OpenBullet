// Decompiled with JetBrains decompiler
// Type: Tesseract.LoadLibraryException
// Assembly: Tesseract, Version=3.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 10D55B5F-CAB6-4027-9165-B66DDE8823E1
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Tesseract.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Tesseract;

[Serializable]
public class LoadLibraryException : SystemException
{
  public LoadLibraryException()
  {
  }

  public LoadLibraryException(string message)
    : base(message)
  {
  }

  public LoadLibraryException(string message, Exception inner)
    : base(message, inner)
  {
  }

  protected LoadLibraryException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
