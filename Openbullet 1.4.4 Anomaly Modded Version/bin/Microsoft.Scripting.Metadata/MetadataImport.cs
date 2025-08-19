// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataImport
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

internal sealed class MetadataImport
{
  private readonly MemoryBlock _image;
  private const int TableCount = 45;
  private int _numberOfSections;
  private OptionalHeaderDirectoryEntries _optionalHeaderDirectoryEntries;
  private SectionHeader[] _sectionHeaders;
  private COR20Header _cor20Header;
  private StorageHeader _storageHeader;
  private StreamHeader[] _streamHeaders;
  private MemoryBlock _stringStream;
  private MemoryBlock _blobStream;
  private MemoryBlock _guidStream;
  private MemoryBlock _userStringStream;
  private MetadataStreamKind _metadataStreamKind;
  private MemoryBlock _metadataTableStream;
  private MetadataTableHeader _metadataTableHeader;
  private int[] _tableRowCounts;
  internal ModuleTable ModuleTable;
  internal TypeRefTable TypeRefTable;
  internal TypeDefTable TypeDefTable;
  internal FieldPtrTable FieldPtrTable;
  internal FieldTable FieldTable;
  internal MethodPtrTable MethodPtrTable;
  internal MethodTable MethodTable;
  internal ParamPtrTable ParamPtrTable;
  internal ParamTable ParamTable;
  internal InterfaceImplTable InterfaceImplTable;
  internal MemberRefTable MemberRefTable;
  internal ConstantTable ConstantTable;
  internal CustomAttributeTable CustomAttributeTable;
  internal FieldMarshalTable FieldMarshalTable;
  internal DeclSecurityTable DeclSecurityTable;
  internal ClassLayoutTable ClassLayoutTable;
  internal FieldLayoutTable FieldLayoutTable;
  internal StandAloneSigTable StandAloneSigTable;
  internal EventMapTable EventMapTable;
  internal EventPtrTable EventPtrTable;
  internal EventTable EventTable;
  internal PropertyMapTable PropertyMapTable;
  internal PropertyPtrTable PropertyPtrTable;
  internal PropertyTable PropertyTable;
  internal MethodSemanticsTable MethodSemanticsTable;
  internal MethodImplTable MethodImplTable;
  internal ModuleRefTable ModuleRefTable;
  internal TypeSpecTable TypeSpecTable;
  internal ImplMapTable ImplMapTable;
  internal FieldRVATable FieldRVATable;
  internal EnCLogTable EnCLogTable;
  internal EnCMapTable EnCMapTable;
  internal AssemblyTable AssemblyTable;
  internal AssemblyProcessorTable AssemblyProcessorTable;
  internal AssemblyOSTable AssemblyOSTable;
  internal AssemblyRefTable AssemblyRefTable;
  internal AssemblyRefProcessorTable AssemblyRefProcessorTable;
  internal AssemblyRefOSTable AssemblyRefOSTable;
  internal FileTable FileTable;
  internal ExportedTypeTable ExportedTypeTable;
  internal ManifestResourceTable ManifestResourceTable;
  internal NestedClassTable NestedClassTable;
  internal GenericParamTable GenericParamTable;
  internal MethodSpecTable MethodSpecTable;
  internal GenericParamConstraintTable GenericParamConstraintTable;

  internal MetadataImport(MemoryBlock image)
  {
    this._image = image;
    try
    {
      this.ReadPEFileLevelData();
      this.ReadCORModuleLevelData();
      this.ReadMetadataLevelData();
    }
    catch (ArgumentOutOfRangeException ex)
    {
      throw new BadImageFormatException();
    }
  }

  private void ReadOptionalHeaderDirectoryEntries(MemoryReader memReader)
  {
    memReader.SeekRelative(16 /*0x10*/);
    this._optionalHeaderDirectoryEntries.ResourceTableDirectory.RelativeVirtualAddress = memReader.ReadUInt32();
    this._optionalHeaderDirectoryEntries.ResourceTableDirectory.Size = memReader.ReadUInt32();
    memReader.SeekRelative(88);
    this._optionalHeaderDirectoryEntries.COR20HeaderTableDirectory.RelativeVirtualAddress = memReader.ReadUInt32();
    this._optionalHeaderDirectoryEntries.COR20HeaderTableDirectory.Size = memReader.ReadUInt32();
    memReader.SeekRelative(8);
  }

  private void ReadSectionHeaders(MemoryReader memReader)
  {
    if (memReader.RemainingBytes < this._numberOfSections * 40)
      throw new BadImageFormatException();
    this._sectionHeaders = new SectionHeader[this._numberOfSections];
    SectionHeader[] sectionHeaders = this._sectionHeaders;
    for (int index = 0; index < this._numberOfSections; ++index)
    {
      memReader.SeekRelative(8);
      sectionHeaders[index].VirtualSize = memReader.ReadUInt32();
      sectionHeaders[index].VirtualAddress = memReader.ReadUInt32();
      sectionHeaders[index].SizeOfRawData = memReader.ReadUInt32();
      sectionHeaders[index].OffsetToRawData = memReader.ReadUInt32();
      memReader.SeekRelative(16 /*0x10*/);
    }
  }

  private void ReadPEFileLevelData()
  {
    MemoryReader memReader = this._image.Length >= 60 ? new MemoryReader(this._image) : throw new BadImageFormatException();
    int position = this._image.ReadUInt16(0) == (ushort) 23117 ? this._image.ReadInt32(60) : throw new BadImageFormatException();
    memReader.Seek(position);
    if (memReader.ReadUInt32() != 17744U)
      throw new BadImageFormatException();
    this._numberOfSections = (int) memReader.Block.ReadUInt16(memReader.Position + 2);
    memReader.SeekRelative(20);
    switch ((PEMagic) memReader.ReadUInt16())
    {
      case PEMagic.PEMagic32:
        memReader.SeekRelative(26);
        memReader.SeekRelative(68);
        break;
      case PEMagic.PEMagic64:
        memReader.SeekRelative(22);
        memReader.SeekRelative(88);
        break;
      default:
        throw new BadImageFormatException();
    }
    this.ReadOptionalHeaderDirectoryEntries(memReader);
    this.ReadSectionHeaders(memReader);
  }

