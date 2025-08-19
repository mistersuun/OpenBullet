// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ObjectDebugView
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using System.Diagnostics;

#nullable disable
namespace IronPython.Runtime;

[DebuggerDisplay("{Value}", Name = "{GetName(),nq}", Type = "{GetClassName(),nq}")]
internal class ObjectDebugView
{
  [DebuggerBrowsable(DebuggerBrowsableState.Never)]
  private readonly string _name;
  [DebuggerBrowsable(DebuggerBrowsableState.Never)]
  private readonly object _value;

  public ObjectDebugView(object name, object value)
  {
    this._name = name.ToString();
    this._value = value;
  }

  [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
  public object Value => this._value;

  public string GetClassName() => PythonTypeOps.GetName(this._value);

  public string GetName() => this._name;
}
