// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComTypeLibDesc
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public sealed class ComTypeLibDesc : IDynamicMetaObjectProvider
{
  private LinkedList<ComTypeClassDesc> _classes;
  private Dictionary<string, ComTypeEnumDesc> _enums;
  private string _typeLibName;
  private System.Runtime.InteropServices.ComTypes.TYPELIBATTR _typeLibAttributes;
  private static Dictionary<Guid, ComTypeLibDesc> _CachedTypeLibDesc = new Dictionary<Guid, ComTypeLibDesc>();

  private ComTypeLibDesc()
  {
    this._enums = new Dictionary<string, ComTypeEnumDesc>();
    this._classes = new LinkedList<ComTypeClassDesc>();
  }

  public override string ToString()
  {
    return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<type library {0}>", (object) this._typeLibName);
  }

  public string Documentation => string.Empty;

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new TypeLibMetaObject(parameter, this);
  }

  public static ComTypeLibInfo CreateFromGuid(Guid typeLibGuid)
  {
    return new ComTypeLibInfo(ComTypeLibDesc.GetFromTypeLib(UnsafeMethods.LoadRegTypeLib(ref typeLibGuid, (short) -1, (short) -1, 0)));
  }

  public static ComTypeLibInfo CreateFromObject(object rcw)
  {
    if (!Marshal.IsComObject(rcw))
      throw new ArgumentException("COM object is expected.");
    ITypeLib ppTLB;
    ComRuntimeHelpers.GetITypeInfoFromIDispatch(rcw as IDispatch, true).GetContainingTypeLib(out ppTLB, out int _);
    return new ComTypeLibInfo(ComTypeLibDesc.GetFromTypeLib(ppTLB));
  }

  internal static ComTypeLibDesc GetFromTypeLib(ITypeLib typeLib)
  {
    System.Runtime.InteropServices.ComTypes.TYPELIBATTR typeAttrForTypeLib = ComRuntimeHelpers.GetTypeAttrForTypeLib(typeLib);
    lock (ComTypeLibDesc._CachedTypeLibDesc)
    {
      ComTypeLibDesc fromTypeLib;
      if (ComTypeLibDesc._CachedTypeLibDesc.TryGetValue(typeAttrForTypeLib.guid, out fromTypeLib))
        return fromTypeLib;
    }
    ComTypeLibDesc typeLibDesc = new ComTypeLibDesc();
    typeLibDesc._typeLibName = ComRuntimeHelpers.GetNameOfLib(typeLib);
    typeLibDesc._typeLibAttributes = typeAttrForTypeLib;
    int typeInfoCount = typeLib.GetTypeInfoCount();
    for (int index = 0; index < typeInfoCount; ++index)
    {
      System.Runtime.InteropServices.ComTypes.TYPEKIND pTKind;
      typeLib.GetTypeInfoType(index, out pTKind);
      ITypeInfo ppTI1;
      typeLib.GetTypeInfo(index, out ppTI1);
      switch (pTKind)
      {
        case System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_ENUM:
          ComTypeEnumDesc comTypeEnumDesc1 = new ComTypeEnumDesc(ppTI1, typeLibDesc);
          typeLibDesc._enums.Add(comTypeEnumDesc1.TypeName, comTypeEnumDesc1);
          break;
        case System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_COCLASS:
          ComTypeClassDesc comTypeClassDesc = new ComTypeClassDesc(ppTI1, typeLibDesc);
          typeLibDesc._classes.AddLast(comTypeClassDesc);
          break;
        case System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_ALIAS:
          System.Runtime.InteropServices.ComTypes.TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(ppTI1);
          if (typeAttrForTypeInfo.tdescAlias.vt == (short) 29)
          {
            string name;
            ComRuntimeHelpers.GetInfoFromType(ppTI1, out name, out string _);
            ITypeInfo ppTI2;
            ppTI1.GetRefTypeInfo(typeAttrForTypeInfo.tdescAlias.lpValue.ToInt32(), out ppTI2);
            if (ComRuntimeHelpers.GetTypeAttrForTypeInfo(ppTI2).typekind == System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_ENUM)
            {
              ComTypeEnumDesc comTypeEnumDesc2 = new ComTypeEnumDesc(ppTI2, typeLibDesc);
              typeLibDesc._enums.Add(name, comTypeEnumDesc2);
              break;
            }
            break;
          }
          break;
      }
    }
    lock (ComTypeLibDesc._CachedTypeLibDesc)
      ComTypeLibDesc._CachedTypeLibDesc.Add(typeAttrForTypeLib.guid, typeLibDesc);
    return typeLibDesc;
  }

  public object GetTypeLibObjectDesc(string member)
  {
    foreach (ComTypeClassDesc typeLibObjectDesc in this._classes)
    {
      if (member == typeLibObjectDesc.TypeName)
        return (object) typeLibObjectDesc;
    }
    ComTypeEnumDesc comTypeEnumDesc;
    return this._enums != null && this._enums.TryGetValue(member, out comTypeEnumDesc) ? (object) comTypeEnumDesc : (object) null;
  }

  public string[] GetMemberNames()
  {
    string[] memberNames = new string[this._enums.Count + this._classes.Count];
    int num = 0;
    foreach (ComTypeClassDesc comTypeClassDesc in this._classes)
      memberNames[num++] = comTypeClassDesc.TypeName;
    foreach (KeyValuePair<string, ComTypeEnumDesc> keyValuePair in this._enums)
      memberNames[num++] = keyValuePair.Key;
    return memberNames;
  }

  internal bool HasMember(string member)
  {
    foreach (ComTypeClassDesc comTypeClassDesc in this._classes)
    {
      if (member == comTypeClassDesc.TypeName)
        return true;
    }
    return this._enums.ContainsKey(member);
  }

  public Guid Guid => this._typeLibAttributes.guid;

  public short VersionMajor => this._typeLibAttributes.wMajorVerNum;

  public short VersionMinor => this._typeLibAttributes.wMinorVerNum;

  public string Name => this._typeLibName;

  internal ComTypeClassDesc GetCoClassForInterface(string itfName)
  {
    foreach (ComTypeClassDesc classForInterface in this._classes)
    {
      if (classForInterface.Implements(itfName, false))
        return classForInterface;
    }
    return (ComTypeClassDesc) null;
  }
}
