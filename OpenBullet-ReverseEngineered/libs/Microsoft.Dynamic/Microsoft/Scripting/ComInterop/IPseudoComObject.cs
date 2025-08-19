// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.ComInterop.IPseudoComObject
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.ComInterop;

internal interface IPseudoComObject
{
  DynamicMetaObject GetMetaObject(Expression expression);
}
