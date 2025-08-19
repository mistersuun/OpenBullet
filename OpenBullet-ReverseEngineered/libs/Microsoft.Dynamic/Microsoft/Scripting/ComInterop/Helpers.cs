// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.Helpers
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal static class Helpers
{
  internal static Expression Convert(Expression expression, Type type)
  {
    return expression.Type == type ? expression : (Expression) Expression.Convert(expression, type);
  }
}
