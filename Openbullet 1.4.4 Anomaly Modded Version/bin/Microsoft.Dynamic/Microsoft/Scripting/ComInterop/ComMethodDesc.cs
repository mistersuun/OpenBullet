﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComMethodDesc
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Runtime.InteropServices.ComTypes;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public class ComMethodDesc
{
  private readonly int _memid;
  private readonly string _name;
  internal readonly INVOKEKIND InvokeKind;
  private readonly int _paramCnt;

  private ComMethodDesc(int dispId) => this._memid = dispId;

  internal ComMethodDesc(string name, int dispId)
    : this(dispId)
  {
    this._name = name;
  }

  internal ComMethodDesc(string name, int dispId, INVOKEKIND invkind)
    : this(name, dispId)
  {
    this.InvokeKind = invkind;
  }

  internal ComMethodDesc(ITypeInfo typeInfo, FUNCDESC funcDesc)
    : this(funcDesc.memid)
  {
    this.InvokeKind = funcDesc.invkind;
    string[] rgBstrNames = new string[1 + (int) funcDesc.cParams];
    int pcNames;
    typeInfo.GetNames(this._memid, rgBstrNames, rgBstrNames.Length, out pcNames);
    if (this.IsPropertyPut && rgBstrNames[rgBstrNames.Length - 1] == null)
    {
      rgBstrNames[rgBstrNames.Length - 1] = "value";
      int num = pcNames + 1;
    }
    this._name = rgBstrNames[0];
    this._paramCnt = (int) funcDesc.cParams;
  }

  public string Name => this._name;

  public int DispId => this._memid;

  public bool IsPropertyGet => (this.InvokeKind & INVOKEKIND.INVOKE_PROPERTYGET) != 0;

  public bool IsDataMember => this.IsPropertyGet && this.DispId != -4 && this._paramCnt == 0;

  public bool IsPropertyPut
  {
    get
    {
      return (this.InvokeKind & (INVOKEKIND.INVOKE_PROPERTYPUT | INVOKEKIND.INVOKE_PROPERTYPUTREF)) != 0;
    }
  }

  public bool IsPropertyPutRef => (this.InvokeKind & INVOKEKIND.INVOKE_PROPERTYPUTREF) != 0;

  internal int ParamCount => this._paramCnt;
}