  internal MemoryBlock RvaToMemoryBlock(uint rva, uint size)
  {
    foreach (SectionHeader sectionHeader in this._sectionHeaders)
    {
      uint num1;
      if (rva >= sectionHeader.VirtualAddress && (num1 = rva - sectionHeader.VirtualAddress) < sectionHeader.VirtualSize)
      {
        uint num2;
        if (size > (num2 = sectionHeader.VirtualSize - num1))
          throw new BadImageFormatException();
        return this._image.GetRange((int) sectionHeader.OffsetToRawData + (int) num1, size == 0U ? (int) num2 : (int) size);
      }
    }
    throw new BadImageFormatException();
  }

  private MemoryBlock DirectoryToMemoryBlock(DirectoryEntry directory)
  {
    return directory.RelativeVirtualAddress == 0U || directory.Size == 0U ? (MemoryBlock) null : this.RvaToMemoryBlock(directory.RelativeVirtualAddress, directory.Size);
  }

  private void ReadCOR20Header()
  {
    MemoryBlock memoryBlock = this.DirectoryToMemoryBlock(this._optionalHeaderDirectoryEntries.COR20HeaderTableDirectory);
    if (memoryBlock == null || (long) memoryBlock.Length < (long) this._optionalHeaderDirectoryEntries.COR20HeaderTableDirectory.Size)
      throw new BadImageFormatException();
    MemoryReader memoryReader = new MemoryReader(memoryBlock);
    memoryReader.SeekRelative(8);
    this._cor20Header.MetaDataDirectory.RelativeVirtualAddress = memoryReader.ReadUInt32();
    this._cor20Header.MetaDataDirectory.Size = memoryReader.ReadUInt32();
    memoryReader.SeekRelative(8);
    this._cor20Header.ResourcesDirectory.RelativeVirtualAddress = memoryReader.ReadUInt32();
    this._cor20Header.ResourcesDirectory.Size = memoryReader.ReadUInt32();
    this._cor20Header.StrongNameSignatureDirectory.RelativeVirtualAddress = memoryReader.ReadUInt32();
    this._cor20Header.StrongNameSignatureDirectory.Size = memoryReader.ReadUInt32();
    memoryReader.SeekRelative(32 /*0x20*/);
  }

  private void ReadMetadataHeader(MemoryReader memReader)
  {
    if (memReader.ReadUInt32() != 1112167234U)
      throw new BadImageFormatException();
    memReader.SeekRelative(4);
    int offset = memReader.ReadUInt32() == 0U ? memReader.ReadInt32() : throw new BadImageFormatException();
    memReader.SeekRelative(offset);
  }

  private void ReadStorageHeader(MemoryReader memReader)
  {
    this._storageHeader.Flags = memReader.ReadUInt16();
    this._storageHeader.NumberOfStreams = memReader.ReadUInt16();
  }

  private void ReadStreamHeaders(MemoryReader memReader)
  {
    int numberOfStreams = (int) this._storageHeader.NumberOfStreams;
    this._streamHeaders = new StreamHeader[numberOfStreams];
    StreamHeader[] streamHeaders = this._streamHeaders;
    for (int index = 0; index < numberOfStreams; ++index)
    {
      streamHeaders[index].Offset = memReader.RemainingBytes >= 8 ? memReader.ReadUInt32() : throw new BadImageFormatException();
      streamHeaders[index].Size = memReader.ReadUInt32();
      streamHeaders[index].Name = memReader.ReadAscii(32 /*0x20*/);
      memReader.Align(4);
    }
  }

  private void ProcessAndCacheStreams(MemoryBlock metadataRoot)
  {
    this._metadataStreamKind = MetadataStreamKind.Illegal;
    foreach (StreamHeader streamHeader in this._streamHeaders)
    {
      if ((long) streamHeader.Offset + (long) streamHeader.Size > (long) metadataRoot.Length)
        throw new BadImageFormatException();
      MemoryBlock range = metadataRoot.GetRange((int) streamHeader.Offset, (int) streamHeader.Size);
      switch (streamHeader.Name)
      {
        case "#Strings":
          if (this._stringStream != null)
            throw new BadImageFormatException();
          this._stringStream = range.Length != 0 && range.ReadByte(0) == (byte) 0 && range.ReadByte(range.Length - 1) == (byte) 0 ? range : throw new BadImageFormatException();
          break;
        case "#Blob":
          this._blobStream = this._blobStream == null ? range : throw new BadImageFormatException();
          break;
        case "#GUID":
          this._guidStream = this._guidStream == null ? range : throw new BadImageFormatException();
          break;
        case "#US":
          this._userStringStream = this._userStringStream == null ? range : throw new BadImageFormatException();
          break;
        case "#~":
          this._metadataStreamKind = this._metadataStreamKind == MetadataStreamKind.Illegal ? MetadataStreamKind.Compressed : throw new BadImageFormatException();
          this._metadataTableStream = range;
          break;
        case "#-":
          this._metadataStreamKind = this._metadataStreamKind == MetadataStreamKind.Illegal ? MetadataStreamKind.UnCompressed : throw new BadImageFormatException();
          this._metadataTableStream = range;
          break;
        default:
          throw new BadImageFormatException();
      }
    }
    if (this._stringStream == null || this._guidStream == null || this._metadataStreamKind == MetadataStreamKind.Illegal)
      throw new BadImageFormatException();
  }

