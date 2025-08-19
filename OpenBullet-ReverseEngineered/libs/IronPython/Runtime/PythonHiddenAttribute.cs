// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.PythonHiddenAttribute
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using System;
using System.Reflection;

#nullable disable
namespace IronPython.Runtime;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
public sealed class PythonHiddenAttribute : PlatformsAttribute
{
  public PythonHiddenAttribute(params PlatformID[] hiddenPlatforms)
  {
    this.ValidPlatforms = hiddenPlatforms;
  }

  public PythonHiddenAttribute(
    PlatformsAttribute.PlatformFamily hiddenPlatformFamily)
  {
    this.SetValidPlatforms(hiddenPlatformFamily);
  }

  public static bool IsHidden(MemberInfo m, bool inherit = false)
  {
    return m.IsDefined(typeof (PythonHiddenAttribute), inherit) && ((PlatformsAttribute) m.GetCustomAttributes(typeof (PythonHiddenAttribute), inherit)[0]).IsPlatformValid;
  }
}
