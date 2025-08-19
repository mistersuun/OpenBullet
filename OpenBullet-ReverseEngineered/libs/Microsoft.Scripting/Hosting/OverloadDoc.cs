// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Hosting.OverloadDoc
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Scripting.Hosting;

[Serializable]
public class OverloadDoc
{
  private readonly string _name;
  private readonly string _doc;
  private readonly ICollection<ParameterDoc> _params;
  private readonly ParameterDoc _returnParam;

  public OverloadDoc(string name, string documentation, ICollection<ParameterDoc> parameters)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNullItems<ParameterDoc>((IEnumerable<ParameterDoc>) parameters, nameof (parameters));
    this._name = name;
    this._params = parameters;
    this._doc = documentation;
  }

  public OverloadDoc(
    string name,
    string documentation,
    ICollection<ParameterDoc> parameters,
    ParameterDoc returnParameter)
  {
    ContractUtils.RequiresNotNull((object) name, nameof (name));
    ContractUtils.RequiresNotNullItems<ParameterDoc>((IEnumerable<ParameterDoc>) parameters, nameof (parameters));
    this._name = name;
    this._params = parameters;
    this._doc = documentation;
    this._returnParam = returnParameter;
  }

  public string Name => this._name;

  public string Documentation => this._doc;

  public ICollection<ParameterDoc> Parameters => this._params;

  public ParameterDoc ReturnParameter => this._returnParam;
}