  private void ReadCORModuleLevelData()
  {
    this.ReadCOR20Header();
    MemoryBlock memoryBlock = this.DirectoryToMemoryBlock(this._cor20Header.MetaDataDirectory);
    if (memoryBlock == null || (long) memoryBlock.Length < (long) this._cor20Header.MetaDataDirectory.Size)
      throw new BadImageFormatException();
    MemoryReader memReader = new MemoryReader(memoryBlock);
    this.ReadMetadataHeader(memReader);
    this.ReadStorageHeader(memReader);
    this.ReadStreamHeaders(memReader);
    this.ProcessAndCacheStreams(memoryBlock);
  }

  internal bool IsManifestModule => this.AssemblyTable.NumberOfRows == 1;

  internal bool UseFieldPtrTable => this.FieldPtrTable.NumberOfRows > 0;

  internal bool UseMethodPtrTable => this.MethodPtrTable.NumberOfRows > 0;

  internal bool UseParamPtrTable => this.ParamPtrTable.NumberOfRows > 0;

  internal bool UseEventPtrTable => this.EventPtrTable.NumberOfRows > 0;

  internal bool UsePropertyPtrTable => this.PropertyPtrTable.NumberOfRows > 0;

  private void ReadMetadataTableInformation(MemoryReader memReader)
  {
    if (memReader.RemainingBytes < 24)
      throw new BadImageFormatException();
    memReader.SeekRelative(4);
    this._metadataTableHeader.MajorVersion = memReader.ReadByte();
    this._metadataTableHeader.MinorVersion = memReader.ReadByte();
    this._metadataTableHeader.HeapSizeFlags = (HeapSizeFlag) memReader.ReadByte();
    memReader.SeekRelative(1);
    this._metadataTableHeader.ValidTables = (TableMask) memReader.ReadUInt64();
    this._metadataTableHeader.SortedTables = (TableMask) memReader.ReadUInt64();
    ulong validTables = (ulong) this._metadataTableHeader.ValidTables;
    ulong num1;
    switch ((int) this._metadataTableHeader.MajorVersion << 8 | (int) this._metadataTableHeader.MinorVersion)
    {
      case 256 /*0x0100*/:
        num1 = 4166118277119UL;
        break;
      case 257:
        num1 = 4166118277119UL;
        break;
      case 512 /*0x0200*/:
        num1 = 34952443854847UL;
        break;
      default:
        throw new BadImageFormatException();
    }
    if (((long) validTables & ~(long) num1) != 0L)
      throw new BadImageFormatException();
    if (this._metadataStreamKind == MetadataStreamKind.Compressed && ((long) validTables & 3225944232L) != 0L)
      throw new BadImageFormatException();
    ulong num2 = (ulong) ((long) validTables & (long) num1 & 24190111578624L);
    if (((TableMask) num2 & this._metadataTableHeader.SortedTables) != (TableMask) num2)
      throw new BadImageFormatException();
    int numberOfTablesPresent = this._metadataTableHeader.GetNumberOfTablesPresent();
    if (memReader.RemainingBytes < numberOfTablesPresent * 4)
      throw new BadImageFormatException();
    int[] numArray = this._metadataTableHeader.CompressedMetadataTableRowCount = new int[numberOfTablesPresent];
    for (int index = 0; index < numberOfTablesPresent; ++index)
    {
      uint num3 = memReader.ReadUInt32();
      numArray[index] = num3 <= 16777215U /*0xFFFFFF*/ ? (int) num3 : throw new BadImageFormatException();
    }
  }

  private static int ComputeCodedTokenSize(
    int largeRowSize,
    int[] rowCountArray,
    TableMask tablesReferenced)
  {
    bool flag = true;
    ulong num = (ulong) tablesReferenced;
    for (int index = 0; index < 45; ++index)
    {
      if (((long) num & 1L) != 0L)
        flag &= rowCountArray[index] < largeRowSize;
      num >>= 1;
    }
    return !flag ? 4 : 2;
  }

