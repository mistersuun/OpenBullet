// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Generation.DynamicILGen
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Reflection;
using System.Reflection.Emit;

#nullable disable
namespace Microsoft.Scripting.Generation;

public abstract class DynamicILGen : ILGen
{
  internal DynamicILGen(ILGenerator il)
    : base(il)
  {
  }

  public T CreateDelegate<T>() => this.CreateDelegate<T>(out MethodInfo _);

  public abstract T CreateDelegate<T>(out MethodInfo mi);

  public abstract MethodInfo Finish();
}
