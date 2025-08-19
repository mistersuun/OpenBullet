// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Utils.EmptyArray`1
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Utils;

internal static class EmptyArray<T>
{
  internal static T[] Instance = new T[0];
}
