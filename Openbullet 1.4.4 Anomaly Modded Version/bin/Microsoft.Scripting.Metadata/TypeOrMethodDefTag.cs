// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.TypeOrMethodDefTag
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class TypeOrMethodDefTag
{
  internal const int NumberOfBits = 1;
  internal const int LargeRowSize = 32768 /*0x8000*/;
  internal const uint TypeDef = 0;
  internal const uint MethodDef = 1;
  internal const uint TagMask = 1;
  internal const TableMask TablesReferenced = TableMask.TypeDef | TableMask.Method;

  internal static MetadataToken ConvertToToken(uint typeOrMethodDef)
  {
    return new MetadataToken(((int) typeOrMethodDef & 1) == 0 ? MetadataTokenType.TypeDef : MetadataTokenType.MethodDef, typeOrMethodDef >> 1);
  }

  internal static uint ConvertTypeDefRowIdToTag(int typeDefRowId) => (uint) (typeDefRowId << 1 | 0);

  internal static uint ConvertMethodDefRowIdToTag(int methodDefRowId)
  {
    return (uint) (methodDefRowId << 1 | 1);
  }
}
