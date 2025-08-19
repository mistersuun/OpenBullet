// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.NotImplementedType
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.Threading;

#nullable disable
namespace IronPython.Runtime.Types;

[PythonType("NotImplementedType")]
[Documentation(null)]
public class NotImplementedType : ICodeFormattable
{
  private static NotImplementedType _instance;

  private NotImplementedType()
  {
  }

  internal static NotImplementedType Value
  {
    get
    {
      if (NotImplementedType._instance == null)
        Interlocked.CompareExchange<NotImplementedType>(ref NotImplementedType._instance, new NotImplementedType(), (NotImplementedType) null);
      return NotImplementedType._instance;
    }
  }

  public string __repr__(CodeContext context) => "NotImplemented";

  public int __hash__() => 505028248;
}
