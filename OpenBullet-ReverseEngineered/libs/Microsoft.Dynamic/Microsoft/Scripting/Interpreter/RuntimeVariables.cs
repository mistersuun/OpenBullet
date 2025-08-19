// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.RuntimeVariables
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class RuntimeVariables : IRuntimeVariables
{
  private readonly IStrongBox[] _boxes;

  private RuntimeVariables(IStrongBox[] boxes) => this._boxes = boxes;

  int IRuntimeVariables.Count => this._boxes.Length;

  object IRuntimeVariables.this[int index]
  {
    get => this._boxes[index].Value;
    set => this._boxes[index].Value = value;
  }

  internal static IRuntimeVariables Create(IStrongBox[] boxes)
  {
    return (IRuntimeVariables) new RuntimeVariables(boxes);
  }
}
