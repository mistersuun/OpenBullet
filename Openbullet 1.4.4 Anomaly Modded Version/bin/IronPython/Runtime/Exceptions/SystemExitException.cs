// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Exceptions.SystemExitException
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Modules;
using IronPython.Runtime.Operations;
using IronPython.Runtime.Types;
using System;
using System.Numerics;
using System.Runtime.Serialization;

#nullable disable
namespace IronPython.Runtime.Exceptions;

[Serializable]
public sealed class SystemExitException : Exception
{
  public SystemExitException()
  {
  }

  public SystemExitException(string msg)
    : base(msg)
  {
  }

  public SystemExitException(string message, Exception innerException)
    : base(message, innerException)
  {
  }

  private SystemExitException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  [PythonHidden(new PlatformID[] {})]
  public int GetExitCode(out object otherCode)
  {
    otherCode = (object) null;
    object ret;
    if (!PythonOps.TryGetBoundAttr(PythonExceptions.ToPython((Exception) this), "args", out ret) || !(ret is PythonTuple pythonTuple) || pythonTuple.__len__() == 0)
      return 0;
    if (Builtin.isinstance(pythonTuple[0], TypeCache.Int32))
      return Converter.ConvertToInt32(pythonTuple[0]);
    if (Builtin.isinstance(pythonTuple[0], TypeCache.BigInteger))
    {
      BigInteger bigInteger = Converter.ConvertToBigInteger(pythonTuple[0]);
      return bigInteger > (long) int.MaxValue ? -1 : (int) bigInteger;
    }
    otherCode = pythonTuple[0];
    return 1;
  }
}
