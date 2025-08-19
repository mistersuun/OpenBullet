// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.NameType
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;

#nullable disable
namespace IronPython.Runtime;

[Flags]
public enum NameType
{
  None = 0,
  Python = 1,
  Method = 2,
  Field = 4,
  Property = 8,
  Event = 16, // 0x00000010
  Type = 32, // 0x00000020
  BaseTypeMask = Type | Event | Property | Field | Method, // 0x0000003E
  PythonMethod = Method | Python, // 0x00000003
  PythonField = Field | Python, // 0x00000005
  PythonProperty = Property | Python, // 0x00000009
  PythonEvent = Event | Python, // 0x00000011
  PythonType = Type | Python, // 0x00000021
  ClassMember = 64, // 0x00000040
  ClassMethod = ClassMember | PythonMethod, // 0x00000043
}
