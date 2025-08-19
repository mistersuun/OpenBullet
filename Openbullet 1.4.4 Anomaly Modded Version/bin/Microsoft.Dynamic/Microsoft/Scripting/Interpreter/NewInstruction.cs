// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Interpreter.NewInstruction
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Runtime;
using System.Reflection;

#nullable disable
namespace Microsoft.Scripting.Interpreter;

internal sealed class NewInstruction : Instruction
{
  private readonly ConstructorInfo _constructor;
  private readonly int _argCount;

  public NewInstruction(ConstructorInfo constructor)
  {
    this._constructor = constructor;
    this._argCount = constructor.GetParameters().Length;
  }

  public override int ConsumedStack => this._argCount;

  public override int ProducedStack => 1;

  public override int Run(InterpretedFrame frame)
  {
    object[] parameters = new object[this._argCount];
    for (int index = this._argCount - 1; index >= 0; --index)
      parameters[index] = frame.Pop();
    object obj;
    try
    {
      obj = this._constructor.Invoke(parameters);
    }
    catch (TargetInvocationException ex)
    {
      ExceptionHelpers.UpdateForRethrow(ex.InnerException);
      throw ex.InnerException;
    }
    frame.Push(obj);
    return 1;
  }

  public override string ToString()
  {
    return $"New {this._constructor.DeclaringType.Name}({(object) this._constructor})";
  }
}
