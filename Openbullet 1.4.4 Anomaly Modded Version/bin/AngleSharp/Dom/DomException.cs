// Decompiled with JetBrains decompiler
// Type: AngleSharp.Dom.DomException
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Common;
using System;

#nullable disable
namespace AngleSharp.Dom;

public sealed class DomException : Exception, IDomException
{
  public DomException(DomError code)
    : base(code.GetMessage<DomError>())
  {
    this.Code = (int) code;
    this.Name = code.ToString();
  }

  public DomException(string message)
  {
    this.Code = 0;
    this.Name = message;
  }

  public string Name { get; private set; }

  public int Code { get; private set; }
}
