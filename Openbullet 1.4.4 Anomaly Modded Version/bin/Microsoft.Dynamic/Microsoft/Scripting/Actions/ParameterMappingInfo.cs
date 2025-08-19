// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Actions.ParameterMappingInfo
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using System.Linq.Expressions;

#nullable disable
namespace Microsoft.Scripting.Actions;

public class ParameterMappingInfo
{
  private readonly int _parameterIndex;
  private readonly int _actionIndex;
  private ConstantExpression _fixedInput;

  private ParameterMappingInfo(int param, int action, ConstantExpression fixedInput)
  {
    this._parameterIndex = param;
    this._actionIndex = action;
    this._fixedInput = fixedInput;
  }

  public static ParameterMappingInfo Parameter(int index)
  {
    return new ParameterMappingInfo(index, -1, (ConstantExpression) null);
  }

  public static ParameterMappingInfo Action(int index)
  {
    return new ParameterMappingInfo(-1, index, (ConstantExpression) null);
  }

  public static ParameterMappingInfo Fixed(ConstantExpression e)
  {
    return new ParameterMappingInfo(-1, -1, e);
  }

  public int ParameterIndex => this._parameterIndex;

  public int ActionIndex => this._actionIndex;

  public ConstantExpression Constant => this._fixedInput;

  public bool IsParameter => this._parameterIndex != -1;

  public bool IsAction => this._actionIndex != -1;

  public bool IsConstant => this._fixedInput != null;

  public override bool Equals(object obj)
  {
    if (!(obj is ParameterMappingInfo parameterMappingInfo) || parameterMappingInfo.ParameterIndex != this.ParameterIndex || parameterMappingInfo.ActionIndex != this.ActionIndex)
      return false;
    if (this.Constant == null)
      return parameterMappingInfo.Constant == null;
    return parameterMappingInfo.Constant != null && this.Constant.Value == parameterMappingInfo.Constant.Value;
  }

  public override int GetHashCode()
  {
    int num = this.ParameterIndex;
    int hashCode1 = num.GetHashCode();
    num = this.ActionIndex;
    int hashCode2 = num.GetHashCode();
    int hashCode3 = hashCode1 ^ hashCode2;
    if (this.Constant?.Value != null)
      hashCode3 ^= this.Constant.Value.GetHashCode();
    return hashCode3;
  }

  public override string ToString()
  {
    if (this.IsAction)
      return "Action" + (object) this.ActionIndex;
    if (this.IsParameter)
      return "Parameter" + (object) this.ParameterIndex;
    object obj = this.Constant.Value;
    return obj != null ? obj.ToString() : "(null)";
  }
}
