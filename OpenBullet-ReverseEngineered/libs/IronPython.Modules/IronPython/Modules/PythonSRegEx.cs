// Decompiled with JetBrains decompiler
// Type: IronPython.Modules.PythonSRegEx
// Assembly: IronPython.Modules, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 7E74D536-11C2-4DF1-B811-06D30EAF7D30
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.Modules.dll

using IronPython.Runtime;

#nullable disable
namespace IronPython.Modules;

public static class PythonSRegEx
{
  public const string __doc__ = "non-functional _sre module.  Included only for completeness.";
  public const int MAGIC = 20031017;
  public const int CODESIZE = 2;

  public static object getlower(CodeContext context, object val, object encoding)
  {
    int int32_1 = context.LanguageContext.ConvertToInt32(val);
    int int32_2 = context.LanguageContext.ConvertToInt32(val);
    return int32_1 == 32 /*0x20*/ ? (object) (int) char.ToLower((char) int32_2) : (object) (int) char.ToLowerInvariant((char) int32_2);
  }

  public static object compile(object a, object b, object c) => (object) null;
}
