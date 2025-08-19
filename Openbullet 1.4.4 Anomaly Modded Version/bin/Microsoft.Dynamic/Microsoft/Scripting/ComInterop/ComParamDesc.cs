// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComParamDesc
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public class ComParamDesc
{
  private readonly bool _isOut;
  private readonly bool _isOpt;
  private readonly bool _byRef;
  private readonly bool _isArray;
  private readonly VarEnum _vt;
  private readonly string _name;
  private readonly Type _type;
  private readonly object _defaultValue;

  internal ComParamDesc(ref System.Runtime.InteropServices.ComTypes.ELEMDESC elemDesc, string name)
  {
    this._defaultValue = (object) DBNull.Value;
    if (!string.IsNullOrEmpty(name))
    {
      this._isOut = (elemDesc.desc.paramdesc.wParamFlags & System.Runtime.InteropServices.ComTypes.PARAMFLAG.PARAMFLAG_FOUT) != 0;
      this._isOpt = (elemDesc.desc.paramdesc.wParamFlags & System.Runtime.InteropServices.ComTypes.PARAMFLAG.PARAMFLAG_FOPT) != 0;
    }
    this._name = name;
    this._vt = (VarEnum) elemDesc.tdesc.vt;
    System.Runtime.InteropServices.ComTypes.TYPEDESC typedesc = elemDesc.tdesc;
    while (true)
    {
      if (this._vt == VarEnum.VT_PTR)
        this._byRef = true;
      else if (this._vt == VarEnum.VT_ARRAY)
        this._isArray = true;
      else
        break;
      System.Runtime.InteropServices.ComTypes.TYPEDESC structure = (System.Runtime.InteropServices.ComTypes.TYPEDESC) Marshal.PtrToStructure(typedesc.lpValue, typeof (System.Runtime.InteropServices.ComTypes.TYPEDESC));
      this._vt = (VarEnum) structure.vt;
      typedesc = structure;
    }
    VarEnum vt = this._vt;
    if ((this._vt & VarEnum.VT_BYREF) != VarEnum.VT_EMPTY)
    {
      vt = this._vt & ~VarEnum.VT_BYREF;
      this._byRef = true;
    }
    this._type = ComParamDesc.GetTypeForVarEnum(vt);
  }

  internal ComParamDesc(ref System.Runtime.InteropServices.ComTypes.ELEMDESC elemDesc)
    : this(ref elemDesc, string.Empty)
  {
  }

  private static Type GetTypeForVarEnum(VarEnum vt)
  {
    Type typeForVarEnum;
    switch (vt)
    {
      case VarEnum.VT_EMPTY:
      case VarEnum.VT_NULL:
      case VarEnum.VT_RECORD:
        throw new InvalidOperationException($"Unexpected VarEnum {vt}.");
      case VarEnum.VT_VOID:
        typeForVarEnum = (Type) null;
        break;
      case VarEnum.VT_HRESULT:
        typeForVarEnum = typeof (int);
        break;
      case VarEnum.VT_PTR:
      case (VarEnum) 37:
        typeForVarEnum = typeof (IntPtr);
        break;
      case VarEnum.VT_SAFEARRAY:
      case VarEnum.VT_CARRAY:
        typeForVarEnum = typeof (Array);
        break;
      case VarEnum.VT_USERDEFINED:
        typeForVarEnum = typeof (object);
        break;
      case VarEnum.VT_LPSTR:
      case VarEnum.VT_LPWSTR:
        typeForVarEnum = typeof (string);
        break;
      case (VarEnum) 38:
        typeForVarEnum = typeof (UIntPtr);
        break;
      default:
        typeForVarEnum = VarEnumSelector.GetManagedMarshalType(vt);
        break;
    }
    return typeForVarEnum;
  }

  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (this._isOpt)
      stringBuilder.Append("[Optional] ");
    if (this._isOut)
      stringBuilder.Append("[out]");
    stringBuilder.Append(this._type.Name);
    if (this._isArray)
      stringBuilder.Append("[]");
    if (this._byRef)
      stringBuilder.Append("&");
    stringBuilder.Append(" ");
    stringBuilder.Append(this._name);
    if (this._defaultValue != DBNull.Value)
    {
      stringBuilder.Append("=");
      stringBuilder.Append(this._defaultValue);
    }
    return stringBuilder.ToString();
  }

  public bool IsOut => this._isOut;

  public bool IsOptional => this._isOpt;

  public bool ByReference => this._byRef;

  public bool IsArray => this._isArray;

  public Type ParameterType => this._type;

  internal object DefaultValue => this._defaultValue;
}
