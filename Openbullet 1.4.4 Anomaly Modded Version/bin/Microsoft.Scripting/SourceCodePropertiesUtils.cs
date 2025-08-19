// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.SourceCodePropertiesUtils
// Assembly: Microsoft.Scripting, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 548BA5F4-A8C0-402B-BC7F-554AD4A08C69
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Scripting.dll

#nullable disable
namespace Microsoft.Scripting;

public static class SourceCodePropertiesUtils
{
  public static bool IsCompleteOrInvalid(ScriptCodeParseResult props, bool allowIncompleteStatement)
  {
    if (props == ScriptCodeParseResult.Invalid)
      return true;
    if (props == ScriptCodeParseResult.IncompleteToken)
      return false;
    return allowIncompleteStatement || props != ScriptCodeParseResult.IncompleteStatement;
  }
}