  private void ProcessAndCacheMetadataTableBlocks(MemoryBlock metadataTablesMemoryBlock)
  {
    int[] rowCountArray = this._tableRowCounts = new int[45];
    int[] numArray = new int[45];
    int[] metadataTableRowCount = this._metadataTableHeader.CompressedMetadataTableRowCount;
    ulong validTables = (ulong) this._metadataTableHeader.ValidTables;
    int index = 0;
    int num1 = 0;
    for (; index < numArray.Length; ++index)
    {
      if (((long) validTables & 1L) != 0L)
      {
        int num2 = metadataTableRowCount[num1++];
        rowCountArray[index] = num2;
        numArray[index] = num2 < 65536 /*0x010000*/ ? 2 : 4;
      }
      else
        numArray[index] = 2;
      validTables >>= 1;
    }
    int fieldRefSize = numArray[3] > 2 ? 4 : numArray[3];
    int methodRefSize = numArray[5] > 2 ? 4 : numArray[5];
    int paramRefSize = numArray[7] > 2 ? 4 : numArray[7];
    int eventRefSize = numArray[19] > 2 ? 4 : numArray[19];
    int propertyRefSize = numArray[22] > 2 ? 4 : numArray[22];
    int codedTokenSize1 = MetadataImport.ComputeCodedTokenSize(16384 /*0x4000*/, rowCountArray, TableMask.TypeRef | TableMask.TypeDef | TableMask.TypeSpec);
    int codedTokenSize2 = MetadataImport.ComputeCodedTokenSize(16384 /*0x4000*/, rowCountArray, TableMask.Field | TableMask.Param | TableMask.Property);
    int codedTokenSize3 = MetadataImport.ComputeCodedTokenSize(2048 /*0x0800*/, rowCountArray, TableMask.Module | TableMask.TypeRef | TableMask.TypeDef | TableMask.Field | TableMask.Method | TableMask.Param | TableMask.InterfaceImpl | TableMask.MemberRef | TableMask.DeclSecurity | TableMask.StandAloneSig | TableMask.Event | TableMask.Property | TableMask.ModuleRef | TableMask.TypeSpec | TableMask.Assembly | TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType | TableMask.ManifestResource | TableMask.GenericParam);
    int codedTokenSize4 = MetadataImport.ComputeCodedTokenSize(32768 /*0x8000*/, rowCountArray, TableMask.Field | TableMask.Param);
    int codedTokenSize5 = MetadataImport.ComputeCodedTokenSize(16384 /*0x4000*/, rowCountArray, TableMask.TypeDef | TableMask.Method | TableMask.Assembly);
    int codedTokenSize6 = MetadataImport.ComputeCodedTokenSize(8192 /*0x2000*/, rowCountArray, TableMask.TypeRef | TableMask.TypeDef | TableMask.Method | TableMask.ModuleRef | TableMask.TypeSpec);
    int codedTokenSize7 = MetadataImport.ComputeCodedTokenSize(32768 /*0x8000*/, rowCountArray, TableMask.Event | TableMask.Property);
    int codedTokenSize8 = MetadataImport.ComputeCodedTokenSize(32768 /*0x8000*/, rowCountArray, TableMask.Method | TableMask.MemberRef);
    int codedTokenSize9 = MetadataImport.ComputeCodedTokenSize(32768 /*0x8000*/, rowCountArray, TableMask.Field | TableMask.Method);
    int codedTokenSize10 = MetadataImport.ComputeCodedTokenSize(16384 /*0x4000*/, rowCountArray, TableMask.AssemblyRef | TableMask.File | TableMask.ExportedType);
    int codedTokenSize11 = MetadataImport.ComputeCodedTokenSize(8192 /*0x2000*/, rowCountArray, TableMask.Method | TableMask.MemberRef);
    int codedTokenSize12 = MetadataImport.ComputeCodedTokenSize(16384 /*0x4000*/, rowCountArray, TableMask.Module | TableMask.TypeRef | TableMask.ModuleRef | TableMask.AssemblyRef);
    int codedTokenSize13 = MetadataImport.ComputeCodedTokenSize(32768 /*0x8000*/, rowCountArray, TableMask.TypeDef | TableMask.Method);
    int stringHeapRefSize = (this._metadataTableHeader.HeapSizeFlags & HeapSizeFlag.StringHeapLarge) == HeapSizeFlag.StringHeapLarge ? 4 : 2;
    int guidHeapRefSize = (this._metadataTableHeader.HeapSizeFlags & HeapSizeFlag.GUIDHeapLarge) == HeapSizeFlag.GUIDHeapLarge ? 4 : 2;
    int blobHeapRefSize = (this._metadataTableHeader.HeapSizeFlags & HeapSizeFlag.BlobHeapLarge) == HeapSizeFlag.BlobHeapLarge ? 4 : 2;
    int start1 = 0;
    this.ModuleTable = new ModuleTable(rowCountArray[0], stringHeapRefSize, guidHeapRefSize, start1, metadataTablesMemoryBlock);
    int length1 = this.ModuleTable.Table.Length;
    int num3 = 0 + length1;
    int start2 = start1 + length1;
    this.TypeRefTable = new TypeRefTable(rowCountArray[1], codedTokenSize12, stringHeapRefSize, start2, metadataTablesMemoryBlock);
    int length2 = this.TypeRefTable.Table.Length;
    int num4 = length2;
    int num5 = num3 + num4;
    int start3 = start2 + length2;
    this.TypeDefTable = new TypeDefTable(rowCountArray[2], fieldRefSize, methodRefSize, codedTokenSize1, stringHeapRefSize, start3, metadataTablesMemoryBlock);
    int length3 = this.TypeDefTable.Table.Length;
    int num6 = length3;
    int num7 = num5 + num6;
    int start4 = start3 + length3;
    this.FieldPtrTable = new FieldPtrTable(rowCountArray[3], numArray[3], start4, metadataTablesMemoryBlock);
    int length4 = this.FieldPtrTable.Table.Length;
    int num8 = length4;
    int num9 = num7 + num8;
    int start5 = start4 + length4;
    this.FieldTable = new FieldTable(rowCountArray[4], stringHeapRefSize, blobHeapRefSize, start5, metadataTablesMemoryBlock);
    int length5 = this.FieldTable.Table.Length;
    int num10 = length5;
    int num11 = num9 + num10;
    int start6 = start5 + length5;
    this.MethodPtrTable = new MethodPtrTable(rowCountArray[5], numArray[5], start6, metadataTablesMemoryBlock);
    int length6 = this.MethodPtrTable.Table.Length;
    int num12 = length6;
    int num13 = num11 + num12;
    int start7 = start6 + length6;
    this.MethodTable = new MethodTable(rowCountArray[6], paramRefSize, stringHeapRefSize, blobHeapRefSize, start7, metadataTablesMemoryBlock);
    int length7 = this.MethodTable.Table.Length;
    int num14 = length7;
    int num15 = num13 + num14;
    int start8 = start7 + length7;
    this.ParamPtrTable = new ParamPtrTable(rowCountArray[7], numArray[7], start8, metadataTablesMemoryBlock);
    int length8 = this.ParamPtrTable.Table.Length;
    int num16 = length8;
    int num17 = num15 + num16;
    int start9 = start8 + length8;
    this.ParamTable = new ParamTable(rowCountArray[8], stringHeapRefSize, start9, metadataTablesMemoryBlock);
    int length9 = this.ParamTable.Table.Length;
    int num18 = length9;
    int num19 = num17 + num18;
    int start10 = start9 + length9;
    this.InterfaceImplTable = new InterfaceImplTable(rowCountArray[9], numArray[9], codedTokenSize1, start10, metadataTablesMemoryBlock);
    int length10 = this.InterfaceImplTable.Table.Length;
    int num20 = length10;
    int num21 = num19 + num20;
    int start11 = start10 + length10;
    this.MemberRefTable = new MemberRefTable(rowCountArray[10], codedTokenSize6, stringHeapRefSize, blobHeapRefSize, start11, metadataTablesMemoryBlock);
    int length11 = this.MemberRefTable.Table.Length;
    int num22 = length11;
    int num23 = num21 + num22;
    int start12 = start11 + length11;
    this.ConstantTable = new ConstantTable(rowCountArray[11], codedTokenSize2, blobHeapRefSize, start12, metadataTablesMemoryBlock);
    int length12 = this.ConstantTable.Table.Length;
    int num24 = length12;
    int num25 = num23 + num24;
    int start13 = start12 + length12;
    this.CustomAttributeTable = new CustomAttributeTable(rowCountArray[12], codedTokenSize3, codedTokenSize11, blobHeapRefSize, start13, metadataTablesMemoryBlock);
    int length13 = this.CustomAttributeTable.Table.Length;
    int num26 = length13;
    int num27 = num25 + num26;
    int start14 = start13 + length13;
    this.FieldMarshalTable = new FieldMarshalTable(rowCountArray[13], codedTokenSize4, blobHeapRefSize, start14, metadataTablesMemoryBlock);
    int length14 = this.FieldMarshalTable.Table.Length;
    int num28 = length14;
    int num29 = num27 + num28;
    int start15 = start14 + length14;
    this.DeclSecurityTable = new DeclSecurityTable(rowCountArray[14], codedTokenSize5, blobHeapRefSize, start15, metadataTablesMemoryBlock);
    int length15 = this.DeclSecurityTable.Table.Length;
    int num30 = length15;
    int num31 = num29 + num30;
    int start16 = start15 + length15;
    this.ClassLayoutTable = new ClassLayoutTable(rowCountArray[15], numArray[15], start16, metadataTablesMemoryBlock);
    int length16 = this.ClassLayoutTable.Table.Length;
    int num32 = length16;
    int num33 = num31 + num32;
    int start17 = start16 + length16;
    this.FieldLayoutTable = new FieldLayoutTable(rowCountArray[16 /*0x10*/], numArray[16 /*0x10*/], start17, metadataTablesMemoryBlock);
    int length17 = this.FieldLayoutTable.Table.Length;
    int num34 = length17;
    int num35 = num33 + num34;
    int start18 = start17 + length17;
    this.StandAloneSigTable = new StandAloneSigTable(rowCountArray[17], blobHeapRefSize, start18, metadataTablesMemoryBlock);
    int length18 = this.StandAloneSigTable.Table.Length;
    int num36 = length18;
    int num37 = num35 + num36;
    int start19 = start18 + length18;
    this.EventMapTable = new EventMapTable(rowCountArray[18], numArray[18], eventRefSize, start19, metadataTablesMemoryBlock);
    int length19 = this.EventMapTable.Table.Length;
    int num38 = length19;
    int num39 = num37 + num38;
    int start20 = start19 + length19;
    this.EventPtrTable = new EventPtrTable(rowCountArray[19], numArray[19], start20, metadataTablesMemoryBlock);
    int length20 = this.EventPtrTable.Table.Length;
    int num40 = length20;
    int num41 = num39 + num40;
    int start21 = start20 + length20;
    this.EventTable = new EventTable(rowCountArray[20], codedTokenSize1, stringHeapRefSize, start21, metadataTablesMemoryBlock);
    int length21 = this.EventTable.Table.Length;
    int num42 = length21;
    int num43 = num41 + num42;
    int start22 = start21 + length21;
    this.PropertyMapTable = new PropertyMapTable(rowCountArray[21], numArray[21], propertyRefSize, start22, metadataTablesMemoryBlock);
    int length22 = this.PropertyMapTable.Table.Length;
    int num44 = length22;
    int num45 = num43 + num44;
    int start23 = start22 + length22;
    this.PropertyPtrTable = new PropertyPtrTable(rowCountArray[22], numArray[22], start23, metadataTablesMemoryBlock);
    int length23 = this.PropertyPtrTable.Table.Length;
    int num46 = length23;
    int num47 = num45 + num46;
    int start24 = start23 + length23;
    this.PropertyTable = new PropertyTable(rowCountArray[23], stringHeapRefSize, blobHeapRefSize, start24, metadataTablesMemoryBlock);
    int length24 = this.PropertyTable.Table.Length;
    int num48 = length24;
    int num49 = num47 + num48;
    int start25 = start24 + length24;
    this.MethodSemanticsTable = new MethodSemanticsTable(rowCountArray[24], numArray[24], codedTokenSize7, start25, metadataTablesMemoryBlock);
    int length25 = this.MethodSemanticsTable.Table.Length;
    int num50 = length25;
    int num51 = num49 + num50;
    int start26 = start25 + length25;
    this.MethodImplTable = new MethodImplTable(rowCountArray[25], numArray[25], codedTokenSize8, start26, metadataTablesMemoryBlock);
    int length26 = this.MethodImplTable.Table.Length;
    int num52 = length26;
    int num53 = num51 + num52;
    int start27 = start26 + length26;
    this.ModuleRefTable = new ModuleRefTable(rowCountArray[26], stringHeapRefSize, start27, metadataTablesMemoryBlock);
    int length27 = this.ModuleRefTable.Table.Length;
    int num54 = length27;
    int num55 = num53 + num54;
    int start28 = start27 + length27;
    this.TypeSpecTable = new TypeSpecTable(rowCountArray[27], blobHeapRefSize, start28, metadataTablesMemoryBlock);
    int length28 = this.TypeSpecTable.Table.Length;
    int num56 = length28;
    int num57 = num55 + num56;
    int start29 = start28 + length28;
    this.ImplMapTable = new ImplMapTable(rowCountArray[28], numArray[28], codedTokenSize9, stringHeapRefSize, start29, metadataTablesMemoryBlock);
    int length29 = this.ImplMapTable.Table.Length;
    int num58 = length29;
    int num59 = num57 + num58;
    int start30 = start29 + length29;
    this.FieldRVATable = new FieldRVATable(rowCountArray[29], numArray[29], start30, metadataTablesMemoryBlock);
    int length30 = this.FieldRVATable.Table.Length;
    int num60 = length30;
    int num61 = num59 + num60;
    int start31 = start30 + length30;
    this.EnCLogTable = new EnCLogTable(rowCountArray[30], start31, metadataTablesMemoryBlock);
    int length31 = this.EnCLogTable.Table.Length;
    int num62 = length31;
    int num63 = num61 + num62;
    int start32 = start31 + length31;
    this.EnCMapTable = new EnCMapTable(rowCountArray[31 /*0x1F*/], start32, metadataTablesMemoryBlock);
    int length32 = this.EnCMapTable.Table.Length;
    int num64 = length32;
    int num65 = num63 + num64;
    int start33 = start32 + length32;
    this.AssemblyTable = new AssemblyTable(rowCountArray[32 /*0x20*/], stringHeapRefSize, blobHeapRefSize, start33, metadataTablesMemoryBlock);
    int length33 = this.AssemblyTable.Table.Length;
    int num66 = length33;
    int num67 = num65 + num66;
    int start34 = start33 + length33;
    this.AssemblyProcessorTable = new AssemblyProcessorTable(rowCountArray[33], start34, metadataTablesMemoryBlock);
    int length34 = this.AssemblyProcessorTable.Table.Length;
    int num68 = length34;
    int num69 = num67 + num68;
    int start35 = start34 + length34;
    this.AssemblyOSTable = new AssemblyOSTable(rowCountArray[34], start35, metadataTablesMemoryBlock);
    int length35 = this.AssemblyOSTable.Table.Length;
    int num70 = length35;
    int num71 = num69 + num70;
    int start36 = start35 + length35;
    this.AssemblyRefTable = new AssemblyRefTable(rowCountArray[35], stringHeapRefSize, blobHeapRefSize, start36, metadataTablesMemoryBlock);
    int length36 = this.AssemblyRefTable.Table.Length;
    int num72 = length36;
    int num73 = num71 + num72;
    int start37 = start36 + length36;
    this.AssemblyRefProcessorTable = new AssemblyRefProcessorTable(rowCountArray[36], numArray[36], start37, metadataTablesMemoryBlock);
    int length37 = this.AssemblyRefProcessorTable.Table.Length;
    int num74 = length37;
    int num75 = num73 + num74;
    int start38 = start37 + length37;
    this.AssemblyRefOSTable = new AssemblyRefOSTable(rowCountArray[37], numArray[37], start38, metadataTablesMemoryBlock);
    int length38 = this.AssemblyRefOSTable.Table.Length;
    int num76 = length38;
    int num77 = num75 + num76;
    int start39 = start38 + length38;
    this.FileTable = new FileTable(rowCountArray[38], stringHeapRefSize, blobHeapRefSize, start39, metadataTablesMemoryBlock);
    int length39 = this.FileTable.Table.Length;
    int num78 = length39;
    int num79 = num77 + num78;
    int start40 = start39 + length39;
    this.ExportedTypeTable = new ExportedTypeTable(rowCountArray[39], codedTokenSize10, stringHeapRefSize, start40, metadataTablesMemoryBlock);
    int length40 = this.ExportedTypeTable.Table.Length;
    int num80 = length40;
    int num81 = num79 + num80;
    int start41 = start40 + length40;
    this.ManifestResourceTable = new ManifestResourceTable(rowCountArray[40], codedTokenSize10, stringHeapRefSize, start41, metadataTablesMemoryBlock);
    int length41 = this.ManifestResourceTable.Table.Length;
    int num82 = length41;
    int num83 = num81 + num82;
    int start42 = start41 + length41;
    this.NestedClassTable = new NestedClassTable(rowCountArray[41], numArray[41], start42, metadataTablesMemoryBlock);
    int length42 = this.NestedClassTable.Table.Length;
    int num84 = length42;
    int num85 = num83 + num84;
    int start43 = start42 + length42;
    this.GenericParamTable = new GenericParamTable(rowCountArray[42], codedTokenSize13, stringHeapRefSize, start43, metadataTablesMemoryBlock);
    int length43 = this.GenericParamTable.Table.Length;
    int num86 = length43;
    int num87 = num85 + num86;
    int start44 = start43 + length43;
    this.MethodSpecTable = new MethodSpecTable(rowCountArray[43], codedTokenSize8, blobHeapRefSize, start44, metadataTablesMemoryBlock);
    int length44 = this.MethodSpecTable.Table.Length;
    int num88 = length44;
    int num89 = num87 + num88;
    int start45 = start44 + length44;
    this.GenericParamConstraintTable = new GenericParamConstraintTable(rowCountArray[44], numArray[44], codedTokenSize1, start45, metadataTablesMemoryBlock);
    int length45 = this.GenericParamConstraintTable.Table.Length;
    int num90 = start45 + length45;
  }

