// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.InterpretedFrameInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

[Serializable]
public struct InterpretedFrameInfo(string methodName, DebugInfo info)
{
  public readonly string MethodName = methodName;
  public readonly DebugInfo DebugInfo = info;

  public override string ToString()
  {
    return this.MethodName + (this.DebugInfo != null ? ": " + (object) this.DebugInfo : (string) null);
  }
}
