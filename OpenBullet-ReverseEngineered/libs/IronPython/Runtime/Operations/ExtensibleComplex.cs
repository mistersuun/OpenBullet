// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.ExtensibleComplex
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using System.Numerics;

#nullable disable
namespace IronPython.Runtime.Operations;

public class ExtensibleComplex : Extensible<Complex>
{
  public ExtensibleComplex()
  {
  }

  public ExtensibleComplex(double real)
    : base(MathUtils.MakeReal(real))
  {
  }

  public ExtensibleComplex(double real, double imag)
    : base(new Complex(real, imag))
  {
  }
}
