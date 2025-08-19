// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.InstanceCreator
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Runtime.Types;

internal abstract class InstanceCreator
{
  private readonly PythonType _type;

  protected InstanceCreator(PythonType type) => this._type = type;

  public static InstanceCreator Make(PythonType type)
  {
    return type.IsSystemType ? (InstanceCreator) new SystemInstanceCreator(type) : (InstanceCreator) new UserInstanceCreator(type);
  }

  protected PythonType Type => this._type;

  internal abstract object CreateInstance(CodeContext context);

  internal abstract object CreateInstance(CodeContext context, object arg0);

  internal abstract object CreateInstance(CodeContext context, object arg0, object arg1);

  internal abstract object CreateInstance(
    CodeContext context,
    object arg0,
    object arg1,
    object arg2);

  internal abstract object CreateInstance(CodeContext context, params object[] args);

  internal abstract object CreateInstance(CodeContext context, object[] args, string[] names);
}
