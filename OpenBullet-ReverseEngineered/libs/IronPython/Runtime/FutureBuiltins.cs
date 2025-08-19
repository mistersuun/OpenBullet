// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.FutureBuiltins
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Operations;
using Microsoft.Scripting.Utils;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime;

public static class FutureBuiltins
{
  public const string __doc__ = "Provides access to built-ins which will be defined differently in Python 3.0.";

  [SpecialName]
  public static void PerformModuleReload(PythonContext context, PythonDictionary dict)
  {
    if (!(Importer.ImportModule(context.SharedContext, (object) context.SharedContext.GlobalDict, "itertools", false, -1) is PythonModule pythonModule))
      return;
    dict[(object) "map"] = pythonModule.__dict__[(object) "imap"];
    dict[(object) "filter"] = pythonModule.__dict__[(object) "ifilter"];
    dict[(object) "zip"] = pythonModule.__dict__[(object) "izip"];
  }

  public static string ascii(CodeContext context, object @object)
  {
    return PythonOps.Repr(context, @object);
  }

  public static string hex(CodeContext context, object number)
  {
    if (number is int x)
      return Int32Ops.__hex__(x);
    if (number is BigInteger self)
      return self < 0L ? "-0x" + (-self).ToString(16 /*0x10*/).ToLower() : "0x" + self.ToString(16 /*0x10*/).ToLower();
    object obj;
    if (!PythonTypeOps.TryInvokeUnaryOperator(context, number, "__index__", out obj))
      throw PythonOps.TypeError("hex() argument cannot be interpreted as an index");
    switch (obj)
    {
      case int _:
      case BigInteger _:
        return FutureBuiltins.hex(context, obj);
      default:
        throw PythonOps.TypeError("index returned non-(int, long), got '{0}'", (object) PythonTypeOps.GetName(obj));
    }
  }

  public static string oct(CodeContext context, object number)
  {
    if (number is int)
      number = (object) (BigInteger) (int) number;
    if (number is BigInteger self)
    {
      if (self == 0L)
        return "0o0";
      return self > 0L ? "0o" + self.ToString(8) : "-0o" + (-self).ToString(8);
    }
    object obj;
    if (!PythonTypeOps.TryInvokeUnaryOperator(context, number, "__index__", out obj))
      throw PythonOps.TypeError("oct() argument cannot be interpreted as an index");
    switch (obj)
    {
      case int _:
      case BigInteger _:
        return FutureBuiltins.oct(context, obj);
      default:
        throw PythonOps.TypeError("index returned non-(int, long), got '{0}'", (object) PythonTypeOps.GetName(obj));
    }
  }
}
