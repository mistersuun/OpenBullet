// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Metadata.MetadataToken
// Assembly: Microsoft.Scripting.Metadata, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: BE741C3D-3A90-4CCD-BF31-5CBF515F5166
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.Metadata.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Security;

#nullable disable
namespace Microsoft.Scripting.Metadata;

[DebuggerDisplay("{DebugView}")]
[Serializable]
public struct MetadataToken : IEquatable<MetadataToken>
{
  internal readonly int m_value;

  public MetadataToken(int value) => this.m_value = value;

  internal MetadataToken(MetadataTokenType type, int rowId)
  {
    this.m_value = (int) (type | (MetadataTokenType) (rowId & 16777215 /*0xFFFFFF*/));
  }

  internal MetadataToken(MetadataTokenType type, uint rowId)
  {
    this.m_value = (int) (type | (MetadataTokenType) ((int) rowId & 16777215 /*0xFFFFFF*/));
  }

  public MetadataToken(MetadataRecordType type, int rowId)
  {
    this.m_value = (int) type << 24 | rowId;
  }

  [SecuritySafeCritical]
  public override bool Equals(object obj) => obj is MetadataToken other && this.Equals(other);

  [SecuritySafeCritical]
  public bool Equals(MetadataToken other) => this.m_value == other.m_value;

  public static bool operator ==(MetadataToken self, MetadataToken other) => self.Equals(other);

  public static bool operator !=(MetadataToken self, MetadataToken other) => self.Equals(other);

  [SecuritySafeCritical]
  public override int GetHashCode() => this.m_value;

  public bool IsNull => this.Rid == 0;

  public int Rid => this.m_value & 16777215 /*0xFFFFFF*/;

  public int Value => this.m_value;

  internal MetadataTokenType TokenType
  {
    get => (MetadataTokenType) ((long) this.m_value & 4278190080L /*0xFF000000*/);
  }

  public MetadataRecordType RecordType => (MetadataRecordType) (this.m_value >> 24);

  internal string DebugView
  {
    get
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:x8}", (object) this.m_value);
    }
  }

  public bool IsTypeRef => this.TokenType == MetadataTokenType.TypeRef;

  public bool IsTypeDef => this.TokenType == MetadataTokenType.TypeDef;

  public bool IsFieldDef => this.TokenType == MetadataTokenType.FieldDef;

  public bool IsMethodDef => this.TokenType == MetadataTokenType.MethodDef;

  public bool IsMemberRef => this.TokenType == MetadataTokenType.MemberRef;

  public bool IsEvent => this.TokenType == MetadataTokenType.Event;

  public bool IsProperty => this.TokenType == MetadataTokenType.Property;

  public bool IsParamDef => this.TokenType == MetadataTokenType.ParamDef;

  public bool IsTypeSpec => this.TokenType == MetadataTokenType.TypeSpec;

  public bool IsMethodSpec => this.TokenType == MetadataTokenType.MethodSpec;

  public bool IsString => this.TokenType == MetadataTokenType.String;

  public bool IsSignature => this.TokenType == MetadataTokenType.Signature;

  public bool IsCustomAttribute => this.TokenType == MetadataTokenType.CustomAttribute;

  public bool IsGenericParam => this.TokenType == MetadataTokenType.GenericPar;

  public bool IsGenericParamContraint => this.TokenType == MetadataTokenType.GenericParamConstraint;
}
