// Decompiled with JetBrains decompiler
// Type: AngleSharp.Scripting.ScriptOptions
// Assembly: AngleSharp, Version=0.13.0.0, Culture=neutral, PublicKeyToken=e83494dcdc6d31ea
// MVID: 339EDA8C-04A7-4BF4-9B64-DA1BC68B2CFB
// Assembly location: C:\Users\futiliter\Documents\Projects\OpenBullet\OpenBullet-ReverseEngineered\libs\AngleSharp.dll

using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Text;

#nullable disable
namespace AngleSharp.Scripting;

public sealed class ScriptOptions
{
  public ScriptOptions(IDocument document, IEventLoop loop)
  {
    this.Document = document;
    this.EventLoop = loop;
  }

  public IEventLoop EventLoop { get; }

  public IDocument Document { get; }

  public IHtmlScriptElement Element { get; set; }

  public Encoding Encoding { get; set; }
}
