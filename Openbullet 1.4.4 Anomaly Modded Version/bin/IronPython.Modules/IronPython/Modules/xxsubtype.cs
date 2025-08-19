// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.xxsubtype
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using Microsoft.Scripting;
using System.Collections.Generic;

#nullable disable
namespace IronPython.Modules;

public static class xxsubtype
{
  public const string __doc__ = "Provides samples on how to subtype built-in types from .NET.";

  public static double bench(CodeContext context, object x, string name)
  {
    double num = PythonTime.clock();
    for (int index = 0; index < 1001; ++index)
      PythonOps.GetBoundAttr(context, x, name);
    return PythonTime.clock() - num;
  }

  [PythonType]
  public class spamlist : IronPython.Runtime.List
  {
    private int _state;

    public spamlist()
    {
    }

    public spamlist(object sequence)
      : base(sequence)
    {
    }

    public int state
    {
      get => this._state;
      set => this._state = value;
    }

    public int getstate() => this.state;

    public void setstate(int value) => this.state = value;

    public static object staticmeth([ParamDictionary] IDictionary<object, object> dict, params object[] args)
    {
      return (object) PythonTuple.MakeTuple(null, (object) PythonTuple.MakeTuple(args), (object) dict);
    }

    [ClassMethod]
    public static object classmeth(
      PythonType cls,
      [ParamDictionary] IDictionary<object, object> dict,
      params object[] args)
    {
      return (object) PythonTuple.MakeTuple((object) cls, (object) PythonTuple.MakeTuple(args), (object) dict);
    }
  }

  [PythonType]
  public class spamdict : PythonDictionary
  {
    private int _state;

    public int state
    {
      get => this._state;
      set => this._state = value;
    }

    public int getstate() => this.state;

    public void setstate(int value) => this.state = value;
  }
}
