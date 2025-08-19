// Decompiled with JetBrains decompiler
// Type: ComponentAce.Compression.Libs.ZLib.ZLibResultCode
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace ComponentAce.Compression.Libs.ZLib;

public enum ZLibResultCode
{
  Z_VERSION_ERROR = -6, // 0xFFFFFFFA
  Z_BUF_ERROR = -5, // 0xFFFFFFFB
  Z_MEM_ERROR = -4, // 0xFFFFFFFC
  Z_DATA_ERROR = -3, // 0xFFFFFFFD
  Z_STREAM_ERROR = -2, // 0xFFFFFFFE
  Z_ERRNO = -1, // 0xFFFFFFFF
  Z_OK = 0,
  Z_STREAM_END = 1,
  Z_NEED_DICT = 2,
}
