// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Types.IPythonObject
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

#nullable disable
namespace IronPython.Runtime.Types;

public interface IPythonObject
{
  PythonDictionary Dict { get; }

  PythonDictionary SetDict(PythonDictionary dict);

  bool ReplaceDict(PythonDictionary dict);

  PythonType PythonType { get; }

  void SetPythonType(PythonType newType);

  object[] GetSlots();

  object[] GetSlotsCreate();
}
