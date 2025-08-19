// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.IDebugThreadFactory
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Debugging.CompilerServices;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal interface IDebugThreadFactory
{
  DebugThread CreateDebugThread(DebugContext debugContext);

  Expression CreatePushFrameExpression(
    ParameterExpression functionInfo,
    ParameterExpression debugMarker,
    IList<ParameterExpression> locals,
    IList<VariableInfo> varInfos,
    Expression runtimeThread);
}
