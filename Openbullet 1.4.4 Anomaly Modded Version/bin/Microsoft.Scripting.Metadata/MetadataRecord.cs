// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataRecord
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Diagnostics;
using System.Security;

#nullable disable
namespace Microsoft.Scripting.Metadata;

[DebuggerDisplay("{DebugView}")]
public struct MetadataRecord : IEquatable<MetadataRecord>
{
  internal readonly MetadataToken m_token;
  internal readonly MetadataTables m_tables;

  internal int Rid => this.m_token.Rid;

  internal MetadataImport Import => this.m_tables.m_import;

  internal MetadataRecord(MetadataToken token, MetadataTables tables)
  {
    this.m_token = token;
    this.m_tables = tables;
  }

  public MetadataTables Tables => this.m_tables;

  public MetadataToken Token => this.m_token;

  [SecuritySafeCritical]
  public override bool Equals(object obj) => obj is MetadataRecord other && this.Equals(other);

  [SecuritySafeCritical]
  public bool Equals(MetadataRecord other)
  {
    return this.m_token.Equals(other.m_token) && this.m_tables == other.m_tables;
  }

  public static bool operator ==(MetadataRecord self, MetadataRecord other) => self.Equals(other);

  public static bool operator !=(MetadataRecord self, MetadataRecord other) => self.Equals(other);

  [SecuritySafeCritical]
  public override int GetHashCode() => this.m_token.GetHashCode() ^ this.m_tables.GetHashCode();

  public bool IsNull => this.m_token.IsNull;

  public bool IsValid => this.m_tables.IsValidToken(this.m_token);

  internal static MetadataRecord Null(MetadataTables tables)
  {
    return new MetadataRecord(new MetadataToken(0), tables);
  }

  internal string DebugView => this.m_token.DebugView;

  public MetadataRecordType Type => this.m_token.RecordType;

  public bool IsAssemblyDef => this.Type == MetadataRecordType.AssemblyDef;

  public bool IsAssemblyRef => this.Type == MetadataRecordType.AssemblyRef;

  public bool IsModuleDef => this.Type == MetadataRecordType.ModuleDef;

  public bool IsModuleRef => this.Type == MetadataRecordType.ModuleRef;

  public bool IsFileDef => this.Type == MetadataRecordType.FileDef;

  public bool IsManifestResourceDef => this.Type == MetadataRecordType.ManifestResourceDef;

  public bool IsTypeRef => this.Type == MetadataRecordType.TypeRef;

  public bool IsTypeDef => this.Type == MetadataRecordType.TypeDef;

  public bool IsTypeSpec => this.Type == MetadataRecordType.TypeSpec;

  public bool IsTypeExport => this.Type == MetadataRecordType.TypeExport;

  public bool IsTypeNesting => this.Type == MetadataRecordType.TypeNesting;

  public bool IsMemberRef => this.Type == MetadataRecordType.MemberRef;

  public bool IsFieldDef => this.Type == MetadataRecordType.FieldDef;

  public bool IsMethodDef => this.Type == MetadataRecordType.MethodDef;

  public bool IsMethodSpec => this.Type == MetadataRecordType.MethodSpec;

  public bool IsInterfaceImpl => this.Type == MetadataRecordType.InterfaceImpl;

  public bool IsEvent => this.Type == MetadataRecordType.EventDef;

  public bool IsProperty => this.Type == MetadataRecordType.PropertyDef;

  public bool IsParamDef => this.Type == MetadataRecordType.ParamDef;

  public bool IsGenericParamDef => this.Type == MetadataRecordType.GenericParamDef;

  public bool IsGenericParamConstraint => this.Type == MetadataRecordType.GenericParamConstraint;

  public bool IsSignatureDef => this.Type == MetadataRecordType.SignatureDef;

  public bool IsCustomAttributeDef => this.Type == MetadataRecordType.CustomAttributeDef;

  public AssemblyDef AssemblyDef => new AssemblyDef(this);

  public AssemblyRef AssemblyRef => new AssemblyRef(this);

  public FileDef FileDef => new FileDef(this);

  public ManifestResourceDef ResourceDef => new ManifestResourceDef(this);

  public ModuleDef ModuleDef => new ModuleDef(this);

  public ModuleRef ModuleRef => new ModuleRef(this);

  public TypeDef TypeDef => new TypeDef(this);

  public TypeRef TypeRef => new TypeRef(this);

  public TypeSpec TypeSpec => new TypeSpec(this);

  public TypeNesting TypeNesting => new TypeNesting(this);

  public TypeExport TypeExport => new TypeExport(this);

  public InterfaceImpl InterfaceImpl => new InterfaceImpl(this);

  public FieldDef FieldDef => new FieldDef(this);

  public MethodDef MethodDef => new MethodDef(this);

  public MethodSpec MethodSpec => new MethodSpec(this);

  public ParamDef ParamDef => new ParamDef(this);

  public MemberRef MemberRef => new MemberRef(this);

  public EventDef EventDef => new EventDef(this);

  public PropertyDef PropertyDef => new PropertyDef(this);

  public GenericParamDef GenericParamDef => new GenericParamDef(this);

  public GenericParamConstraint GenericParamConstraint => new GenericParamConstraint(this);

  public CustomAttributeDef CustomAttributeDef => new CustomAttributeDef(this);

  public SignatureDef SignatureDef => new SignatureDef(this);
}
