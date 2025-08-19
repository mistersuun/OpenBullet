// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ArgumentBinding
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public struct ArgumentBinding
{
  private static readonly int[] _EmptyBinding = new int[0];
  private readonly int _positionalArgCount;
  private readonly int[] _binding;

  internal ArgumentBinding(int positionalArgCount)
  {
    this._positionalArgCount = positionalArgCount;
    this._binding = ArgumentBinding._EmptyBinding;
  }

  internal ArgumentBinding(int positionalArgCount, int[] binding)
  {
    this._binding = binding;
    this._positionalArgCount = positionalArgCount;
  }

  public int PositionalArgCount => this._positionalArgCount;

  public int ArgumentToParameter(int argumentIndex)
  {
    int index = argumentIndex - this._positionalArgCount;
    return index >= 0 ? this._positionalArgCount + this._binding[index] : argumentIndex;
  }
}