  private void ReadMetadataLevelData()
  {
    MemoryReader memReader = new MemoryReader(this._metadataTableStream);
    this.ReadMetadataTableInformation(memReader);
    this.ProcessAndCacheMetadataTableBlocks(memReader.GetRemainingBlock());
    if (this.ModuleTable.NumberOfRows != 1)
      throw new BadImageFormatException();
  }

  internal byte[] GetBlob(uint blob)
  {
    int size;
    int blobDataOffset = this.GetBlobDataOffset(blob, out size);
    byte[] result = new byte[size];
    this._blobStream.Read(blobDataOffset, result);
    return result;
  }

  internal MemoryBlock GetBlobBlock(uint blob)
  {
    int size;
    return this._blobStream.GetRange(this.GetBlobDataOffset(blob, out size), size);
  }

  internal int GetBlobDataOffset(uint blob, out int size)
  {
    if (this._blobStream == null || (long) blob >= (long) this._blobStream.Length)
      throw new BadImageFormatException();
    int offset = (int) blob;
    int numberOfBytesRead;
    size = this._blobStream.ReadCompressedInt32(offset, out numberOfBytesRead);
    if (offset > this._blobStream.Length - numberOfBytesRead - size)
      throw new BadImageFormatException();
    return offset + numberOfBytesRead;
  }

