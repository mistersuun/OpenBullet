// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.Operations.UnicodeHelper
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using IronPython.Runtime.Types;
using Microsoft.Scripting.Utils;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace IronPython.Runtime.Operations;

public class UnicodeHelper
{
  internal static BuiltinFunction Function = BuiltinFunction.MakeFunction("unicode", (MethodBase[]) ArrayUtils.ConvertAll<MemberInfo, MethodInfo>(typeof (UnicodeHelper).GetMember("unicode"), (Func<MemberInfo, MethodInfo>) (x => (MethodInfo) x)), typeof (string));

  public static object unicode(CodeContext context) => (object) string.Empty;

  public static object unicode(CodeContext context, object @string)
  {
    return StringOps.FastNewUnicode(context, @string);
  }

  public static object unicode(CodeContext context, object @string, object encoding)
  {
    return StringOps.FastNewUnicode(context, @string, encoding);
  }

  public static object unicode(
    CodeContext context,
    object @string,
    [Optional] object encoding,
    object errors)
  {
    return (object) StringOps.FastNewUnicode(context, @string, encoding, errors);
  }
}
