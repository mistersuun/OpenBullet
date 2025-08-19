// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.PEFileConstants
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal static class PEFileConstants
{
  internal const ushort DosSignature = 23117;
  internal const int PESignatureOffsetLocation = 60;
  internal const uint PESignature = 17744;
  internal const int BasicPEHeaderSize = 60;
  internal const int SizeofCOFFFileHeader = 20;
  internal const int SizeofOptionalHeaderStandardFields32 = 28;
  internal const int SizeofOptionalHeaderStandardFields64 = 24;
  internal const int SizeofOptionalHeaderNTAdditionalFields32 = 68;
  internal const int SizeofOptionalHeaderNTAdditionalFields64 = 88;
  internal const int NumberofOptionalHeaderDirectoryEntries = 16 /*0x10*/;
  internal const int SizeofOptionalHeaderDirectoriesEntries = 64 /*0x40*/;
  internal const int SizeofSectionHeader = 40;
  internal const int SizeofSectionName = 8;
  internal const int SizeofResourceDirectory = 16 /*0x10*/;
  internal const int SizeofResourceDirectoryEntry = 8;
}
