// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Services.StormWall.StormWallException
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet.Services.StormWall;

[Serializable]
public class StormWallException : Exception
{
  public StormWallException()
  {
  }

  public StormWallException(string message)
    : base(message)
  {
  }

  public StormWallException(string message, Exception inner)
    : base(message, inner)
  {
  }
}
