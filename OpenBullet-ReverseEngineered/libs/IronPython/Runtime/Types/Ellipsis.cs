// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.Ellipsis
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("ellipsis")]
[Documentation(null)]
public class Ellipsis : ICodeFormattable
{
  private static Ellipsis _instance;

  private Ellipsis()
  {
  }

  internal static Ellipsis Value
  {
    get
    {
      if (Ellipsis._instance == null)
        Interlocked.CompareExchange<Ellipsis>(ref Ellipsis._instance, new Ellipsis(), (Ellipsis) null);
      return Ellipsis._instance;
    }
  }

  public string __repr__(CodeContext context) => nameof (Ellipsis);

  public int __hash__() => 505045512;
}
