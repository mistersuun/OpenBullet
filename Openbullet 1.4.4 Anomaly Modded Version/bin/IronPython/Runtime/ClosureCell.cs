// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.ClosureCell
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Runtime;
using System;

#nullable disable
namespace IronPython.Runtime;

[PythonType("cell")]
public sealed class ClosureCell : ICodeFormattable
{
  [PythonHidden(new PlatformID[] {})]
  public object Value;
  public const object __hash__ = null;

  internal ClosureCell(object value) => this.Value = value;

  public object cell_contents
  {
    get
    {
      return this.Value != Uninitialized.Instance ? this.Value : throw PythonOps.ValueError("cell is empty");
    }
  }

  public string __repr__(CodeContext context)
  {
    return $"<cell at {IdDispenser.GetId((object) this)}: {this.GetContentsRepr()}>";
  }

  private string GetContentsRepr()
  {
    return this.Value == Uninitialized.Instance ? "empty" : $"{PythonTypeOps.GetName(this.Value)} object at {IdDispenser.GetId(this.Value)}";
  }

  [Python3Warning("cell comparisons not supported in 3.x")]
  public int __cmp__(object other)
  {
    return other is ClosureCell closureCell ? PythonOps.Compare(this.Value, closureCell.Value) : throw PythonOps.TypeError("cell.__cmp__(x,y) expected cell, got {0}", (object) PythonTypeOps.GetName(other));
  }
}
