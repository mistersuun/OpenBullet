// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataTables
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Metadata;

public sealed class MetadataTables
{
  internal readonly MetadataImport m_import;
  internal readonly string m_path;

  internal MetadataTables(MetadataImport import, string path, Module module)
  {
    this.m_import = import;
    this.m_path = path;
    this.Module = module;
  }

  public Module Module { get; }

  public bool IsValidToken(MetadataToken token) => this.m_import.IsValidToken(token);

  internal MetadataName ToMetadataName(uint blob) => this.m_import.GetMetadataName(blob);

  public static MetadataTables OpenFile(string path)
  {
    return path != null ? new MetadataTables(MetadataTables.CreateImport(path), path, (Module) null) : throw new ArgumentNullException(nameof (path));
  }

  public static MetadataTables OpenModule(Module module)
  {
    return !(module == (Module) null) ? new MetadataTables(MetadataTables.CreateImport(module.FullyQualifiedName), (string) null, module) : throw new ArgumentNullException(nameof (module));
  }

  private static MetadataImport CreateImport(string path)
  {
    MemoryMapping memoryMapping = MemoryMapping.Create(path);
    return new MetadataImport(memoryMapping.GetRange(0, (int) Math.Min(memoryMapping.Capacity, (long) int.MaxValue)));
  }

  public int GetRowCount(int tableIndex) => this.m_import.GetRowCount(tableIndex);

  internal int GetRowCount(MetadataRecordType tableIndex)
  {
    return this.m_import.GetRowCount((int) tableIndex);
  }

  public string Path
  {
    get => !(this.Module != (Module) null) ? this.m_path : this.Module.Assembly.Location;
  }

  public ModuleDef ModuleDef
  {
    get => new MetadataRecord(new MetadataToken(MetadataTokenType.Module, 1), this).ModuleDef;
  }

  public AssemblyDef AssemblyDef
  {
    get
    {
      switch (this.GetRowCount(32 /*0x20*/))
      {
        case 0:
          return MetadataRecord.Null(this).AssemblyDef;
        case 1:
          return new MetadataRecord(new MetadataToken(MetadataTokenType.Assembly, 1), this).AssemblyDef;
        default:
          throw new BadImageFormatException();
      }
    }
  }

  public MetadataTableView AssemblyRefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.AssemblyRef);
  }

  public MetadataTableView ModuleRefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.ModuleRef);
  }

  public MetadataTableView Files
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.File);
  }

  public MetadataTableView ManifestResources
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.ManifestResource);
  }

  public MetadataTableView TypeDefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.TypeDef);
  }

  public MetadataTableView TypeSpecs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.TypeSpec);
  }

  public MetadataTableView TypeRefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.TypeRef);
  }

  public MetadataTableView TypeNestings
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.NestedClass);
  }

  public MetadataTableView TypeExports
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.ExportedType);
  }

  public MetadataTableView MethodDefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.MethodDef);
  }

  public MetadataTableView MethodSpecs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.MethodSpec);
  }

  public MetadataTableView FieldDefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.FieldDef);
  }

  public MetadataTableView MemberRefs
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.MemberRef);
  }

  public MetadataTableView Signatures
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.Signature);
  }

  public MetadataTableView CustomAttributes
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.CustomAttribute);
  }

  public MetadataTableView InterfacesImpls
  {
    get => new MetadataTableView(MetadataRecord.Null(this), MetadataTokenType.InterfaceImpl);
  }

  public MetadataRecord GetRecord(MetadataToken token) => new MetadataRecord(token, this);
}
