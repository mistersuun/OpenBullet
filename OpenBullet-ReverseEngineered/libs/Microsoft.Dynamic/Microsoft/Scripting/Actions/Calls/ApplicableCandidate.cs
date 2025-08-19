// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.Calls.ApplicableCandidate
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Actions.Calls;

public sealed class ApplicableCandidate
{
  public readonly MethodCandidate Method;
  public readonly ArgumentBinding ArgumentBinding;

  internal ApplicableCandidate(MethodCandidate method, ArgumentBinding argBinding)
  {
    this.Method = method;
    this.ArgumentBinding = argBinding;
  }

  public ParameterWrapper GetParameter(int argumentIndex)
  {
    return this.Method.GetParameter(argumentIndex, this.ArgumentBinding);
  }

  public override string ToString() => this.Method.ToString();
}