  internal object GetBlobValue(uint blob, ElementType type)
  {
    int size;
    int blobDataOffset = this.GetBlobDataOffset(blob, out size);
    if (size < MetadataImport.GetMinTypeSize(type))
      throw new BadImageFormatException();
    switch (type)
    {
      case ElementType.Void:
        return (object) DBNull.Value;
      case ElementType.Boolean:
        return (object) (this._blobStream.ReadByte(blobDataOffset) > (byte) 0);
      case ElementType.Char:
        return (object) (char) this._blobStream.ReadUInt16(blobDataOffset);
      case ElementType.Int8:
        return (object) this._blobStream.ReadSByte(blobDataOffset);
      case ElementType.UInt8:
        return (object) this._blobStream.ReadByte(blobDataOffset);
      case ElementType.Int16:
        return (object) this._blobStream.ReadInt16(blobDataOffset);
      case ElementType.UInt16:
        return (object) this._blobStream.ReadUInt16(blobDataOffset);
      case ElementType.Int32:
        return (object) this._blobStream.ReadInt32(blobDataOffset);
      case ElementType.UInt32:
        return (object) this._blobStream.ReadUInt32(blobDataOffset);
      case ElementType.Int64:
        return (object) this._blobStream.ReadInt64(blobDataOffset);
      case ElementType.UInt64:
        return (object) this._blobStream.ReadUInt64(blobDataOffset);
      case ElementType.Single:
        return (object) this._blobStream.ReadSingle(blobDataOffset);
      case ElementType.Double:
        return (object) this._blobStream.ReadDouble(blobDataOffset);
      case ElementType.String:
        return (object) this._blobStream.ReadUtf16(blobDataOffset, size);
      case ElementType.Class:
        if (this._blobStream.ReadUInt32(blobDataOffset) != 0U)
          throw new BadImageFormatException();
        return (object) null;
      default:
        throw new BadImageFormatException();
    }
  }

