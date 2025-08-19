// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ExtensionBinaryOperationBinder
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System.Dynamic;
using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

public abstract class ExtensionBinaryOperationBinder : BinaryOperationBinder
{
  private readonly string _operation;

  protected ExtensionBinaryOperationBinder(string operation)
    : base(ExpressionType.Extension)
  {
    ContractUtils.RequiresNotNull((object) operation, nameof (operation));
    this._operation = operation;
  }

  public string ExtensionOperation => this._operation;

  public override int GetHashCode() => base.GetHashCode() ^ this._operation.GetHashCode();

  public override bool Equals(object obj)
  {
    return obj is ExtensionBinaryOperationBinder binaryOperationBinder && base.Equals(obj) && this._operation == binaryOperationBinder._operation;
  }
}
