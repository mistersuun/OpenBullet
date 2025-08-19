// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonNarrowing
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Actions.Calls;

#nullable disable
namespace IronPython.Runtime;

internal static class PythonNarrowing
{
  public const NarrowingLevel None = NarrowingLevel.None;
  public const NarrowingLevel BinaryOperator = NarrowingLevel.Two;
  public const NarrowingLevel IndexOperator = NarrowingLevel.Three;
  public const NarrowingLevel All = NarrowingLevel.All;
}
