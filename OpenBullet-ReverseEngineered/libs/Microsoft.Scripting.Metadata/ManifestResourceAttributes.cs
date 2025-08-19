// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.ManifestResourceAttributes
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Metadata;

[Flags]
public enum ManifestResourceAttributes
{
  PublicVisibility = 1,
  PrivateVisibility = 2,
  VisibilityMask = 7,
  InExternalFile = 16, // 0x00000010
}
