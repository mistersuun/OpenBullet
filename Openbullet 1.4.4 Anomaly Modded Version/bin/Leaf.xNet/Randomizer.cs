// Decompiled with JetBrains decompiler
// Type: Leaf.xNet.Randomizer
// Assembly: Leaf.xNet, Version=5.1.83.0, Culture=neutral, PublicKeyToken=null
// MVID: A34D0085-375C-4EBE-A259-023772FF5358
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Leaf.xNet.dll

using System;

#nullable disable
namespace Leaf.xNet;

public static class Randomizer
{
  [ThreadStatic]
  private static Random _rand;

  public static Random Instance => Randomizer._rand ?? (Randomizer._rand = new Random());
}