  private static int GetMinTypeSize(ElementType type)
  {
    switch (type)
    {
      case ElementType.Void:
      case ElementType.String:
        return 0;
      case ElementType.Boolean:
      case ElementType.Int8:
      case ElementType.UInt8:
        return 1;
      case ElementType.Char:
      case ElementType.Int16:
      case ElementType.UInt16:
        return 2;
      case ElementType.Int32:
      case ElementType.UInt32:
      case ElementType.Single:
      case ElementType.Class:
        return 2;
      case ElementType.Int64:
      case ElementType.UInt64:
      case ElementType.Double:
        return 8;
      default:
        return int.MaxValue;
    }
  }

  internal Guid GetGuid(uint blob)
  {
    if ((long) (blob - 1U) > (long) (this._guidStream.Length - 16 /*0x10*/))
      throw new BadImageFormatException();
    return blob == 0U ? Guid.Empty : this._guidStream.ReadGuid((int) blob - 1);
  }

  internal int GetFieldRange(int typeDefRid, out int count)
  {
    int rowCount = this.UseFieldPtrTable ? this.FieldPtrTable.NumberOfRows : this.FieldTable.NumberOfRows;
    uint firstFieldRid = this.TypeDefTable.GetFirstFieldRid(typeDefRid);
    uint nextStart = typeDefRid == this.TypeDefTable.NumberOfRows ? (uint) (rowCount + 1) : this.TypeDefTable.GetFirstFieldRid(typeDefRid + 1);
    count = this.GetRangeCount(rowCount, firstFieldRid, nextStart);
    return (int) firstFieldRid;
  }

  internal int GetMethodRange(int typeDefRid, out int count)
  {
    int rowCount = this.UseMethodPtrTable ? this.MethodPtrTable.NumberOfRows : this.MethodTable.NumberOfRows;
    uint firstMethodRid = this.TypeDefTable.GetFirstMethodRid(typeDefRid);
    uint nextStart = typeDefRid == this.TypeDefTable.NumberOfRows ? (uint) (rowCount + 1) : this.TypeDefTable.GetFirstMethodRid(typeDefRid + 1);
    count = this.GetRangeCount(rowCount, firstMethodRid, nextStart);
    return (int) firstMethodRid;
  }

  internal int GetEventRange(int typeDefRid, out int count)
  {
    int eventMapRowIdFor = this.EventMapTable.FindEventMapRowIdFor(typeDefRid);
    if (eventMapRowIdFor == 0)
    {
      count = 0;
      return 0;
    }
    int rowCount = this.UseEventPtrTable ? this.EventPtrTable.NumberOfRows : this.EventTable.NumberOfRows;
    uint eventListStartFor = this.EventMapTable.GetEventListStartFor(eventMapRowIdFor);
    uint nextStart = eventMapRowIdFor == this.EventMapTable.NumberOfRows ? (uint) (rowCount + 1) : this.EventMapTable.GetEventListStartFor(eventMapRowIdFor + 1);
    count = this.GetRangeCount(rowCount, eventListStartFor, nextStart);
    return (int) eventListStartFor;
  }

