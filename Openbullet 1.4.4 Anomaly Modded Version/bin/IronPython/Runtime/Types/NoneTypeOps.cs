// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.NoneTypeOps
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;

#nullable disable
namespace IronPython.Runtime.Types;

public class NoneTypeOps
{
  internal const int NoneHashCode = 505032256;
  public static readonly string __doc__;

  public static int __hash__(DynamicNull self) => 505032256;

  public static string __repr__(DynamicNull self) => "None";
}
