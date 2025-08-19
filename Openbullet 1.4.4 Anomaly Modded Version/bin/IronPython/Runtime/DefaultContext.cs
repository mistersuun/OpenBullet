// Decompiled with JetBrains decompiler
// Type: IronPython.Runtime.DefaultContext
// Assembly: IronPython, Version=2.7.9.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 0569A20E-5DD9-4F74-A766-CC11C6214B7C
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\IronPython.dll

using Microsoft.Scripting.Runtime;
using System.Threading;

#nullable disable
namespace IronPython.Runtime;

public static class DefaultContext
{
  internal static CodeContext _default;
  internal static CodeContext _defaultCLS;

  public static ContextId Id => DefaultContext.Default.LanguageContext.ContextId;

  public static CodeContext Default => DefaultContext._default;

  public static PythonContext DefaultPythonContext => DefaultContext._default.LanguageContext;

  public static CodeContext DefaultCLS => DefaultContext._defaultCLS;

  internal static CodeContext CreateDefaultCLSContext(PythonContext context)
  {
    return new ModuleContext(new PythonDictionary(), context)
    {
      ShowCls = true
    }.GlobalContext;
  }

  internal static void InitializeDefaults(
    CodeContext defaultContext,
    CodeContext defaultClsCodeContext)
  {
    Interlocked.CompareExchange<CodeContext>(ref DefaultContext._default, defaultContext, (CodeContext) null);
    Interlocked.CompareExchange<CodeContext>(ref DefaultContext._defaultCLS, defaultClsCodeContext, (CodeContext) null);
  }
}
