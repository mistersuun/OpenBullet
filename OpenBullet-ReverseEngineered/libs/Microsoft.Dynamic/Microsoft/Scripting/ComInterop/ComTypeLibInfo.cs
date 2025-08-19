// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ComTypeLibInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

public sealed class ComTypeLibInfo : IDynamicMetaObjectProvider
{
  private readonly ComTypeLibDesc _typeLibDesc;

  internal ComTypeLibInfo(ComTypeLibDesc typeLibDesc) => this._typeLibDesc = typeLibDesc;

  public string Name => this._typeLibDesc.Name;

  public Guid Guid => this._typeLibDesc.Guid;

  public short VersionMajor => this._typeLibDesc.VersionMajor;

  public short VersionMinor => this._typeLibDesc.VersionMinor;

  public ComTypeLibDesc TypeLibDesc => this._typeLibDesc;

  public string[] GetMemberNames()
  {
    return new string[5]
    {
      this.Name,
      "Guid",
      "Name",
      "VersionMajor",
      "VersionMinor"
    };
  }

  DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
  {
    return (DynamicMetaObject) new TypeLibInfoMetaObject(parameter, this);
  }
}
