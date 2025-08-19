// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.SubstringException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

public class SubstringException : Exception
{
  public SubstringException()
  {
  }

  public SubstringException(string message)
    : base(message)
  {
  }

  public SubstringException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
