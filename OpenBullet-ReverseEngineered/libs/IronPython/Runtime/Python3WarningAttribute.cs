// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Python3WarningAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;
using System;

#nullable disable
namespace IronPython.Runtime;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
internal sealed class Python3WarningAttribute : Attribute
{
  private readonly string _message;

  public Python3WarningAttribute(string message)
  {
    ContractUtils.RequiresNotNull((object) message, nameof (message));
    this._message = message;
  }

  public string Message => this._message;
}
