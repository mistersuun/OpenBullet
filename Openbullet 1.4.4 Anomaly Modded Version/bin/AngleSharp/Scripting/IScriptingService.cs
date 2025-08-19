// Decompiled with JetBrains decompiler
// Type: AngleSharp.Scripting.IScriptingService
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Io;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace AngleSharp.Scripting;

public interface IScriptingService
{
  bool SupportsType(string mimeType);

  Task EvaluateScriptAsync(IResponse response, ScriptOptions options, CancellationToken cancel);
}
