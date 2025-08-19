// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.COR20Constants
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class COR20Constants
{
  internal const int SizeOfCOR20Header = 72;
  internal const uint COR20MetadataSignature = 1112167234;
  internal const int MinimumSizeofMetadataHeader = 16 /*0x10*/;
  internal const int SizeofStorageHeader = 4;
  internal const int MinimumSizeofStreamHeader = 8;
  internal const string StringStreamName = "#Strings";
  internal const string BlobStreamName = "#Blob";
  internal const string GUIDStreamName = "#GUID";
  internal const string UserStringStreamName = "#US";
  internal const string CompressedMetadataTableStreamName = "#~";
  internal const string UncompressedMetadataTableStreamName = "#-";
  internal const int LargeStreamHeapSize = 4096 /*0x1000*/;
}