  internal int GetPropertyRange(int typeDefRid, out int count)
  {
    int propertyMapRowIdFor = this.PropertyMapTable.FindPropertyMapRowIdFor(typeDefRid);
    if (propertyMapRowIdFor == 0)
    {
      count = 0;
      return 0;
    }
    int rowCount = this.UsePropertyPtrTable ? this.PropertyPtrTable.NumberOfRows : this.PropertyTable.NumberOfRows;
    uint firstPropertyRid = this.PropertyMapTable.GetFirstPropertyRid(propertyMapRowIdFor);
    uint nextStart = propertyMapRowIdFor == this.PropertyMapTable.NumberOfRows ? (uint) (rowCount + 1) : this.PropertyMapTable.GetFirstPropertyRid(propertyMapRowIdFor + 1);
    count = this.GetRangeCount(rowCount, firstPropertyRid, nextStart);
    return (int) firstPropertyRid;
  }

  private int GetRangeCount(int rowCount, uint start, uint nextStart)
  {
    if (start == 0U)
      return 0;
    if ((long) start > (long) (rowCount + 1) || (long) nextStart > (long) (rowCount + 1) || start > nextStart)
      throw new BadImageFormatException();
    return (int) nextStart - (int) start;
  }

  internal int GetParamRange(int methodDefRid, out int count)
  {
    int rowCount = this.UseParamPtrTable ? this.ParamPtrTable.NumberOfRows : this.ParamTable.NumberOfRows;
    uint firstParamRid = this.MethodTable.GetFirstParamRid(methodDefRid);
    uint nextStart = methodDefRid == this.MethodTable.NumberOfRows ? (uint) (rowCount + 1) : this.MethodTable.GetFirstParamRid(methodDefRid + 1);
    count = this.GetRangeCount(rowCount, firstParamRid, nextStart);
    return (int) firstParamRid;
  }

  internal EnumerationIndirection GetEnumeratorRange(
    MetadataTokenType type,
    MetadataToken parent,
    out int startRid,
    out int count)
  {
    switch (type)
    {
      case MetadataTokenType.TypeRef:
      case MetadataTokenType.TypeDef:
      case MetadataTokenType.MemberRef:
      case MetadataTokenType.Signature:
      case MetadataTokenType.ModuleRef:
      case MetadataTokenType.TypeSpec:
      case MetadataTokenType.AssemblyRef:
      case MetadataTokenType.File:
      case MetadataTokenType.ExportedType:
      case MetadataTokenType.ManifestResource:
      case MetadataTokenType.NestedClass:
      case MetadataTokenType.MethodSpec:
        count = this._tableRowCounts[(int) type >> 24];
        startRid = 1;
        return EnumerationIndirection.None;
      case MetadataTokenType.FieldDef:
        if (parent.IsNull)
        {
          count = this.FieldTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.GetFieldRange(parent.Rid, out count);
        return !this.UseFieldPtrTable ? EnumerationIndirection.None : EnumerationIndirection.Field;
      case MetadataTokenType.MethodDef:
        if (parent.IsNull)
        {
          count = this.MethodTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.GetMethodRange(parent.Rid, out count);
        return !this.UseParamPtrTable ? EnumerationIndirection.None : EnumerationIndirection.Method;
      case MetadataTokenType.ParamDef:
        if (parent.IsNull)
        {
          count = this.ParamTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.GetParamRange(parent.Rid, out count);
        return !this.UseParamPtrTable ? EnumerationIndirection.None : EnumerationIndirection.Param;
      case MetadataTokenType.InterfaceImpl:
        if (parent.IsNull)
        {
          count = this.InterfaceImplTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.InterfaceImplTable.FindInterfaceImplForType(parent.Rid, out count);
        return EnumerationIndirection.None;
      case MetadataTokenType.CustomAttribute:
        if (parent.IsNull)
        {
          count = this.CustomAttributeTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.CustomAttributeTable.FindCustomAttributesForToken(parent, out count);
        return EnumerationIndirection.None;
      case MetadataTokenType.Event:
        if (parent.IsNull)
        {
          count = this.EventTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.GetEventRange(parent.Rid, out count);
        return !this.UseEventPtrTable ? EnumerationIndirection.None : EnumerationIndirection.Event;
      case MetadataTokenType.Property:
        if (parent.IsNull)
        {
          count = this.PropertyTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.GetPropertyRange(parent.Rid, out count);
        return !this.UsePropertyPtrTable ? EnumerationIndirection.None : EnumerationIndirection.Property;
      case MetadataTokenType.GenericPar:
        if (parent.IsNull)
        {
          count = this.GenericParamTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = !parent.IsTypeDef ? this.GenericParamTable.FindGenericParametersForMethod(parent.Rid, out count) : this.GenericParamTable.FindGenericParametersForType(parent.Rid, out count);
        return EnumerationIndirection.None;
      case MetadataTokenType.GenericParamConstraint:
        if (parent.IsNull)
        {
          count = this.GenericParamConstraintTable.NumberOfRows;
          startRid = 1;
        }
        else
          startRid = this.GenericParamConstraintTable.FindConstraintForGenericParam(parent.Rid, out count);
        return EnumerationIndirection.None;
      default:
        throw new InvalidOperationException();
    }
  }

  internal bool IsValidToken(MetadataToken token)
  {
    int recordType = (int) token.RecordType;
    if (recordType < this._tableRowCounts.Length)
      return token.Rid <= this._tableRowCounts[recordType];
    switch (recordType)
    {
      case 112 /*0x70*/:
        return token.Rid < this._stringStream.Length;
      case 113:
        return this._userStringStream != null && token.Rid < this._userStringStream.Length;
      default:
        return false;
    }
  }

  internal int GetRowCount(int tableIndex) => this._tableRowCounts[tableIndex];

  internal MetadataName GetMetadataName(uint blob) => this._stringStream.ReadName(blob);

  internal object GetDefaultValue(MetadataToken token)
  {
    int constantRowId = this.ConstantTable.GetConstantRowId(token);
    ElementType type;
    return constantRowId == 0 ? (object) Missing.Value : this.GetBlobValue(this.ConstantTable.GetValue(constantRowId, out type), type);
  }

  internal MemoryBlock Image => this._image;
}
