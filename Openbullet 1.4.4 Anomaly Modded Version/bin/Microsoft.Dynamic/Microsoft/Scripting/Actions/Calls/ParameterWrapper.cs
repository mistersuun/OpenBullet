// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ParameterWrapper
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class ParameterWrapper
{
  private readonly ParameterBindingFlags _flags;

  [Obsolete("Use ParameterBindingAttributes overload")]
  public ParameterWrapper(Type type, string name, bool prohibitNull)
    : this((ParameterInfo) null, type, name, prohibitNull, false, false, false)
  {
  }

  [Obsolete("Use ParameterBindingAttributes overload")]
  public ParameterWrapper(
    ParameterInfo info,
    Type type,
    string name,
    bool prohibitNull,
    bool isParams,
    bool isParamsDict,
    bool isHidden)
    : this(info, type, name, (ParameterBindingFlags) ((prohibitNull ? 1 : 0) | (isParams ? 4 : 0) | (isParamsDict ? 8 : 0) | (isHidden ? 16 /*0x10*/ : 0)))
  {
  }

  public ParameterWrapper(ParameterInfo info, Type type, string name, ParameterBindingFlags flags)
  {
    ContractUtils.RequiresNotNull((object) type, nameof (type));
    this.Type = type;
    this.ParameterInfo = info;
    this._flags = flags;
    this.Name = this.IsParamsArray || this.IsParamsDict || name == null ? "<unknown>" : name;
  }

  public ParameterWrapper Clone(string name)
  {
    return new ParameterWrapper(this.ParameterInfo, this.Type, name, this._flags);
  }

  public Type Type { get; }

  public ParameterInfo ParameterInfo { get; }

  public string Name { get; }

  public ParameterBindingFlags Flags => this._flags;

  public bool ProhibitNull => (this._flags & ParameterBindingFlags.ProhibitNull) != 0;

  public bool ProhibitNullItems => (this._flags & ParameterBindingFlags.ProhibitNullItems) != 0;

  public bool IsHidden => (this._flags & ParameterBindingFlags.IsHidden) != 0;

  public bool IsByRef => this.ParameterInfo != null && this.ParameterInfo.ParameterType.IsByRef;

  public bool IsParamsArray => (this._flags & ParameterBindingFlags.IsParamArray) != 0;

  public bool IsParamsDict => (this._flags & ParameterBindingFlags.IsParamDictionary) != 0;

  internal static int IndexOfParamsArray(IList<ParameterWrapper> parameters)
  {
    for (int index = 0; index < parameters.Count; ++index)
    {
      if (parameters[index].IsParamsArray)
        return index;
    }
    return -1;
  }

  internal ParameterWrapper Expand()
  {
    return new ParameterWrapper(this.ParameterInfo, this.Type.GetElementType(), (string) null, (ParameterBindingFlags) ((this.ProhibitNullItems ? 1 : 0) | (this.IsHidden ? 16 /*0x10*/ : 0)));
  }
}
