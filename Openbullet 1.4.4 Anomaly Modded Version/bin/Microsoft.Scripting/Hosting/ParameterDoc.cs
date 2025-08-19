// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.ParameterDoc
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[Serializable]
public class ParameterDoc
{
  private readonly string _name;
  private readonly string _typeName;
  private readonly string _doc;
  private readonly ParameterFlags _flags;

  public ParameterDoc(string name)
    : this(name, (string) null, (string) null, ParameterFlags.None)
  {
  }

  public ParameterDoc(string name, ParameterFlags paramFlags)
    : this(name, (string) null, (string) null, paramFlags)
  {
  }

  public ParameterDoc(string name, string typeName)
    : this(name, typeName, (string) null, ParameterFlags.None)
  {
  }

  public ParameterDoc(string name, string typeName, string documentation)
    : this(name, typeName, documentation, ParameterFlags.None)
  {
  }

  public ParameterDoc(
    string name,
    string typeName,
    string documentation,
    ParameterFlags paramFlags)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    this._name = name;
    this._flags = paramFlags;
    this._typeName = typeName;
    this._doc = documentation;
  }

  public string Name => this._name;

  public string TypeName => this._typeName;

  public ParameterFlags Flags => this._flags;

  public string Documentation => this._doc;
}
