// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComTypeEnumDesc
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public sealed class ComTypeEnumDesc : ComTypeDesc, IDynamicMetaObjectProvider
{
  private readonly string[] _memberNames;
  private readonly object[] _memberValues;

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<enum '{0}'>", (object) this.TypeName);
  }

  internal ComTypeEnumDesc(ITypeInfo typeInfo, ComTypeLibDesc typeLibDesc)
    : base(typeInfo, ComType.Enum, typeLibDesc)
  {
    System.Runtime.InteropServices.ComTypes.TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(typeInfo);
    string[] strArray = new string[(int) typeAttrForTypeInfo.cVars];
    object[] objArray = new object[(int) typeAttrForTypeInfo.cVars];
    IntPtr ppVarDesc = IntPtr.Zero;
    for (int index = 0; index < (int) typeAttrForTypeInfo.cVars; ++index)
    {
      typeInfo.GetVarDesc(index, out ppVarDesc);
      System.Runtime.InteropServices.ComTypes.VARDESC structure;
      try
      {
        structure = (System.Runtime.InteropServices.ComTypes.VARDESC) Marshal.PtrToStructure(ppVarDesc, typeof (System.Runtime.InteropServices.ComTypes.VARDESC));
        if (structure.varkind == VARKIND.VAR_CONST)
          objArray[index] = Marshal.GetObjectForNativeVariant(structure.desc.lpvarValue);
      }
      finally
      {
        typeInfo.ReleaseVarDesc(ppVarDesc);
      }
      strArray[index] = ComRuntimeHelpers.GetNameOfMethod(typeInfo, structure.memid);
    }
    this._memberNames = strArray;
    this._memberValues = objArray;
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new TypeEnumMetaObject(this, parameter);
  }

  public object GetValue(string enumValueName)
  {
    for (int index = 0; index < this._memberNames.Length; ++index)
    {
      if (this._memberNames[index] == enumValueName)
        return this._memberValues[index];
    }
    throw new MissingMemberException(enumValueName);
  }

  internal bool HasMember(string name)
  {
    for (int index = 0; index < this._memberNames.Length; ++index)
    {
      if (this._memberNames[index] == name)
        return true;
    }
    return false;
  }

  public string[] GetMemberNames() => (string[]) this._memberNames.Clone();
}
