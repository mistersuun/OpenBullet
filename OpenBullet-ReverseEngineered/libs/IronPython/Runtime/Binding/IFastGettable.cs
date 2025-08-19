// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Binding.IFastGettable
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System.Runtime.CompilerServices;

#nullable disable
namespace IronPython.Runtime.Binding;

internal interface IFastGettable
{
  T MakeGetBinding<T>(
    CallSite<T> site,
    PythonGetMemberBinder binder,
    CodeContext state,
    string name)
    where T : class;
}
