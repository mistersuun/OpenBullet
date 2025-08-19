// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.ArgBuilder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal abstract class ArgBuilder
{
  internal abstract Expression Marshal(Expression parameter);

  internal virtual Expression MarshalToRef(Expression parameter) => this.Marshal(parameter);

  internal virtual Expression UnmarshalFromRef(Expression newValue) => newValue;
}
