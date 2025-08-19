// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.EnumInfo
// Assembly: Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: D47DE75A-7E3F-422C-A4CA-64A654C80495
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Newtonsoft.Json.dll

#nullable disable
namespace Newtonsoft.Json.Utilities;

internal class EnumInfo
{
  public readonly bool IsFlags;
  public readonly ulong[] Values;
  public readonly string[] Names;
  public readonly string[] ResolvedNames;

  public EnumInfo(bool isFlags, ulong[] values, string[] names, string[] resolvedNames)
  {
    this.IsFlags = isFlags;
    this.Values = values;
    this.Names = names;
    this.ResolvedNames = resolvedNames;
  }
}
