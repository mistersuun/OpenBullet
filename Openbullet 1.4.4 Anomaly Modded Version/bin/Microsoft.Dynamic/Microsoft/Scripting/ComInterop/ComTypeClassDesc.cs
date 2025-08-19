// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComTypeClassDesc
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public class ComTypeClassDesc : ComTypeDesc, IDynamicMetaObjectProvider
{
  private LinkedList<string> _itfs;
  private LinkedList<string> _sourceItfs;
  private Type _typeObj;

  public object CreateInstance()
  {
    if (this._typeObj == (Type) null)
      this._typeObj = Type.GetTypeFromCLSID(this.Guid);
    return Activator.CreateInstance(Type.GetTypeFromCLSID(this.Guid));
  }

  internal ComTypeClassDesc(ITypeInfo typeInfo, ComTypeLibDesc typeLibDesc)
    : base(typeInfo, ComType.Class, typeLibDesc)
  {
    TYPEATTR typeAttrForTypeInfo = ComRuntimeHelpers.GetTypeAttrForTypeInfo(typeInfo);
    this.Guid = typeAttrForTypeInfo.guid;
    for (int index = 0; index < (int) typeAttrForTypeInfo.cImplTypes; ++index)
    {
      int href;
      typeInfo.GetRefTypeOfImplType(index, out href);
      ITypeInfo ppTI;
      typeInfo.GetRefTypeInfo(href, out ppTI);
      IMPLTYPEFLAGS pImplTypeFlags;
      typeInfo.GetImplTypeFlags(index, out pImplTypeFlags);
      bool isSourceItf = (pImplTypeFlags & IMPLTYPEFLAGS.IMPLTYPEFLAG_FSOURCE) != 0;
      this.AddInterface(ppTI, isSourceItf);
    }
  }

  private void AddInterface(ITypeInfo itfTypeInfo, bool isSourceItf)
  {
    string nameOfType = ComRuntimeHelpers.GetNameOfType(itfTypeInfo);
    if (isSourceItf)
    {
      if (this._sourceItfs == null)
        this._sourceItfs = new LinkedList<string>();
      this._sourceItfs.AddLast(nameOfType);
    }
    else
    {
      if (this._itfs == null)
        this._itfs = new LinkedList<string>();
      this._itfs.AddLast(nameOfType);
    }
  }

  internal bool Implements(string itfName, bool isSourceItf)
  {
    return isSourceItf ? this._sourceItfs.Contains(itfName) : this._itfs.Contains(itfName);
  }

  public DynamicMetaObject GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new ComClassMetaObject(parameter, this);
  }
}
