// Decompiled with JetBrains decompiler
// Type: Microsoft.Scripting.AmbiguousFileNameException
// Assembly: Microsoft.Dynamic, Version=1.2.2.0, Culture=neutral, PublicKeyToken=7f709c5b713576e1
// MVID: 6FEC0381-2A2D-402C-8B65-6ED3EE0D3308
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\Microsoft.Dynamic.dll

using Microsoft.Scripting.Utils;
using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Scripting;

[Serializable]
public class AmbiguousFileNameException : Exception
{
  private readonly string _firstPath;
  private readonly string _secondPath;

  public string FirstPath => this._firstPath;

  public string SecondPath => this._secondPath;

  public AmbiguousFileNameException(string firstPath, string secondPath)
    : this(firstPath, secondPath, (string) null, (Exception) null)
  {
  }

  public AmbiguousFileNameException(string firstPath, string secondPath, string message)
    : this(firstPath, secondPath, message, (Exception) null)
  {
  }

  public AmbiguousFileNameException(
    string firstPath,
    string secondPath,
    string message,
    Exception inner)
  {
    string message1 = message;
    if (message1 == null)
      message1 = $"File name is ambiguous; more files are matching the same name (including '{firstPath}' and '{secondPath}')";
    // ISSUE: explicit constructor call
    base.\u002Ector(message1, inner);
    ContractUtils.RequiresNotNull((object) firstPath, nameof (firstPath));
    ContractUtils.RequiresNotNull((object) secondPath, nameof (secondPath));
    this._firstPath = firstPath;
    this._secondPath = secondPath;
  }

  protected AmbiguousFileNameException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue("firstPath", (object) this._firstPath);
    info.AddValue("secondPath", (object) this._secondPath);
    base.GetObjectData(info, context);
  }
}
