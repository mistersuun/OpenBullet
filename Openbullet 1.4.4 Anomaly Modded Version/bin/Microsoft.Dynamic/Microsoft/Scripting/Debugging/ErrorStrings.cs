// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.Debugging.ErrorStrings
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

#nullable disable
namespace Microsoft.Scripting.Debugging;

internal static class ErrorStrings
{
  internal static string JumpNotAllowedInNonLeafFrames
  {
    get => "Frame location can only be changed in leaf frames";
  }

  internal static string DebugContextAlreadyConnectedToTracePipeline
  {
    get
    {
      return "Cannot create TracePipeline because DebugContext is already connected to another TracePipeline";
    }
  }

  internal static string InvalidSourceSpan => "Invalid SourceSpan";

  internal static string SetNextStatementOnlyAllowedInsideTraceback
  {
    get => "Unable to perform SetNextStatement because current thread is not inside a traceback";
  }

  internal static string ITracePipelineClosed
  {
    get => "ITracePipeline cannot be used because it has been closed";
  }

  internal static string InvalidFunctionVersion
  {
    get => "Frame cannot be remapped to function verion {0} because it does not exist";
  }

  internal static string DebugInfoWithoutSymbolDocumentInfo
  {
    get
    {
      return "Unable to transform LambdaExpression because DebugInfoExpression #{0} did not have a valid SymbolDocumentInfo";
    }
  }
}
