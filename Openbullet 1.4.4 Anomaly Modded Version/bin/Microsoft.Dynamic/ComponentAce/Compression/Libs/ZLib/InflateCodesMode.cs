// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.InflateCodesMode
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

internal enum InflateCodesMode
{
  START,
  LEN,
  LENEXT,
  DIST,
  DISTEXT,
  COPY,
  LIT,
  WASH,
  END,
  BADCODE,
}
