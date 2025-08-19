// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Index
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Utils;

#nullable disable
namespace IronPython.Runtime;

public class Index
{
  private readonly object _value;

  public Index(object value)
  {
    ContractUtils.RequiresNotNull(value, nameof (value));
    this._value = value;
  }

  internal object Value => this._value;
}
